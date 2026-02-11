using KameraData.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi_GoogleSearchTemplatesService.Services;

namespace WebApi_GoogleSearchTemplatesService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GoogleModelRequestsController : BaseControllerClass
    {
        public GoogleModelRequestsController(IDatabaseService databaseService)
            : base(databaseService)
        {
        }

        /// <summary>
        /// Получить pending-запросы к Google (Status = 0).
        /// GET /api/GoogleModelRequests/pending?batchSize=100
        /// </summary>
        [HttpGet("pending")]
        [ProducesResponseType(typeof(List<GoogleModelRequest>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<GoogleModelRequest>>> GetPending([FromQuery] int batchSize = 100)
        {
            if (batchSize <= 0) batchSize = 100;

            try
            {
                var items = await _databaseService.GetPendingGoogleRequestsAsync(batchSize);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка получения pending-запросов: {ex.Message}");
            }
        }

        /// <summary>
        /// Создать новый запрос к Google.
        /// POST /api/GoogleModelRequests
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(GoogleModelRequest), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GoogleModelRequest>> Create([FromBody] string request)
        {
            if (string.IsNullOrWhiteSpace(request))
                return BadRequest("Request is required.");

            try
            {
                var entity = await _databaseService.CreateGoogleModelRequestAsync(request);
                return CreatedAtAction(nameof(GetPending), new { id = entity.Id }, entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка создания запроса: {ex.Message}");
            }
        }

        /// <summary>
        /// Пометить запрос обработанным (Status = 1).
        /// POST /api/GoogleModelRequests/{id}/processed
        /// </summary>
        [HttpPost("{id:int}/processed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MarkProcessed(int id)
        {
            try
            {
                await _databaseService.MarkGoogleModelRequestProcessedAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка пометки запроса обработанным: {ex.Message}");
            }
        }
    }
}
