using CRC.Persistence.Postgres;
using Npgsql;

namespace CRC.Foundation.Tests;

public sealed class TransportConfigurationTests
{
    [Theory]
    [InlineData("example.invalid", false)]
    [InlineData("example.invalid", true)]
    [InlineData("127.0.0.1", false)]
    public void UnverifiedDatabaseTransportIsRejectedBeforeConnecting(string host, bool fixture)
    {
        var settings = new NpgsqlConnectionStringBuilder
        {
            Host = host,
            Database = "fixture",
            Username = "fixture",
            Password = Guid.NewGuid().ToString("N"),
            SslMode = SslMode.Prefer
        };
        Assert.Throws<InvalidOperationException>(() => OnlineContext.Options(settings.ConnectionString, fixture));
        settings.SslMode = SslMode.VerifyFull;
        Assert.NotNull(OnlineContext.Options(settings.ConnectionString, fixture));
    }
}
