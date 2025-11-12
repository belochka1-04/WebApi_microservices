using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_applications.Services;

namespace WebApi_applications.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationController : BaseControllerClass
    {
        public ApplicationController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        [HttpGet("get")]
        public async Task<ActionResult<List<Application>>> GetApplicationList()
        {
            var result = await _databaseService.GetApplicationList();
            if (result == null || result.Count() == 0)
            {
                return NoContent(); // Возвращаем 204 No Content
            }
            return Ok(result);
        }

        [HttpGet("getapp/{name}")]
        public async Task<ActionResult<Application>> GetApplicationName(string name)
        {
            var result = await _databaseService.GetApplicationName(name);
            if (result == null || result.Count()==0)
            {
                return NoContent(); // Возвращаем 204 No Content
            }
            return Ok(result.FirstOrDefault());
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateApplication(int id, [FromBody] Application app)
        {
            if (app == null)
            {
                return BadRequest("Invalid app.");
            }

            await _databaseService.UpdateApplication(app);
            return NoContent(); // Возвращает 204 No Content
        }
    } 
}
