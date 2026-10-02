using CRC.Api;
using CRC.Foundation;
using CRC.Persistence.Postgres;
using CRC.Persistence.Sqlite;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

try
{
    var app = FoundationHost.Build(args);
    await app.RunAsync();
    return 0;
}
catch (Exception exception)
{
    using var logs = LoggerFactory.Create(builder => SafeLogging.ConfigureJson(builder));
    SafeLogging.Failure(logs.CreateLogger("CRC.Startup"), 1000, "STARTUP_FAILED", exception);
    return 1;
}

public partial class Program;

namespace CRC.Api
{
    public static class FoundationHost
    {
        public static WebApplication Build(string[] args, Action<WebApplicationBuilder>? configure = null)
        {
            if (args.Length != 0) throw new InvalidOperationException("COMMAND_LINE_CONFIGURATION_FORBIDDEN");
            var root = AppContext.BaseDirectory;
            var config = FoundationConfiguration.Load(root, typeof(Program).Assembly);
            var options = config.Get<FoundationOptions>() ?? throw new InvalidOperationException("CONFIGURATION_REQUIRED");
            options.Validate(root, Path.Combine(root, "wwwroot"));
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = [],
                ApplicationName = typeof(HealthController).Assembly.FullName,
                ContentRootPath = root,
                WebRootPath = Path.Combine(root, "wwwroot"),
                EnvironmentName = options.Environment
            });
            builder.Configuration.Sources.Clear();
            builder.Configuration.AddConfiguration(config);
            builder.Configuration["AllowedHosts"] = string.Join(';', options.AllowedHosts);
            builder.Logging.ClearProviders();
            // Framework providers can include request URLs or raw database exceptions.
            builder.Logging.AddFilter((category, _) => category?.StartsWith("CRC.", StringComparison.Ordinal) == true);
            builder.Logging.AddJsonConsole(settings =>
            {
                settings.UseUtcTimestamp = true;
                settings.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
                settings.IncludeScopes = true;
            });
            builder.Services.AddWindowsService();
            builder.WebHost.ConfigureKestrel(server =>
            {
                server.AddServerHeader = false;
                server.Limits.MaxRequestBodySize = 1024 * 1024;
                server.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(15);
                server.Listen(System.Net.IPAddress.Parse(options.BindAddress), options.Port, listen =>
                {
                    if (options.Https) listen.UseHttps();
                });
            });
            builder.Services.AddSingleton(Options.Create(options));
            if (options.Profile == "Local")
            {
                builder.Services.AddScoped(_ => new LocalContext(LocalContext.Options(options.DataRoot)));
                builder.Services.AddScoped<IDatabaseProbe>(services => services.GetRequiredService<LocalContext>());
            }
            else
            {
                builder.Services.AddScoped(_ => new OnlineContext(OnlineContext.Options(options.ConnectionString, options.Environment is "Development" or "Test")));
                builder.Services.AddScoped<IDatabaseProbe>(services => services.GetRequiredService<OnlineContext>());
            }
            builder.Services.AddHostedService<SchemaStartupCheck>();
            builder.Services.AddAuthorization(auth => auth.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAssertion(_ => false).Build());
            builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, DenyResultHandler>();
            builder.Services.AddControllers().ConfigureApiBehaviorOptions(api =>
            {
                api.InvalidModelStateResponseFactory = action => new BadRequestObjectResult(ApiErrors.Create(400, action.HttpContext.TraceIdentifier));
                api.SuppressMapClientErrors = true;
            });
            builder.Services.AddOpenApi();
            configure?.Invoke(builder);
            var app = builder.Build();
            app.Use(async (context, next) =>
            {
                context.TraceIdentifier = Guid.NewGuid().ToString("N");
                context.Response.Headers["X-Request-ID"] = context.TraceIdentifier;
                context.Response.Headers.ContentSecurityPolicy = "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self' data:; connect-src 'self'; object-src 'none'; base-uri 'none'; frame-ancestors 'none'";
                context.Response.Headers.XContentTypeOptions = "nosniff";
                context.Response.Headers["Referrer-Policy"] = "no-referrer";
                if (context.Request.IsHttps && options.Environment is "Production" or "Staging")
                    context.Response.Headers.StrictTransportSecurity = "max-age=31536000";
                using var scope = app.Logger.BeginScope(new Dictionary<string, object> { ["RequestId"] = context.TraceIdentifier });
                try
                {
                    if (context.Request.ContentLength > 1024 * 1024)
                        await ApiErrors.WriteAsync(context, 413);
                    else
                        await next(context);
                }
                catch (BadHttpRequestException exception)
                {
                    if (context.Response.HasStarted) { context.Abort(); return; }
                    await ApiErrors.WriteAsync(context, exception.StatusCode);
                }
                catch (Exception exception)
                {
                    SafeLogging.Failure(app.Logger, 1001, "REQUEST_FAILED", exception);
                    if (context.Response.HasStarted) { context.Abort(); return; }
                    await ApiErrors.WriteAsync(context, 500);
                }
                app.Logger.LogInformation(new EventId(1002, "REQUEST_COMPLETED"), "{DiagnosticCode} {StatusCode}", "REQUEST_COMPLETED", context.Response.StatusCode);
            });
            app.UseStatusCodePages(async status => await ApiErrors.WriteAsync(status.HttpContext, status.HttpContext.Response.StatusCode));
            app.UseRouting();
            app.Use(async (context, next) =>
            {
                if (context.GetEndpoint() is null)
                {
                    await ApiErrors.WriteAsync(context, 404);
                    return;
                }
                // ASP.NET routing's synthetic method-rejection endpoint has no route
                // pattern. Run it before fallback authorization; never bypass a mapped route.
                if (context.GetEndpoint() is not Microsoft.AspNetCore.Routing.RouteEndpoint)
                {
                    await context.GetEndpoint()!.RequestDelegate!(context);
                    return;
                }
                await next(context);
            });
            app.UseAuthorization();
            app.MapControllers();
            // Endpoint metadata makes anonymous static exposure explicit. No SPA fallback.
            app.MapGet("/", () => Results.File(Path.Combine(root, "wwwroot", "index.html"), "text/html")).AllowAnonymous();
            var staticRoot = Path.Combine(root, "wwwroot", "assets");
            if (Directory.Exists(staticRoot))
            {
                var types = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
                foreach (var file in Directory.EnumerateFiles(staticRoot))
                {
                    if (!types.TryGetContentType(file, out var contentType)) continue;
                    app.MapGet("/assets/" + Path.GetFileName(file), () => Results.File(file, contentType)).AllowAnonymous();
                }
            }
            // Routing-generated 404/405 endpoints are handled before fallback authorization.
            return app;
        }
    }

    public sealed class DenyResultHandler : IAuthorizationMiddlewareResultHandler
    {
        public Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult result) =>
            result.Succeeded ? next(context) : ApiErrors.WriteAsync(context, context.User.Identity?.IsAuthenticated == true ? 403 : 401);
    }

    public sealed class SchemaStartupCheck(IServiceScopeFactory scopes) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await using var scope = scopes.CreateAsyncScope();
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            // Total cold startup includes provider/model initialization. Database
            // commands and HTTP readiness still retain their two-second budgets.
            timeout.CancelAfter(TimeSpan.FromSeconds(30));
            if (!await scope.ServiceProvider.GetRequiredService<IDatabaseProbe>().IsReadyAsync(timeout.Token).WaitAsync(timeout.Token))
                throw new InvalidOperationException("SCHEMA_INCOMPATIBLE");
        }
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
