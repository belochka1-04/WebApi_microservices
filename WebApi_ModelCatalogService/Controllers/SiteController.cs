using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_ModelCatalogService.Services;

namespace WebApi_ModelCatalogService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SiteController : BaseControllerClass
    {

        public SiteController(IDatabaseService databaseService) : base(databaseService)
        {
        }

		[HttpGet("{id}")]
		[ProducesResponseType(typeof(Site), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<Site>> GetById(int id)
		{
			try
			{
				
				var site = await _databaseService.GetSiteByIdAsync(id);

				if (site == null)
				{
					return NotFound($"Сайт с ID {id} не найден");
				}

				return Ok(site);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Ошибка получения сайта: {ex.Message}");
			}
		}

		/// <summary>
		/// Получить сайты по списку ID
		/// GET /api/Site/by-ids?ids=1,2,3
		/// </summary>
		[HttpGet("by-ids")]
		[ProducesResponseType(typeof(List<Site>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<Site>>> GetByIds([FromQuery] int[] ids)
		{
			try
			{
				if (ids == null || ids.Length == 0)
				{
					return BadRequest("Список ID не может быть пустым");
				}

				var sites = await _databaseService.GetSitesByIdsAsync(ids);

				return Ok(sites);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Ошибка получения сайтов: {ex.Message}");
			}
		}

		/// <summary>
		/// Получить все сайты (отсортированы по Confidence по возрастанию)
		/// GET /api/Site
		/// </summary>
		[HttpGet]
		[ProducesResponseType(typeof(List<Site>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<Site>>> GetAll()
		{
			try
			{
				
				var sites = await _databaseService.GetAllSitesAsync();

				return Ok(sites);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Ошибка получения списка сайтов: {ex.Message}");
			}
		}

		/// <summary>
		/// Получить сайты с фильтром по Confidence
		/// GET /api/Site/by-confidence?confidence=A
		/// </summary>
		/// <param name="confidence">Значение Confidence (например: A, B, C)</param>
		[HttpGet("by-confidence")]
		[ProducesResponseType(typeof(List<Site>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<Site>>> GetByConfidence([FromQuery] string confidence)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(confidence))
				{
					return BadRequest("Параметр Confidence не может быть пустым");
				}

				var sites = await _databaseService.GetSitesByConfidenceAsync(confidence);

				
				return Ok(sites);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Ошибка получения сайтов: {ex.Message}");
			}
		}

		/// <summary>
		/// Поиск сайтов по части названия
		/// GET /api/Site/search?query=manual
		/// </summary>
		[HttpGet("search")]
		[ProducesResponseType(typeof(List<Site>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<Site>>> SearchByTitle([FromQuery] string query)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(query))
				{
					return BadRequest("Строка поиска не может быть пустой");
				}

				
				var sites = await _databaseService.SearchSitesByTitleAsync(query);
				return Ok(sites);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Ошибка поиска сайтов: {ex.Message}");
			}
		}
	}

}




