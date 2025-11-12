using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_JobDocumentService.Services;

namespace WebApi_JobDocumentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobDocsModelInfoController : BaseControllerClass
    {

        public JobDocsModelInfoController(IDatabaseService databaseService) : base(databaseService)
        {
        }



        [HttpGet("get/{jobId}")]
        public async Task<ActionResult<Job>> GetJobDocModelInfos(int jobId)
        {
            var job = await _databaseService.GetJobDocModelInfos(jobId);
            return Ok(job);
        }

        

    }
}
