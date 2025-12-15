using Microsoft.AspNetCore.Mvc;
using WebApi_JobService.Services;

namespace WebApi_JobService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IDatabaseService _service;

        public TestController(IDatabaseService service) => _service = service;

        [HttpPost("job/{id}")]
        public async Task<IActionResult> Create(int id)
        {
            var job = await _service.AddJobByTgAsync(id);
            return Ok(job);
        }
    }

}
