using CRC.Foundation;

namespace CRC.Foundation.Tests;

public sealed class ConfigurationTests
{
    internal static FoundationOptions Local(string directory) => new()
    {
        Environment = "Test",
        Profile = "Local",
        Provider = "Sqlite",
        DataRoot = directory
    };

    [Theory]
    [InlineData("Environment", "Unknown")]
    [InlineData("Profile", "Unknown")]
    [InlineData("Provider", "Postgres")]
    [InlineData("BindAddress", "0.0.0.0")]
    [InlineData("BindAddress", "8.8.8.8")]
    [InlineData("BindAddress", "10.0.0.1")]
    [InlineData("DataRoot", "relative")]
    [InlineData("ConnectionString", "SECRET_MARKER")]
    public void InvalidConfigurationFailsSafely(string property, string value)
    {
        var options = Local(Path.Combine(Path.GetTempPath(), "crc-test-data"));
        typeof(FoundationOptions).GetProperty(property)!.SetValue(options, value);
        var exception = Assert.Throws<InvalidOperationException>(() => options.Validate(AppContext.BaseDirectory, Path.Combine(AppContext.BaseDirectory, "wwwroot")));
        Assert.DoesNotContain("SECRET_MARKER", exception.Message);
    }

    [Fact]
    public void DataCannotBeStoredAlongsidePublishedFiles()
    {
        var options = Local(Path.Combine(AppContext.BaseDirectory, "wwwroot", "private"));
        Assert.Throws<InvalidOperationException>(() => options.Validate(AppContext.BaseDirectory, Path.Combine(AppContext.BaseDirectory, "wwwroot")));
    }

    [Fact]
    public void LocalDoesNotRequireCloudSecrets()
    {
        var options = Local(Path.Combine(Path.GetTempPath(), "crc-test-data"));
        options.Validate(AppContext.BaseDirectory, Path.Combine(AppContext.BaseDirectory, "wwwroot"));
    }
}
