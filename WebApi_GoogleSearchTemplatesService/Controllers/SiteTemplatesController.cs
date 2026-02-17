using KameraData.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi_GoogleSearchTemplatesService.Services;

namespace WebApi_GoogleSearchTemplatesService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SiteTemplatesController : BaseControllerClass
    {
        public SiteTemplatesController(IDatabaseService databaseService)
            : base(databaseService)
        {
        }

        /// <summary>
        /// Получить активные шаблоны сайтов.
        /// GET /api/SiteTemplates/active
        /// </summary>
        [HttpGet("active")]
        [ProducesResponseType(typeof(List<SiteTemplate>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<SiteTemplate>>> GetActive()
        {
            try
            {
                var templates = await _databaseService.GetActiveSiteTemplatesAsync();
                return Ok(templates);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка получения активных шаблонов: {ex.Message}");
            }
        }

        /// <summary>
        /// Активные шаблоны + активные title rules (отсортированы по Priority).
        /// GET /api/SiteTemplates/active-with-title-rules
        /// </summary>
        [HttpGet("active-with-title-rules")]
        [ProducesResponseType(typeof(List<SiteTemplate>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<SiteTemplate>>> GetActiveWithTitleRules()
        {
            var templates = await _databaseService.GetActiveSiteTemplatesWithTitleRulesAsync();
            return Ok(templates);
        }

        /// <summary>
        /// Получить активные title rules для конкретного шаблона.
        /// GET /api/SiteTemplates/{id}/title-rules
        /// </summary>
        [HttpGet("{id:int}/title-rules")]
        [ProducesResponseType(typeof(List<SiteTemplateTitleRule>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<SiteTemplateTitleRule>>> GetTitleRules(int id)
        {
            var rules = await _databaseService.GetActiveTitleRulesAsync(id);
            return Ok(rules);
        }
    

        /// <summary>
        /// Получить все шаблоны сайтов.
        /// GET /api/SiteTemplates
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<SiteTemplate>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<SiteTemplate>>> GetAll()
        {
            try
            {
                var templates = await _databaseService.GetAllSiteTemplatesAsync();
                return Ok(templates);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка получения шаблонов: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить шаблон сайта по Id.
        /// GET /api/SiteTemplates/{id}
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(SiteTemplate), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SiteTemplate>> GetById(int id)
        {
            try
            {
                var template = await _databaseService.GetSiteTemplateByIdAsync(id);
                if (template == null)
                    return NotFound();

                return Ok(template);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка получения шаблона: {ex.Message}");
            }
        }

        /// <summary>
        /// Создать новый шаблон сайта.
        /// POST /api/SiteTemplates
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> Create([FromBody] SiteTemplate template)
        {
            if (template == null)
                return BadRequest("Template is required.");

            try
            {
                var id = await _databaseService.CreateSiteTemplateAsync(template);
                return CreatedAtAction(nameof(GetById), new { id }, id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка создания шаблона: {ex.Message}");
            }
        }

        /// <summary>
        /// Обновить существующий шаблон сайта.
        /// PUT /api/SiteTemplates/{id}
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] SiteTemplate template)
        {
            if (template == null || template.Id != id)
                return BadRequest("Неверный Id шаблона.");

            try
            {
                await _databaseService.UpdateSiteTemplateAsync(template);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка обновления шаблона: {ex.Message}");
            }
        }
    }
}
