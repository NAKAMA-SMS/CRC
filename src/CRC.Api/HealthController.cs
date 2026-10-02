using CRC.Foundation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRC.Api;

[ApiController]
[Produces("application/json")]
[AllowAnonymous]
[Route("health")]
public sealed class HealthController(IDatabaseProbe probe, ILogger<HealthController> logger) : ControllerBase
{
    [HttpGet("live")]
    [ProducesResponseType<HealthEnvelope>(200)]
    public ActionResult<HealthEnvelope> Live()
    {
        Response.Headers.CacheControl = "no-store";
        return new HealthEnvelope(new("alive"), new { });
    }

    [HttpGet("ready")]
    [ProducesResponseType<HealthEnvelope>(200)]
    [ProducesResponseType<ErrorEnvelope>(503)]
    public async Task<ActionResult<HealthEnvelope>> Ready(CancellationToken cancellationToken)
    {
        Response.Headers.CacheControl = "no-store";
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(2));
        try
        {
            if (await probe.IsReadyAsync(timeout.Token).WaitAsync(timeout.Token)) return new HealthEnvelope(new("ready"), new { });
        }
        catch (Exception exception)
        {
            SafeLogging.Failure(logger, 1003, "READINESS_FAILED", exception);
        }
        return StatusCode(503, ApiErrors.Create(503, HttpContext.TraceIdentifier));
    }
}
