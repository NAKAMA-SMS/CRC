using CRC.Foundation;
using Microsoft.EntityFrameworkCore;

namespace CRC.Persistence.Postgres;

public sealed class OnlineContext(DbContextOptions<OnlineContext> options) : DbContext(options), IDatabaseProbe
{
    public static DbContextOptions<OnlineContext> Options(string connectionString, bool allowLoopbackFixture = false)
    {
        var settings = new Npgsql.NpgsqlConnectionStringBuilder(connectionString);
        if (string.IsNullOrWhiteSpace(settings.Host) || string.IsNullOrWhiteSpace(settings.Database) ||
            string.IsNullOrWhiteSpace(settings.Username) || string.IsNullOrWhiteSpace(settings.Password))
            throw new InvalidOperationException("DATABASE_CREDENTIALS_REQUIRED");
        var loopback = settings.Host is "localhost" or "127.0.0.1" or "::1";
        if (!(allowLoopbackFixture && loopback) && settings.SslMode != Npgsql.SslMode.VerifyFull)
            throw new InvalidOperationException("DATABASE_TLS_REQUIRED");
        return new DbContextOptionsBuilder<OnlineContext>().UseNpgsql(settings.ConnectionString).Options;
    }

    public async Task<bool> IsReadyAsync(CancellationToken cancellationToken)
    {
        Database.SetCommandTimeout(2);
        var actual = (await Database.GetAppliedMigrationsAsync(cancellationToken)).ToArray();
        return actual.SequenceEqual(Database.GetMigrations());
    }
}
