using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using CRC.DbMigrator;
using CRC.Persistence.Sqlite;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace CRC.Foundation.Tests;

public sealed class HttpTests : IAsyncLifetime
{
    private readonly string directory = Path.Combine(Path.GetTempPath(), "crc-http-" + Guid.NewGuid().ToString("N"));
    private readonly Dictionary<string, string?> previous = [];
    private WebApplication host = null!;
    private HttpClient client = null!;
    private readonly CapturedLogs logs = new();

    public async ValueTask InitializeAsync()
    {
        Directory.CreateDirectory(directory);
        await MigrationOperations.RunAsync(ConfigurationTests.Local(directory), true, TestContext.Current.CancellationToken);
        foreach (var (key, value) in new Dictionary<string, string> { ["Environment"] = "Test", ["Profile"] = "Local", ["Provider"] = "Sqlite", ["DataRoot"] = directory, ["ConnectionString"] = "" })
        {
            previous[key] = Environment.GetEnvironmentVariable("CRC_" + key);
            Environment.SetEnvironmentVariable("CRC_" + key, value);
        }
        host = CRC.Api.FoundationHost.Build([], builder =>
        {
            builder.WebHost.UseTestServer();
            builder.Logging.AddProvider(logs);
            builder.Services.AddControllers().AddApplicationPart(typeof(ProbeController).Assembly);
        });
        await host.StartAsync(TestContext.Current.CancellationToken);
        client = host.GetTestClient();
    }

