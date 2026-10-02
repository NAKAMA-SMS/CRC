using CRC.Foundation;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace CRC.Persistence.Sqlite;

public sealed class LocalContext(DbContextOptions<LocalContext> options) : DbContext(options), IDatabaseProbe
{
    public static DbContextOptions<LocalContext> Options(string dataRoot)
    {
        var connection = new SqliteConnectionStringBuilder
        {
            DataSource = Path.Combine(dataRoot, "crc.db"),
            Mode = SqliteOpenMode.ReadWrite,
            ForeignKeys = true,
            DefaultTimeout = 5,
            Pooling = false
        };
        return new DbContextOptionsBuilder<LocalContext>().UseSqlite(connection.ToString())
            .AddInterceptors(new LocalConnectionSettings()).Options;
    }

    public async Task<bool> IsReadyAsync(CancellationToken cancellationToken)
    {
        Database.SetCommandTimeout(2);
        var actual = (await Database.GetAppliedMigrationsAsync(cancellationToken)).ToArray();
        return actual.SequenceEqual(Database.GetMigrations());
    }
}

internal sealed class LocalConnectionSettings : DbConnectionInterceptor
{
    private static void Configure(DbConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA foreign_keys=ON; PRAGMA synchronous=FULL; PRAGMA busy_timeout=5000;";
        command.ExecuteNonQuery();
    }
    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData) => Configure(connection);
    public override Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        Configure(connection);
        return Task.CompletedTask;
    }
}
