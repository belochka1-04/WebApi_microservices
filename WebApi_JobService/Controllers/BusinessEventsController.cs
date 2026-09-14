using KameraData.Data.Other;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi_JobService.Services;

namespace WebApi_JobService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BusinessEventsController : BaseControllerClass
    {
        public BusinessEventsController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        [HttpPost]
        public async Task<IActionResult> InsertEvent([FromBody] BusinessEventRequest request)
        {
            if (request == null)
                return BadRequest("Request is required.");

            if (string.IsNullOrWhiteSpace(request.EventName))
                return BadRequest("EventName is required.");

            if (string.IsNullOrWhiteSpace(request.EventCategory))
                return BadRequest("EventCategory is required.");

            await _databaseService.InsertBusinessEventAsync(request);
            return NoContent();
        }

        [HttpGet("job/{jobId:int}")]
        public async Task<ActionResult<IEnumerable<BusinessEventRequest>>> GetEventsForJob(int jobId)
        {
            var events = await _databaseService.GetBusinessEventsForJobAsync(jobId);
            return Ok(events);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<ActionResult<IEnumerable<BusinessEventRequest>>> GetEventsForUser(int userId, [FromQuery] int take = 100)
        {
            var events = await _databaseService.GetBusinessEventsForUserAsync(userId, take);
            return Ok(events);
        }
    }
}
