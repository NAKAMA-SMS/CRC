using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace CRC.Foundation;

public static class SafeLogging
{
    public static ILoggingBuilder ConfigureJson(ILoggingBuilder builder) => builder.AddJsonConsole(settings =>
    {
        settings.UseUtcTimestamp = true;
        settings.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
        settings.IncludeScopes = true;
    });

    public static void Failure(ILogger logger, int eventId, string code, Exception exception)
    {
        // No exception object/message, source path, SQL, connection details or user input.
        var locations = new StackTrace(exception, false).GetFrames()
            .Select(frame => frame.GetMethod()?.DeclaringType)
            .Where(type => type?.Namespace?.StartsWith("CRC.", StringComparison.Ordinal) == true)
            .Select(type => type!.FullName).Distinct().Take(8).ToArray();
        logger.LogError(new EventId(eventId, code), "{DiagnosticCode} {ExceptionType} {Locations}",
            code, exception.GetType().Name, string.Join(",", locations));
    }
}
