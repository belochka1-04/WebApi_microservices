using Microsoft.AspNetCore.Mvc;
using WebApi_PartsService.Services;

namespace WebApi_PartsService.Controllers;

[ApiController]
[Route("api/PartsAndReplaces/continuous")]
public sealed class PartsGrabberQueueController(IConfiguration config):ControllerBase
{
    private readonly PartsGrabberQueue queue=new(config.GetConnectionString("KameraDb")??throw new InvalidOperationException("Missing KameraDb connection."));
    [HttpPost("claim")]
    public async Task<IActionResult> Claim(PartsGrabberRequest request)
    {
        if(!ValidOwner(request))return BadRequest("Invalid owner.");
        return Ok(await queue.ClaimAsync(request));
    }
    [HttpPost("{id:int}/{operation}")]
    public async Task<IActionResult> Execute(int id,string operation,PartsGrabberRequest request)
    {
        if(id<=0||!ValidOwner(request)||request.AttemptCount<=0||operation is not("renew" or "fail" or "complete" or "name" or "replace" or "picture"))
            return BadRequest("Invalid PartsGrabber lease.");
        try{return await queue.ExecuteAsync(id,operation,request)?NoContent():Conflict("Part lease expired or ownership changed.");}
        catch(ArgumentException ex){return BadRequest(ex.Message);}
    }
    private static bool ValidOwner(PartsGrabberRequest request)=>!string.IsNullOrWhiteSpace(request.WorkerId)&&request.WorkerId.Length<=128;
}
