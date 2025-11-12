using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_MaskService.Services;

namespace WebApi_MaskService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaskController : BaseControllerClass
    {

        public MaskController(IDatabaseService databaseService) : base(databaseService)
        {
        }


        [HttpGet("getmask/{jobId}")]
        public async Task<ActionResult<Job>> GetAllMasks(int jobId)
        {
            var job = await _databaseService.GetAllMasksAsync(jobId);
            return Ok(job);
        }

    }
}
