using System.Net;
using Microsoft.Extensions.Configuration;

namespace CRC.Foundation;

public sealed class FoundationOptions
{
    public string Environment { get; set; } = "";
    public string Profile { get; set; } = "";
    public string Provider { get; set; } = "";
    public string BindAddress { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 5080;
    public bool Https { get; set; }
    public string[] AllowedHosts { get; set; } = ["localhost", "127.0.0.1", "[::1]"];
    public string[] TrustedProxies { get; set; } = [];
    public string DataRoot { get; set; } = "";
    public string ConnectionString { get; set; } = "";

    public void Validate(string applicationRoot, string webRoot)
    {
        if (Environment is not ("Development" or "Test" or "Staging" or "Production") ||
            Profile is not ("Online" or "Local") ||
            (Profile == "Online" ? Provider != "Postgres" : Provider != "Sqlite") ||
            Port is < 1024 or > 65535 ||
            !IPAddress.TryParse(BindAddress, out var address) || !IsPrivate(address) ||
            (!Https && (!IPAddress.IsLoopback(address) || Environment is "Staging" or "Production")) ||
            AllowedHosts.Length == 0 || AllowedHosts.Any(h => string.IsNullOrWhiteSpace(h) || h.Contains('*') || h.Contains('/')) ||
            TrustedProxies.Length != 0 ||
            !Path.IsPathFullyQualified(DataRoot) || DataRoot.StartsWith(@"\\", StringComparison.Ordinal) ||
            DataRoot.StartsWith("//", StringComparison.Ordinal) ||
            IsUnder(DataRoot, applicationRoot) || IsUnder(DataRoot, webRoot) ||
            (Profile == "Online" && string.IsNullOrWhiteSpace(ConnectionString)) ||
            (Profile == "Local" && !string.IsNullOrEmpty(ConnectionString)))
        {
            throw new InvalidOperationException("FOUNDATION_CONFIGURATION_INVALID");
        }

        // Reject symlink/junction indirection to a network or publicly served directory.
        for (var directory = new DirectoryInfo(Path.GetFullPath(DataRoot)); directory is not null; directory = directory.Parent)
        {
            if (directory.Exists && (directory.Attributes & FileAttributes.ReparsePoint) != 0)
                throw new InvalidOperationException("FOUNDATION_DATA_PATH_INVALID");
        }
        if (OperatingSystem.IsWindows() && new DriveInfo(Path.GetPathRoot(DataRoot)!).DriveType != DriveType.Fixed)
            throw new InvalidOperationException("FOUNDATION_DATA_PATH_INVALID");
    }

    private static bool IsUnder(string candidate, string root)
    {
        var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        var fullRoot = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var fullCandidate = Path.GetFullPath(candidate);
        return fullCandidate.Equals(fullRoot, comparison) || fullCandidate.StartsWith(fullRoot + Path.DirectorySeparatorChar, comparison);
    }

    private static bool IsPrivate(IPAddress address)
    {
        if (IPAddress.IsLoopback(address)) return true;
        var bytes = address.GetAddressBytes();
        return bytes.Length == 4 && (bytes[0] == 10 || (bytes[0] == 172 && bytes[1] is >= 16 and <= 31) || (bytes[0] == 192 && bytes[1] == 168));
    }
}

public static class FoundationConfiguration
{
    public static IConfigurationRoot Load(string contentRoot, System.Reflection.Assembly assembly)
    {
        var environment = System.Environment.GetEnvironmentVariable("CRC_Environment") ?? "";
        if (environment is not ("Development" or "Test" or "Staging" or "Production"))
            throw new InvalidOperationException("FOUNDATION_ENVIRONMENT_REQUIRED");
        var builder = new ConfigurationBuilder().SetBasePath(contentRoot)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true);
        if (environment == "Development") builder.AddUserSecrets(assembly, optional: true);
        builder.AddEnvironmentVariables("CRC_");
        if (environment is "Production" or "Staging")
        {
            var directory = System.Environment.GetEnvironmentVariable("CRC_SecretsDirectory");
            if (string.IsNullOrWhiteSpace(directory) || !Path.IsPathFullyQualified(directory) || !Directory.Exists(directory))
                throw new InvalidOperationException("FOUNDATION_SECRET_DIRECTORY_REQUIRED");
            builder.AddKeyPerFile(directory, optional: false);
        }
        var configuration = builder.Build();
        if (configuration["Environment"] != environment)
            throw new InvalidOperationException("FOUNDATION_ENVIRONMENT_MISMATCH");
        return configuration;
    }
}
