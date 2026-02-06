using KameraData.Data.Dtos;
using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_ModelCatalogService.Services;

namespace WebApi_ModelCatalogService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController : BaseControllerClass
    {

        public BrandController(IDatabaseService databaseService) : base(databaseService)
        {
        }


		/// <summary>
		/// Получить бренд по ID
		/// GET /api/Brand/{id}
		/// </summary>
		/// <param name="id">ID бренда</param>
		/// <returns>Объект Brand</returns>
		[HttpGet("{id}")]
		[ProducesResponseType(typeof(Brand), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<Brand>> GetById(int id)
		{
			try
			{
				var brand = await _databaseService.GetBrandByIdAsync(id);

				if (brand == null)
				{
					return NotFound($"Бренд с ID {id} не найден");
				}

				return Ok(brand);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Ошибка получения бренда: {ex.Message}");
			}
		}

		/// <summary>
		/// Получить бренды по списку ID
		/// GET /api/Brand/by-ids?ids=1,2,3
		/// </summary>
		/// <param name="ids">Массив ID брендов</param>
		/// <returns>Список объектов Brand</returns>
		[HttpGet("by-ids")]
		[ProducesResponseType(typeof(List<Brand>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<Brand>>> GetByIds([FromQuery] int[] ids)
		{
			try
			{
				if (ids == null || ids.Length == 0)
				{
					return BadRequest("Список ID не может быть пустым");
				}

				var brands = await _databaseService.GetBrandsByIdsAsync(ids);
				return Ok(brands);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Ошибка получения брендов: {ex.Message}");
			}
		}

		/// <summary>
		/// Получить все бренды (отсортированы по названию)
		/// GET /api/Brand
		/// </summary>
		/// <returns>Список всех объектов Brand</returns>
		[HttpGet]
		[ProducesResponseType(typeof(List<Brand>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<Brand>>> GetAll()
		{
			try
			{
				var brands = await _databaseService.GetAllBrandsAsync();

				return Ok(brands);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Ошибка получения списка брендов: {ex.Message}");
			}
		}

		/// <summary>
		/// Поиск брендов по части названия
		/// GET /api/Brand/search?query=samsung
		/// </summary>
		/// <param name="query">Строка поиска (ищется в Title)</param>
		/// <returns>Список найденных брендов</returns>
		[HttpGet("search")]
		[ProducesResponseType(typeof(List<Brand>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<Brand>>> SearchByTitle([FromQuery] string query)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(query))
				{
					return BadRequest("Строка поиска не может быть пустой");
				}

				var brands = await _databaseService.SearchBrandsByTitleAsync(query);

				return Ok(brands);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Ошибка поиска брендов: {ex.Message}");
			}
		}

		/// <summary>
		/// Получить бренд по точному названию (case-insensitive)
		/// GET /api/Brand/by-title?title=Samsung
		/// </summary>
		/// <param name="title">Точное название бренда</param>
		/// <returns>Объект Brand или 404</returns>
		[HttpGet("by-title")]
		[ProducesResponseType(typeof(Brand), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<Brand>> GetByTitle([FromQuery] string title)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(title))
				{
					return BadRequest("Название бренда не может быть пустым");
				}

				var brand = await _databaseService.GetBrandByTitleAsync(title);

				if (brand == null)
				{
					return NotFound($"Бренд с названием '{title}' не найден");
				}

				return Ok(brand);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Ошибка поиска бренда: {ex.Message}");
			}
		}

		/// <summary>
		/// Получить популярные бренды (топ N по количеству моделей)
		/// GET /api/Brand/popular?top=10
		/// </summary>
		/// <param name="top">Количество брендов (по умолчанию 10)</param>
		/// <returns>Список популярных брендов с количеством моделей</returns>
		[HttpGet("popular")]
		[ProducesResponseType(typeof(List<BrandWithCountDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<BrandWithCountDto>>> GetPopular([FromQuery] int top = 10)
		{
			try
			{
				if (top <= 0)
				{
					top = 10;
				}

				
				var brands = await _databaseService.GetPopularBrandsAsync(top);

				
				return Ok(brands);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Ошибка получения популярных брендов: {ex.Message}");
			}
		}

		/// <summary>
		/// Получить количество моделей для бренда
		/// GET /api/Brand/{id}/models-count
		/// </summary>
		/// <param name="id">ID бренда</param>
		/// <returns>Количество моделей</returns>
		[HttpGet("{id}/models-count")]
		[ProducesResponseType(typeof(BrandModelsCountDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<BrandModelsCountDto>> GetModelsCount(int id)
		{
			try
			{
				
				var count = await _databaseService.GetBrandModelsCountAsync(id);

				if (count < 0) // Бренд не найден
				{
					return NotFound($"Бренд с ID {id} не найден");
				}

				var result = new BrandModelsCountDto
				{
					BrandId = id,
					ModelsCount = count
				};

				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Ошибка получения количества моделей: {ex.Message}");
			}
		}
	}


}