    [Fact]
    public async Task HealthHasExactEnvelopeAndSafeCorrelation()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health/live");
        request.Headers.Add("X-Request-ID", "SECRET_MARKER");
        using var response = await client.SendAsync(request, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("{\"data\":{\"status\":\"alive\"},\"meta\":{}}", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        Assert.Matches("^[0-9a-f]{32}$", response.Headers.GetValues("X-Request-ID").Single());
        Assert.Equal("no-store", response.Headers.CacheControl!.ToString());
        Assert.Equal("{\"data\":{\"status\":\"ready\"},\"meta\":{}}", await client.GetStringAsync("/health/ready", TestContext.Current.CancellationToken));
    }

    [Theory]
    [InlineData("/api/v1/missing", 404, "RESOURCE_NOT_FOUND")]
    [InlineData("/openapi/v1.json", 404, "RESOURCE_NOT_FOUND")]
    [InlineData("/test-only/protected", 401, "AUTHENTICATION_REQUIRED")]
    [InlineData("/test-only/throw", 500, "INTERNAL_ERROR")]
    public async Task ErrorsAreSafe(string path, int expected, string code)
    {
        using var response = await client.GetAsync(path + "?secret=SECRET_MARKER", TestContext.Current.CancellationToken);
        Assert.Equal(expected, (int)response.StatusCode);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.DoesNotContain("SECRET_MARKER", body);
        using var json = JsonDocument.Parse(body);
        Assert.Equal(code, json.RootElement.GetProperty("error").GetProperty("code").GetString());
        Assert.Equal(response.Headers.GetValues("X-Request-ID").Single(), json.RootElement.GetProperty("error").GetProperty("requestId").GetString());
        Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"));
    }

    [Fact]
    public async Task MethodValidationMediaAndSizeErrorsAreStable()
    {
        using var method = await client.PostAsync("/health/live", null, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.MethodNotAllowed, method.StatusCode);
        using var malformed = await client.PostAsync("/test-only/validation", new StringContent("{bad", Encoding.UTF8, "application/json"), TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.BadRequest, malformed.StatusCode);
        using var media = await client.PostAsync("/test-only/validation", new StringContent("text", Encoding.UTF8, "text/plain"), TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.UnsupportedMediaType, media.StatusCode);
        using var large = await client.PostAsync("/test-only/validation", new StringContent(new string('x', 1024 * 1024 + 1), Encoding.UTF8, "application/json"), TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.RequestEntityTooLarge, large.StatusCode);
    }

    [Fact]
    public async Task SchemaDivergenceAffectsReadinessButNotLiveness()
    {
        await using var context = new LocalContext(LocalContext.Options(directory));
        await context.Database.ExecuteSqlRawAsync("DELETE FROM \"__EFMigrationsHistory\"", TestContext.Current.CancellationToken);
        using var readiness = await client.GetAsync("/health/ready", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.ServiceUnavailable, readiness.StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/health/live", TestContext.Current.CancellationToken)).StatusCode);
    }

    [Fact]
    public async Task RawSecretsNeverReachLogProvidersOrResponses()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/test-only/throw?value=SECRET_MARKER");
        request.Headers.Add("Authorization", "Bearer SECRET_MARKER");
        request.Headers.Add("Cookie", "session=SECRET_MARKER");
        request.Content = new StringContent("SECRET_MARKER");
        using var response = await client.SendAsync(request, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.DoesNotContain("SECRET_MARKER", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        Assert.NotEmpty(logs.Messages);
        Assert.Contains(logs.Messages, message => message.Contains("REQUEST_FAILED", StringComparison.Ordinal));
        Assert.All(logs.Messages, message => Assert.DoesNotContain("SECRET_MARKER", message));
    }

    [Fact]
    public async Task ProductionRouteInventoryAndOpenApiExcludeTestEndpoints()
    {
        await using var production = CRC.Api.FoundationHost.Build([], builder => builder.WebHost.UseTestServer());
        await production.StartAsync(TestContext.Current.CancellationToken);
        using var productionClient = production.GetTestClient();
        Assert.Equal(HttpStatusCode.NotFound, (await productionClient.GetAsync("/test-only/throw", TestContext.Current.CancellationToken)).StatusCode);
        await using var scope = production.Services.CreateAsyncScope();
        var provider = scope.ServiceProvider.GetRequiredKeyedService<IOpenApiDocumentProvider>("v1");
        var document = await provider.GetOpenApiDocumentAsync(TestContext.Current.CancellationToken);
        Assert.Contains("/health/live", document.Paths.Keys);
        Assert.Contains("/health/ready", document.Paths.Keys);
        Assert.DoesNotContain(document.Paths.Keys, path => path.StartsWith("/test-only", StringComparison.Ordinal));
        var json = await document.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1, TestContext.Current.CancellationToken);
        var output = Environment.GetEnvironmentVariable("CRC_TEST_EVIDENCE_DIR") ?? Path.Combine(AppContext.BaseDirectory, "evidence");
        Directory.CreateDirectory(output);
        await File.WriteAllTextAsync(Path.Combine(output, "openapi.json"), json, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task MissingSchemaPreventsHostStartup()
    {
        await using var context = new LocalContext(LocalContext.Options(directory));
        await context.Database.ExecuteSqlRawAsync("DELETE FROM \"__EFMigrationsHistory\"", TestContext.Current.CancellationToken);
        await using var incompatible = CRC.Api.FoundationHost.Build([], builder => builder.WebHost.UseTestServer());
        await Assert.ThrowsAsync<InvalidOperationException>(() => incompatible.StartAsync(TestContext.Current.CancellationToken));
    }

    public async ValueTask DisposeAsync()
    {
        client?.Dispose();
        if (host is not null) await host.DisposeAsync();
        foreach (var (key, value) in previous) Environment.SetEnvironmentVariable("CRC_" + key, value);
        Directory.Delete(directory, true);
    }
}

[ApiController]
[Route("test-only")]
public sealed class ProbeController : ControllerBase
{
    [HttpGet("protected")]
    public IActionResult Protected() => Ok();
    [AllowAnonymous]
    [HttpGet("throw")]
    public IActionResult Throw() => throw new InvalidOperationException("SECRET_MARKER");
    [AllowAnonymous]
    [HttpPost("validation")]
    public IActionResult Validate([FromBody] ProbeInput input) => Ok(input);
}
public sealed record ProbeInput(int Value);
