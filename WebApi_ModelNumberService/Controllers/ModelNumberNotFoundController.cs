using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_ModelNumberService.Services;

namespace WebApi_ModelNumberService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModelNumberNotFoundController : BaseControllerClass
    {

        public ModelNumberNotFoundController(IDatabaseService databaseService) : base(databaseService)
        {
        }


        // Метод для вставки номера модели, который не найден
        [HttpPost("insert")]
        public async Task<IActionResult> InsertModelNumberNotFound([FromBody] ModelNumberNotFoundRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.ModelNumber))
            {
                return BadRequest("Invalid request.");
            }

            await _databaseService.InsertModelNumberNotFoundAsync(request.JobId, request.ModelNumber);
            return NoContent(); // Возвращает 204 No Content
        }

        // Метод для удаления номеров моделей, которые не были найдены, по jobId
        [HttpDelete("delete/{jobId}")]
        public async Task<IActionResult> DeleteModelNumbersNotFound(int jobId)
        {
            await _databaseService.DeleteModelNumbersNotFoundAsync(jobId);
            return NoContent(); // Возвращает 204 No Content
        }
    }


}

