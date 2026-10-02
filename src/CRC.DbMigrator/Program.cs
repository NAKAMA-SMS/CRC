using CRC.DbMigrator;
using CRC.Foundation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using var logs = LoggerFactory.Create(builder => SafeLogging.ConfigureJson(builder));
try
{
    if (args.Length != 1 || args[0] is not ("status" or "apply")) throw new InvalidOperationException("EXPECTED_STATUS_OR_APPLY");
    var root = AppContext.BaseDirectory;
    var options = FoundationConfiguration.Load(root, typeof(MigrationOperations).Assembly).Get<FoundationOptions>()
        ?? throw new InvalidOperationException("CONFIGURATION_REQUIRED");
    options.Validate(root, Path.Combine(root, "wwwroot"));
    var ready = await MigrationOperations.RunAsync(options, args[0] == "apply", CancellationToken.None);
    Console.WriteLine(ready ? "SCHEMA_CURRENT" : "SCHEMA_INCOMPATIBLE");
    return ready ? 0 : 1;
}
catch (Exception exception)
{
    SafeLogging.Failure(logs.CreateLogger("CRC.Migrations"), 1004, "MIGRATION_FAILED", exception);
    return 1;
}
