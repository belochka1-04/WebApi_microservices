using Microsoft.AspNetCore.Mvc;
using WebApi_GoogleSearchTemplatesService.Services;

namespace WebApi_GoogleSearchTemplatesService.Controllers;

public partial class GoogleModelRequestsController
{
    [HttpPost("{id:int}/lease/{operation}")]
    public async Task<IActionResult> UpdateLease(int id, string operation, [FromBody] GoogleRequestLeaseRequest request)
    {
        if (id <= 0 || request.AttemptCount <= 0 || string.IsNullOrWhiteSpace(request.WorkerId) || request.WorkerId.Length > 128 ||
            operation is not ("renew" or "complete" or "fail")) return BadRequest("Invalid Google request lease.");
        return await _databaseService.UpdateGoogleRequestLeaseAsync(id, request, operation)
            ? NoContent() : Conflict("Google request lease expired or owner changed.");
    }
}
