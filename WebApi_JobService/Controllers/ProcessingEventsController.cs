using KameraData.Data.Other;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi_JobService.Services;

namespace WebApi_JobService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProcessingEventsController : BaseControllerClass
    {
        public ProcessingEventsController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        [HttpPost]
        public async Task<IActionResult> InsertEvent([FromBody] JobProcessingEventRequest request)
        {
            if (request == null)
                return BadRequest("Request is required.");

            if (string.IsNullOrWhiteSpace(request.Component))
                return BadRequest("Component is required.");

            if (string.IsNullOrWhiteSpace(request.Stage))
                return BadRequest("Stage is required.");

            if (string.IsNullOrWhiteSpace(request.Status))
                return BadRequest("Status is required.");

            await _databaseService.InsertJobProcessingEventAsync(request);
            return NoContent();
        }

        [HttpGet("job/{jobId:int}")]
        public async Task<ActionResult<IEnumerable<JobProcessingEventRequest>>> GetEventsForJob(int jobId)
        {
            var events = await _databaseService.GetJobProcessingEventsAsync(jobId);
            return Ok(events);
        }
    }
}
