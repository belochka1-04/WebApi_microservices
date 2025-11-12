using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_ResponsesService.Services;

namespace WebApi_ResponsesService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResponsesController : BaseControllerClass
    {

        public ResponsesController(IDatabaseService databaseService) : base(databaseService)
        {
        }


        // Метод для вставки ответа
        [HttpPost("insert")]
        public async Task<IActionResult> InsertResponse([FromBody] Response request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request.");
            }

            await _databaseService.InsertResponceAsync(request);
            return NoContent(); // Возвращает 204 No Content
        }

        [HttpGet("getByJobId/{jobId}")]
        public async Task<ActionResult<List<Response>>> GetByJobId(int jobId)
        {
            var result = await _databaseService.GetResponceByJobIdAsync(jobId);
            return Ok(result);
        }


    }
}
