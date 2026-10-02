using CRC.DbMigrator;
using CRC.Persistence.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CRC.Foundation.Tests;

public sealed class SqliteTests : IDisposable
{
    private readonly string directory = Path.Combine(Path.GetTempPath(), "crc-fixture-" + Guid.NewGuid().ToString("N"));
    public SqliteTests() => Directory.CreateDirectory(directory);

    [Fact]
    public async Task MigrationIsRepeatableAndDetectsUnknownSchema()
    {
        var options = ConfigurationTests.Local(directory);
        Assert.True(await MigrationOperations.RunAsync(options, true, TestContext.Current.CancellationToken));
        Assert.True(await MigrationOperations.RunAsync(options, true, TestContext.Current.CancellationToken));
        await using var context = new LocalContext(LocalContext.Options(directory));
        Assert.True(await context.IsReadyAsync(TestContext.Current.CancellationToken));
        await context.Database.ExecuteSqlRawAsync("INSERT INTO \"__EFMigrationsHistory\" VALUES ('future', '10.0.12')", TestContext.Current.CancellationToken);
        Assert.False(await context.IsReadyAsync(TestContext.Current.CancellationToken));
        await Assert.ThrowsAsync<InvalidOperationException>(() => MigrationOperations.RunAsync(options, true, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task RealFileHasRequiredPragmasAndNoDomainTables()
    {
        await MigrationOperations.RunAsync(ConfigurationTests.Local(directory), true, TestContext.Current.CancellationToken);
        await using var context = new LocalContext(LocalContext.Options(directory));
        await context.Database.OpenConnectionAsync(TestContext.Current.CancellationToken);
        await using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT sqlite_version()";
        var nativeVersion = Convert.ToString(await command.ExecuteScalarAsync(TestContext.Current.CancellationToken))!;
        Assert.StartsWith("3.", nativeVersion);
        var evidence = Environment.GetEnvironmentVariable("CRC_TEST_EVIDENCE_DIR");
        if (evidence is not null)
        {
            Directory.CreateDirectory(evidence);
            await File.WriteAllTextAsync(Path.Combine(evidence, "sqlite-version.txt"), nativeVersion, TestContext.Current.CancellationToken);
        }
        foreach (var (sql, expected) in new[] { ("PRAGMA journal_mode", "wal"), ("PRAGMA foreign_keys", "1"), ("PRAGMA synchronous", "2"), ("PRAGMA busy_timeout", "5000"), ("PRAGMA integrity_check", "ok") })
        {
            command.CommandText = sql;
            Assert.Equal(expected, Convert.ToString(await command.ExecuteScalarAsync(TestContext.Current.CancellationToken)));
        }
        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name";
        await using var reader = await command.ExecuteReaderAsync(TestContext.Current.CancellationToken);
        Assert.True(await reader.ReadAsync(TestContext.Current.CancellationToken));
        Assert.Equal("__EFMigrationsHistory", reader.GetString(0));
        Assert.False(await reader.ReadAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ConcurrentMigrationCallsSerialize()
    {
        var options = ConfigurationTests.Local(directory);
        var results = await Task.WhenAll(Enumerable.Range(0, 4).Select(_ => Task.Run(() => MigrationOperations.RunAsync(options, true, TestContext.Current.CancellationToken))));
        Assert.All(results, Assert.True);
    }

    [Fact]
    public async Task RealTransactionsConstraintsAndReopenPreserveCommittedData()
    {
        await MigrationOperations.RunAsync(ConfigurationTests.Local(directory), true, TestContext.Current.CancellationToken);
        await using (var context = new LocalContext(LocalContext.Options(directory)))
        {
            await context.Database.ExecuteSqlRawAsync("CREATE TABLE fixture_parent (id INTEGER PRIMARY KEY, label TEXT UNIQUE); CREATE TABLE fixture_child (id INTEGER PRIMARY KEY, parent_id INTEGER REFERENCES fixture_parent(id)); INSERT INTO fixture_parent VALUES (1, 'committed')", TestContext.Current.CancellationToken);
            await Assert.ThrowsAsync<Microsoft.Data.Sqlite.SqliteException>(() => context.Database.ExecuteSqlRawAsync("INSERT INTO fixture_child VALUES (1, 99)", TestContext.Current.CancellationToken));
            await Assert.ThrowsAsync<Microsoft.Data.Sqlite.SqliteException>(() => context.Database.ExecuteSqlRawAsync("INSERT INTO fixture_parent VALUES (2, 'committed')", TestContext.Current.CancellationToken));
            await using var transaction = await context.Database.BeginTransactionAsync(TestContext.Current.CancellationToken);
            await context.Database.ExecuteSqlRawAsync("INSERT INTO fixture_parent VALUES (2, 'rolled-back')", TestContext.Current.CancellationToken);
            await transaction.RollbackAsync(TestContext.Current.CancellationToken);
        }
        await using var reopened = new LocalContext(LocalContext.Options(directory));
        Assert.Equal(1, await reopened.Database.SqlQueryRaw<int>("SELECT COUNT(*) AS Value FROM fixture_parent").SingleAsync(TestContext.Current.CancellationToken));
    }

    public void Dispose() => Directory.Delete(directory, recursive: true);

    [Fact]
    public async Task FailedUpgradeDoesNotCommitDdlOrMigrationHistory()
    {
        var token = TestContext.Current.CancellationToken;
        await MigrationOperations.RunAsync(ConfigurationTests.Local(directory), true, token);
        var options = new DbContextOptionsBuilder<UpgradeFixtureContext>().UseSqlite($"Data Source={Path.Combine(directory, "crc.db")};Pooling=False").Options;
        await using var upgrade = new UpgradeFixtureContext(options);
        await Assert.ThrowsAsync<Microsoft.Data.Sqlite.SqliteException>(() => MigrationOperations.ApplyAsync(upgrade, token));
        Assert.Single(await upgrade.Database.GetPendingMigrationsAsync(token));
        Assert.Single(await upgrade.Database.GetAppliedMigrationsAsync(token));
        Assert.Equal(0, await upgrade.Database.SqlQueryRaw<int>("SELECT COUNT(*) AS Value FROM sqlite_master WHERE name='fixture_upgrade'").SingleAsync(token));
    }

    [Fact]
    public async Task ContentionFailsWithinBoundAndFailedDdlRollsBack()
    {
        var token = TestContext.Current.CancellationToken;
        await MigrationOperations.RunAsync(ConfigurationTests.Local(directory), true, token);
        await using var first = new LocalContext(LocalContext.Options(directory));
        await using var second = new LocalContext(LocalContext.Options(directory));
        await first.Database.ExecuteSqlRawAsync("CREATE TABLE fixture_lock (id INTEGER PRIMARY KEY)", token);
        await using (var transaction = await first.Database.BeginTransactionAsync(token))
        {
            await first.Database.ExecuteSqlRawAsync("INSERT INTO fixture_lock VALUES (1)", token);
            var elapsed = System.Diagnostics.Stopwatch.StartNew();
            var exception = await Assert.ThrowsAsync<Microsoft.Data.Sqlite.SqliteException>(() => second.Database.ExecuteSqlRawAsync("INSERT INTO fixture_lock VALUES (2)", token));
            Assert.Equal(5, exception.SqliteErrorCode);
            Assert.InRange(elapsed.Elapsed.TotalSeconds, 4, 8);
            await transaction.RollbackAsync(token);
        }
        await using (var transaction = await first.Database.BeginTransactionAsync(token))
        {
            await first.Database.ExecuteSqlRawAsync("CREATE TABLE fixture_failed_migration (id INTEGER PRIMARY KEY)", token);
            await Assert.ThrowsAsync<Microsoft.Data.Sqlite.SqliteException>(() => first.Database.ExecuteSqlRawAsync("INSERT INTO fixture_nonexistent VALUES (1)", token));
            await transaction.RollbackAsync(token);
        }
        Assert.Equal(0, await first.Database.SqlQueryRaw<int>("SELECT COUNT(*) AS Value FROM sqlite_master WHERE name='fixture_failed_migration'").SingleAsync(token));
    }
}
