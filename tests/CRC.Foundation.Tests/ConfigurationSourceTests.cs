using CRC.Foundation;

namespace CRC.Foundation.Tests;

public sealed class ConfigurationSourceTests
{
    [Fact]
    public void EnvironmentOverridesFilesAndMountedSecretsOverrideEnvironment()
    {
        var root = Path.Combine(Path.GetTempPath(), "crc-config-" + Guid.NewGuid().ToString("N"));
        var secrets = Path.Combine(root, "secrets");
        Directory.CreateDirectory(secrets);
        var keys = new[] { "CRC_Environment", "CRC_ConnectionString", "CRC_SecretsDirectory" };
        var previous = keys.ToDictionary(key => key, Environment.GetEnvironmentVariable);
        try
        {
            File.WriteAllText(Path.Combine(root, "appsettings.json"), "{\"ConnectionString\":\"default-marker\"}");
            File.WriteAllText(Path.Combine(root, "appsettings.Test.json"), "{\"ConnectionString\":\"test-marker\"}");
            Environment.SetEnvironmentVariable("CRC_Environment", "Test");
            Environment.SetEnvironmentVariable("CRC_ConnectionString", null);
            Assert.Equal("test-marker", FoundationConfiguration.Load(root, typeof(FoundationOptions).Assembly)["ConnectionString"]);
            Environment.SetEnvironmentVariable("CRC_ConnectionString", "environment-marker");
            Assert.Equal("environment-marker", FoundationConfiguration.Load(root, typeof(FoundationOptions).Assembly)["ConnectionString"]);
            File.WriteAllText(Path.Combine(secrets, "ConnectionString"), "mounted-marker");
            Environment.SetEnvironmentVariable("CRC_Environment", "Production");
            Environment.SetEnvironmentVariable("CRC_SecretsDirectory", secrets);
            Assert.Equal("mounted-marker", FoundationConfiguration.Load(root, typeof(FoundationOptions).Assembly)["ConnectionString"]);
        }
        finally
        {
            foreach (var pair in previous) Environment.SetEnvironmentVariable(pair.Key, pair.Value);
            Directory.Delete(root, true);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(65536)]
    public void InvalidPortsFail(int port)
    {
        var options = ConfigurationTests.Local(Path.Combine(Path.GetTempPath(), "crc-data"));
        options.Port = port;
        Assert.Throws<InvalidOperationException>(() => options.Validate(AppContext.BaseDirectory, Path.Combine(AppContext.BaseDirectory, "wwwroot")));
    }
}
