using CRC.DbMigrator;
using CRC.Foundation;
using CRC.Persistence.Postgres;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Microsoft.AspNetCore.TestHost;
using System.Net;

namespace CRC.Foundation.Tests;

public sealed class PostgresTests : IAsyncLifetime
{
    private readonly string id = "crc_test_" + Guid.NewGuid().ToString("N");
    private NpgsqlConnection admin = null!;
    private string migrationConnection = "";
    private string runtimeConnection = "";
    private CancellationToken Token => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        var input = Environment.GetEnvironmentVariable("CRC_TEST_POSTGRES_ADMIN");
        Assert.False(string.IsNullOrWhiteSpace(input), "BLOCKED: CRC_TEST_POSTGRES_ADMIN must point to a disposable PostgreSQL 17 fixture.");
        Assert.Equal("1", Environment.GetEnvironmentVariable("CRC_TEST_DISPOSABLE"));
        var builder = new NpgsqlConnectionStringBuilder(input);
        Assert.True(builder.Host is "127.0.0.1" or "localhost", "Disposable PostgreSQL fixture must bind loopback.");
        Assert.Equal("postgres", builder.Database);
        admin = new NpgsqlConnection(builder.ConnectionString);
        await admin.OpenAsync(Token);
        Assert.Equal(17, admin.PostgreSqlVersion.Major);
        var evidence = Environment.GetEnvironmentVariable("CRC_TEST_EVIDENCE_DIR");
        if (evidence is not null)
        {
            Directory.CreateDirectory(evidence);
            await File.WriteAllTextAsync(Path.Combine(evidence, "postgres-version.txt"), admin.PostgreSqlVersion.ToString(), Token);
        }
        var password = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
        // All interpolated identifiers/credential text here are test-generated restricted hex,
        // never external input. These roles/databases exist only in the disposable fixture.
        await Execute(admin, $"CREATE ROLE {id}_migration LOGIN PASSWORD '{password}'; CREATE ROLE {id}_runtime LOGIN PASSWORD '{password}';", Token);
        await Execute(admin, $"CREATE DATABASE {id} OWNER {id}_migration", Token);
        builder.Database = id;
        builder.Username = id + "_migration";
        builder.Password = password;
        builder.Pooling = false;
        migrationConnection = builder.ConnectionString;
        builder.Username = id + "_runtime";
        runtimeConnection = builder.ConnectionString;
        await using var migration = new NpgsqlConnection(migrationConnection);
        await migration.OpenAsync(Token);
        await Execute(migration, $"REVOKE CREATE ON SCHEMA public FROM PUBLIC; GRANT USAGE ON SCHEMA public TO {id}_runtime; ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT ON TABLES TO {id}_runtime", Token);
    }

    private FoundationOptions Options() => new()
    {
        Environment = "Test",
        Profile = "Online",
        Provider = "Postgres",
        ConnectionString = migrationConnection,
        DataRoot = Path.Combine(Path.GetTempPath(), "crc-fixtures")
    };

    [Fact]
    public async Task BaselineMigrationIsRepeatableAndRuntimeCannotPerformDdl()
    {
        Assert.True(await MigrationOperations.RunAsync(Options(), true, Token));
        Assert.True(await MigrationOperations.RunAsync(Options(), true, Token));
        await using var context = new OnlineContext(OnlineContext.Options(runtimeConnection, allowLoopbackFixture: true));
        Assert.True(await context.IsReadyAsync(Token));
        var exception = await Assert.ThrowsAsync<PostgresException>(() => context.Database.ExecuteSqlRawAsync("CREATE TABLE forbidden (id integer)", Token));
        Assert.Equal(PostgresErrorCodes.InsufficientPrivilege, exception.SqlState);
        await using var migration = new OnlineContext(OnlineContext.Options(migrationConnection, allowLoopbackFixture: true));
        await migration.Database.ExecuteSqlRawAsync("INSERT INTO \"__EFMigrationsHistory\" VALUES ('future', '10.0.12')", Token);
        Assert.False(await context.IsReadyAsync(Token));
        await Assert.ThrowsAsync<InvalidOperationException>(() => MigrationOperations.RunAsync(Options(), true, Token));
    }

    [Fact]
    public async Task ConcurrentMigratorsSerialize()
    {
        var results = await Task.WhenAll(Enumerable.Range(0, 3).Select(_ => MigrationOperations.RunAsync(Options(), true, Token)));
        Assert.All(results, Assert.True);
    }

    [Fact]
    public async Task ConstraintsRollbackAndReopenUseRealPostgres()
    {
        await using (var connection = new NpgsqlConnection(migrationConnection))
        {
            await connection.OpenAsync(Token);
            await Execute(connection, "CREATE TABLE fixture_parent (id integer PRIMARY KEY, label text UNIQUE); CREATE TABLE fixture_child (id integer PRIMARY KEY, parent_id integer REFERENCES fixture_parent(id)); INSERT INTO fixture_parent VALUES (1, 'committed')", Token);
            Assert.Equal(PostgresErrorCodes.ForeignKeyViolation, (await Assert.ThrowsAsync<PostgresException>(() => Execute(connection, "INSERT INTO fixture_child VALUES (1, 99)", Token))).SqlState);
            Assert.Equal(PostgresErrorCodes.UniqueViolation, (await Assert.ThrowsAsync<PostgresException>(() => Execute(connection, "INSERT INTO fixture_parent VALUES (2, 'committed')", Token))).SqlState);
            await using var transaction = await connection.BeginTransactionAsync(Token);
            await Execute(connection, "INSERT INTO fixture_parent VALUES (2, 'rolled-back')", Token);
            await transaction.RollbackAsync(Token);
        }
        await using var reopened = new NpgsqlConnection(migrationConnection);
        await reopened.OpenAsync(Token);
        await using var count = new NpgsqlCommand("SELECT COUNT(*) FROM fixture_parent", reopened);
        Assert.Equal(1L, await count.ExecuteScalarAsync(Token));
    }

    private static async Task Execute(NpgsqlConnection connection, string sql, CancellationToken token)
    {
        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync(token);
    }

    [Fact]
    public async Task OnlineHostUsesRuntimeRoleAndSurvivesDatabaseLossForLiveness()
    {
        await MigrationOperations.RunAsync(Options(), true, Token);
        var configuration = new Dictionary<string, string>
        {
            ["Environment"] = "Test",
            ["Profile"] = "Online",
            ["Provider"] = "Postgres",
            ["ConnectionString"] = runtimeConnection,
            ["DataRoot"] = Path.Combine(Path.GetTempPath(), id)
        };
        var previous = configuration.Keys.ToDictionary(key => key, key => Environment.GetEnvironmentVariable("CRC_" + key));
        try
        {
            foreach (var pair in configuration) Environment.SetEnvironmentVariable("CRC_" + pair.Key, pair.Value);
            await using var host = CRC.Api.FoundationHost.Build([], builder => builder.WebHost.UseTestServer());
            await host.StartAsync(Token);
            using var client = host.GetTestClient();
            Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/health/ready", Token)).StatusCode);
            await using (var blocked = new NpgsqlConnection(migrationConnection))
            {
                await blocked.OpenAsync(Token);
                await using var transaction = await blocked.BeginTransactionAsync(Token);
                await Execute(blocked, "LOCK TABLE \"__EFMigrationsHistory\" IN ACCESS EXCLUSIVE MODE", Token);
                var timeoutClock = System.Diagnostics.Stopwatch.StartNew();
                Assert.Equal(HttpStatusCode.ServiceUnavailable, (await client.GetAsync("/health/ready", Token)).StatusCode);
                Assert.InRange(timeoutClock.Elapsed.TotalSeconds, 1.5, 3.5);
                Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/health/live", Token)).StatusCode);
                await transaction.RollbackAsync(Token);
            }
            Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/health/ready", Token)).StatusCode);
            await Execute(admin, $"ALTER DATABASE {id} ALLOW_CONNECTIONS false", Token);
            var clock = System.Diagnostics.Stopwatch.StartNew();
            Assert.Equal(HttpStatusCode.ServiceUnavailable, (await client.GetAsync("/health/ready", Token)).StatusCode);
            Assert.True(clock.Elapsed < TimeSpan.FromSeconds(3));
            Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/health/live", Token)).StatusCode);
        }
        finally
        {
            foreach (var pair in previous) Environment.SetEnvironmentVariable("CRC_" + pair.Key, pair.Value);
        }
    }

    [Fact]
    public async Task FailedUpgradeDoesNotCommitDdlOrMigrationHistory()
    {
        await MigrationOperations.RunAsync(Options(), true, Token);
        await using var upgrade = new UpgradeFixtureContext(new DbContextOptionsBuilder<UpgradeFixtureContext>().UseNpgsql(migrationConnection).Options);
        await Assert.ThrowsAsync<PostgresException>(() => MigrationOperations.ApplyAsync(upgrade, Token));
        Assert.Single(await upgrade.Database.GetPendingMigrationsAsync(Token));
        Assert.Single(await upgrade.Database.GetAppliedMigrationsAsync(Token));
        Assert.Equal(0, await upgrade.Database.SqlQueryRaw<int>("SELECT COUNT(*)::integer AS \"Value\" FROM information_schema.tables WHERE table_name='fixture_upgrade'").SingleAsync(Token));
    }

    public async ValueTask DisposeAsync()
    {
        if (admin is null) return;
        // Only this fixture's generated database and roles are eligible for cleanup.
        await Execute(admin, $"DROP DATABASE IF EXISTS {id} WITH (FORCE)", CancellationToken.None);
        await Execute(admin, $"DROP ROLE IF EXISTS {id}_runtime; DROP ROLE IF EXISTS {id}_migration", CancellationToken.None);
        await admin.DisposeAsync();
    }
}
