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
    public class ModelController : BaseControllerClass
    {

        public ModelController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        /// <summary>
        /// Получить модель по ID с опциональными Include
        /// GET /api/Models/{id}?includeBrandModel=true&includeSite=true
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ModelTb), StatusCodes.Status200OK)]  // ← ModelTb
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ModelTb>> GetById(
            int id,
            [FromQuery] bool includeBrandModel = false,
            [FromQuery] bool includeSite = false)
        {
            try
            {
                var model = await _databaseService.GetModelByIdAsync(
                    id, includeBrandModel, includeSite);

                if (model == null)
                {
                    return NotFound($"Модель с ID {id} не найдена");
                }

                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка получения модели: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить модели по списку ID с опциональными Include
        /// GET /api/Models/by-ids?ids=1,2,3&includeBrandModel=true
        /// </summary>
        [HttpGet("by-ids")]
        [ProducesResponseType(typeof(List<ModelTb>), StatusCodes.Status200OK)]  // ← ModelTb
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ModelTb>>> GetByIds(
            [FromQuery] int[] ids,
            [FromQuery] bool includeBrandModel = false,
            [FromQuery] bool includeSite = false)
        {
            try
            {
                if (ids == null || ids.Length == 0)
                {
                    return BadRequest("Список ID не может быть пустым");
                }


                var models = await _databaseService.GetModelsByIdsAsync(
                    ids, includeBrandModel, includeSite);


                return Ok(models);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка получения моделей: {ex.Message}");
            }
        }

        /// <summary>
        /// Обновить состояние ссылки модели
        /// PUT /api/Models/{id}/link-state
        /// </summary>
        [HttpPut("{id}/link-state")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateLinkState(
            int id,
            [FromBody] UpdateLinkStateRequest request)
        {
            try
            {

                await _databaseService.UpdateModelLinkStateAsync(id, request.LinkState);



                return Ok(new { message = "LinkState успешно обновлен", id, linkState = request.LinkState });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка обновления LinkState: {ex.Message}");
            }
        }
	

	 /// <summary>
        /// Получить модель по ключу (SiteId + BrandModelId + CleanedModel).
        /// GET /api/Model/by-key?siteId=1&brandModelId=2&cleanedModel=xxx
        /// </summary>
        [HttpGet("by-key")]
        [ProducesResponseType(typeof(ModelTb), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ModelTb>> GetByKey(
            [FromQuery] int siteId,
            [FromQuery] int brandModelId,
            [FromQuery] string cleanedModel,
            [FromQuery] bool includeBrandModel = false,
            [FromQuery] bool includeSite = false)
        {
            if (string.IsNullOrWhiteSpace(cleanedModel))
                return BadRequest("cleanedModel is required.");

            try
            {
                var model = await _databaseService.GetModelBySiteAndKeyAsync(
                    siteId, brandModelId, cleanedModel, includeBrandModel, includeSite);

                if (model == null)
                    return NotFound($"Модель с ключом SiteId={siteId}, BrandModelId={brandModelId}, CleanedModel={cleanedModel} не найдена");

                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка получения модели по ключу: {ex.Message}");
            }
        }

        /// <summary>
        /// Создать новую модель.
        /// POST /api/Model
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> Create([FromBody] ModelTb model)
        {
            if (model == null)
                return BadRequest("Model is required.");

            try
            {
                var id = await _databaseService.InsertModelAsync(model);
                return CreatedAtAction(nameof(GetById), new { id }, id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка создания модели: {ex.Message}");
            }
        }

        /// <summary>
        /// Обновить ссылку модели с записью в историю.
        /// PUT /api/Model/{id}/link
        /// </summary>
        [HttpPut("{id}/link")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateLink(
            int id,
            [FromBody] UpdateModelLinkRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.NewLink))
                return BadRequest("NewLink is required.");

            try
            {
                await _databaseService.UpdateModelLinkAsync(id, request.NewLink, request.Source ?? "GoogleModelUpdater");
                return Ok(new { message = "Ссылка модели успешно обновлена", id, request.NewLink });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка обновления ссылки модели: {ex.Message}");
            }
        }
    }
}




