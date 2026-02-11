using KameraData.Data.Dtos;
using KameraData.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi_ModelCatalogService.Services;

namespace WebApi_ModelCatalogService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ModelLinkHistoryController : BaseControllerClass
    {

        public ModelLinkHistoryController(IDatabaseService databaseService) : base(databaseService)
        {
        }


        /// <summary>
        /// Получить историю изменения ссылок для модели.
        /// GET /api/ModelLinkHistory/model/{modelId}?top=50
        /// </summary>
        /// <param name="modelId">ID модели.</param>
        /// <param name="top">Максимальное количество записей (по умолчанию 50).</param>
        /// <returns>Список записей history для модели.</returns>
        [HttpGet("model/{modelId}")]
        [ProducesResponseType(typeof(List<ModelLinkHistory>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ModelLinkHistory>>> GetByModelId(
            int modelId,
            [FromQuery] int top = 50)
        {
            try
            {
                if (top <= 0) top = 50;

                var history = await _databaseService.GetModelLinkHistoryAsync(modelId, top);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка получения history для модели {modelId}: {ex.Message}");
            }
        }

        /// <summary>
        /// Добавить запись в историю изменения ссылок модели.
        /// POST /api/ModelLinkHistory
        /// </summary>
        /// <param name="request">Данные для записи history.</param>
        /// <returns>Созданная запись history.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ModelLinkHistory), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ModelLinkHistory>> Create(
            [FromBody] CreateModelLinkHistoryRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.OldLink))
                return BadRequest("OldLink is required.");

            try
            {
                var history = new ModelLinkHistory
                {
                    ModelId = request.ModelId,
                    OldLink = request.OldLink,
                    ChangedAt = DateTime.UtcNow,
                    Source = request.Source
                };

                await _databaseService.InsertModelLinkHistoryAsync(history);

                return CreatedAtAction(nameof(GetByModelId),
                    new { modelId = history.ModelId }, history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка создания записи history: {ex.Message}");
            }
        }
    }
}






