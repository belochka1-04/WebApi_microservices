using Microsoft.AspNetCore.Mvc;
using WebApi_PartsService.Services;

namespace WebApi_PartsService.Controllers;

[ApiController]
[Route("api/PartsGrabberSites")]
public sealed class PartsGrabberSitesController(IConfiguration config) : ControllerBase
{
    [HttpGet("get")]
    public async Task<ActionResult<GrabberCachedPart>> Get([FromQuery] string partNumber, [FromQuery] string sourceSite, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(partNumber) || string.IsNullOrWhiteSpace(sourceSite))
            return BadRequest("Part number and source are required.");
        var cache = new GrabberCache(config.GetConnectionString("KameraDb") ?? throw new InvalidOperationException("Missing KameraDb connection."));
        var result = await cache.ReadAsync(partNumber, sourceSite, ct);
        return result is null ? NotFound() : Ok(result);
    }
}
