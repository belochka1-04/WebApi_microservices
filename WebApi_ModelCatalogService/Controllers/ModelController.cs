using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_ModelCatalogService.Services;

namespace WebApi_ModelCatalogService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
	}

	/// <summary>
	/// DTO для обновления LinkState
	/// </summary>
	public class UpdateLinkStateRequest
	{
		public int LinkState { get; set; }
	}
}




