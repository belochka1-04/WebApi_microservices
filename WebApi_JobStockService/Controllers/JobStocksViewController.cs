using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_JobStockService.Services;

namespace WebApi_JobStockService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobStocksViewController : BaseControllerClass
    {

        public JobStocksViewController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        [HttpGet("getJobStocks/{jobId}")]
        public async Task<ActionResult<IEnumerable<JobStocksView>>> GetJobStocksForId(int jobId)
        {
            var jobs = await _databaseService.GetJobStocks(jobId);
            return Ok(jobs);
        }

    }
}
