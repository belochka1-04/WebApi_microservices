using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_ModelNumberService.Services;

namespace WebApi_ModelNumberService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModelNumberController : BaseControllerClass
    {

        public ModelNumberController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        // Метод для вставки номера модели
        [HttpPost("insert")]
        public async Task<IActionResult> InsertModelNumber([FromBody] KameraData.Data.Models.ModelNumberRequest request)
        {
            if (request == null || request.Model == null)
            {
                return BadRequest("Invalid request.");
            }

            await _databaseService.InsertModelNumberAsync(request.JobId, request.Model, request.SearchText, request.Status, request.Count);
            return NoContent(); // Возвращает 204 No Content
        }

        [HttpPost("insertBatch")]
        public async Task<IActionResult> InsertModelNumberBatch([FromBody] List<KameraData.Data.Models.ModelNumberRequest> requests)
        {
            if (requests == null || requests.Count == 0)
            {
                return BadRequest("Список запросов пуст.");
            }

            try
            {
                await _databaseService.InsertModelNumberBatchAsync(requests);


                // Для примера просто возвращаем OK
                return Ok($"Успешно обработано {requests.Count} запросов.");
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        // Метод для удаления номеров моделей по jobId
        [HttpDelete("delete/{jobId}")]
        public async Task<IActionResult> DeleteModelNumbers(int jobId)
        {
            await _databaseService.DeleteModelNumbersAsync(jobId);
            return NoContent(); // Возвращает 204 No Content
        }

        // Метод для обновления статуса других номеров моделей
        [HttpPut("update-status/{jobId}")]
        public async Task<IActionResult> UpdateModelNumbersStatus(int jobId, [FromBody] int status)
        {
            await _databaseService.UpdateOtherModelNumbersStatusAsync(jobId, status);
            return NoContent(); // Возвращает 204 No Content
        }

        [HttpPut("update-status-byId/{taskId}")]
        public async Task<IActionResult> UpdateModelNumbersStatusById(int taskId, [FromBody] int status)
        {
            await _databaseService.UpdateTaskStatusAsync(taskId, status);
            return NoContent(); // Возвращает 204 No Content
        }



        [HttpGet("get/{confirm}")]
        public async Task<ActionResult<List<ModelNumber>>> GetModelNambersWithConf(string confirm)
        {
            var result = await _databaseService.GetNextTasksWithConfAsync(confirm);
            return Ok(result);
        }

        [HttpGet("getfirst/{confirm}")]
        public async Task<ActionResult<ModelNumber>> GetModelNamberWithConf(string confirm)
        {
            var result = await _databaseService.GetNextTaskWithConfAsync(confirm);
            return Ok(result);
        }

        [HttpGet("pick-next-by-confirmed/{confirm}")]
        public async Task<ActionResult<ModelNumber>> PickNextTask(string confirm)
        {
            var result = await _databaseService.PickNextTaskAsync(confirm);
            return Ok(result);
        }

        [HttpGet("claim-next-by-confirmed/{confirm}")]
        public async Task<ActionResult<ModelNumber>> ClaimNextTask(
            string confirm,
            [FromQuery] string workerId,
            [FromQuery] int leaseSeconds = 900)
        {
            if (string.IsNullOrWhiteSpace(workerId))
                workerId = Environment.MachineName;

            var result = await _databaseService.ClaimNextTaskAsync(confirm, workerId, leaseSeconds);
            return Ok(result);
        }

        [HttpGet("getwaste")]
        public async Task<ActionResult<List<ModelNumber>>> GetWasteTasksAsync()
        {
            var result = await _databaseService.GetWasteTasksAsync();

            // Если нет данных, возвращаем статус 204
            if (result == null || !result.Any()|| result.Count==0)
            {
                return NoContent(); // Возвращаем 204 No Content
            }
            return Ok(result);
        }

        // GET api/modelnumber/by-job/123
        [HttpGet("by-job/{jobId:int}")]
        public async Task<ActionResult<List<ModelNumber>>> GetByJob(int jobId)
        {
            var result = await _databaseService.GetModelNumbersByJobAsync(jobId);
            return Ok(result);
        }

        // GET api/modelnumber/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ModelNumber>> GetById(int id)
        {
            var model = await _databaseService.GetModelNumberByIdAsync(id);
            if (model == null) return NotFound();
            return Ok(model);
        }

        // GET api/modelnumber/notfound/by-job/123
        [HttpGet("notfound/by-job/{jobId:int}")]
        public async Task<ActionResult<List<ModelsNumbersNotFound>>> GetNotFoundByJob(int jobId)
        {
            var result = await _databaseService.GetModelNumbersNotFoundByJobAsync(jobId);
            return Ok(result);
        }
    }


}

