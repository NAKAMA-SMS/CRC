using CRC.Foundation;
using CRC.Persistence.Postgres;
using CRC.Persistence.Sqlite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;

namespace CRC.DbMigrator;

public static class MigrationOperations
{
    public static async Task<bool> RunAsync(FoundationOptions options, bool apply, CancellationToken cancellationToken)
    {
        if (options.Profile == "Local")
        {
            if (!apply)
            {
                await using var status = new LocalContext(LocalContext.Options(options.DataRoot));
                return await status.IsReadyAsync(cancellationToken);
            }
            if (!Directory.Exists(options.DataRoot)) throw new InvalidOperationException("PROTECTED_DATA_DIRECTORY_REQUIRED");
            await using var migrationLock = await AcquireFileLockAsync(Path.Combine(options.DataRoot, "migration.lock"), cancellationToken);
            // Creation and WAL changes belong exclusively to the explicit migrator.
            await using (var create = new SqliteConnection(new SqliteConnectionStringBuilder
            {
                DataSource = Path.Combine(options.DataRoot, "crc.db"),
                Mode = SqliteOpenMode.ReadWriteCreate,
                Pooling = false
            }.ToString()))
            {
                await create.OpenAsync(cancellationToken);
                await using var command = create.CreateCommand();
                command.CommandText = "PRAGMA journal_mode=WAL;";
                if (!string.Equals(Convert.ToString(await command.ExecuteScalarAsync(cancellationToken)), "wal", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("WAL_REQUIRED");
            }
            await using var local = new LocalContext(LocalContext.Options(options.DataRoot));
            await ApplyAsync(local, cancellationToken);
            return await local.IsReadyAsync(cancellationToken);
        }
        await using var online = new OnlineContext(OnlineContext.Options(options.ConnectionString, options.Environment is "Development" or "Test"));
        if (apply)
        {
            await online.Database.OpenConnectionAsync(cancellationToken);
            await using var command = online.Database.GetDbConnection().CreateCommand();
            command.CommandText = "SELECT pg_advisory_lock(485018230000);";
            command.CommandTimeout = 30;
            await command.ExecuteNonQueryAsync(cancellationToken);
            try { await ApplyAsync(online, cancellationToken); }
            finally
            {
                command.CommandText = "SELECT pg_advisory_unlock(485018230000);";
                await command.ExecuteNonQueryAsync(CancellationToken.None);
            }
        }
        return await online.IsReadyAsync(cancellationToken);
    }

    internal static async Task ApplyAsync(DbContext context, CancellationToken cancellationToken)
    {
        var expected = context.Database.GetMigrations().ToArray();
        var applied = (await context.Database.GetAppliedMigrationsAsync(cancellationToken)).ToArray();
        if (!applied.SequenceEqual(expected.Take(applied.Length)) || applied.Length > expected.Length)
            throw new InvalidOperationException("UNKNOWN_MIGRATION_STATE");
        if (applied.SequenceEqual(expected)) return;
        // External provider-specific lock covers inspection and application. Use EF-generated
        // SQL in one transaction without EF's extra SQLite lock table in the baseline schema.
        var script = context.GetService<IMigrator>().GenerateScript(applied.LastOrDefault(), expected[^1], MigrationsSqlGenerationOptions.NoTransactions);
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        await context.Database.ExecuteSqlRawAsync(script, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private static async Task<FileStream> AcquireFileLockAsync(string path, CancellationToken cancellationToken)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(30));
        while (true)
        {
            timeout.Token.ThrowIfCancellationRequested();
            try { return new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None); }
            catch (IOException) { await Task.Delay(100, timeout.Token); }
        }
    }
}
