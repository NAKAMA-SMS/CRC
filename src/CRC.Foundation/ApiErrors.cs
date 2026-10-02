using Microsoft.AspNetCore.Http;

namespace CRC.Foundation;

public sealed record ErrorDetail(string Field, string Code);
public sealed record ErrorBody(string Code, string Message, ErrorDetail[] Details, string RequestId);
public sealed record ErrorEnvelope(ErrorBody Error);
public sealed record HealthData(string Status);
public sealed record HealthEnvelope(HealthData Data, object Meta);

public static class ApiErrors
{
    public static ErrorEnvelope Create(int status, string requestId, ErrorDetail[]? details = null)
    {
        var (code, message) = status switch
        {
            400 => ("VALIDATION_FAILED", "The request is invalid."),
            401 => ("AUTHENTICATION_REQUIRED", "Authentication is required."),
            403 => ("FORBIDDEN", "Access is denied."),
            404 => ("RESOURCE_NOT_FOUND", "The resource was not found."),
            405 => ("METHOD_NOT_ALLOWED", "The request method is not allowed."),
            413 => ("REQUEST_TOO_LARGE", "The request is too large."),
            415 => ("UNSUPPORTED_MEDIA_TYPE", "The media type is not supported."),
            503 => ("SERVICE_UNAVAILABLE", "The service is unavailable."),
            _ => ("INTERNAL_ERROR", "An unexpected error occurred.")
        };
        return new(new(code, message, details ?? [], requestId));
    }

    public static async Task WriteAsync(HttpContext context, int status)
    {
        context.Response.StatusCode = status;
        context.Response.Headers.CacheControl = "no-store";
        await context.Response.WriteAsJsonAsync(Create(status, context.TraceIdentifier), context.RequestAborted);
    }
}
