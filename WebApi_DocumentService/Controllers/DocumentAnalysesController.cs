// Controllers/DocumentAnalysesController.cs
using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_document.Services;

namespace WebApi_document.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentAnalysesController : BaseControllerClass
    {
        public DocumentAnalysesController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentAnalysis>> GetById(int id, [FromQuery] string? include = null)
        {
            DocumentAnalysis? analysis;

            if (include?.Contains("genericQas") == true || include?.Contains("qas") == true)
            {
                analysis = await _databaseService.GetDocumentAnalysisWithIncludesAsync(id);
            }
            else
            {
                analysis = await _databaseService.GetDocumentAnalysisByIdAsync(id);
            }

            if (analysis == null) return NotFound();
            return Ok(analysis);
        }

        [HttpGet("by-document/{documentId}")]
        public async Task<ActionResult<DocumentAnalysis>> GetByDocument(int documentId, [FromQuery] string? include = null)
        {
            var analysis = await _databaseService.GetDocumentAnalysisByDocumentIdAsync(documentId);

            if (analysis == null) return NotFound();

            // Если нужны includes, подгружаем отдельно или используем метод с Include
            if (include?.Contains("genericQas") == true || include?.Contains("qas") == true)
            {
                analysis = await _databaseService.GetDocumentAnalysisWithIncludesAsync(analysis.Id);
            }

            return Ok(analysis);
        }

        [HttpPost]
        public async Task<ActionResult<DocumentAnalysis>> Create([FromBody] CreateDocumentAnalysisDto dto)
        {
            var analysis = await _databaseService.CreateDocumentAnalysisAsync(dto.DocumentId);
            return CreatedAtAction(nameof(GetById), new { id = analysis.Id }, analysis);
        }

        [HttpGet("{id}/generic-qas")]
        public async Task<ActionResult<List<DocumentGenericQa>>> GetGenericQas(int id)
        {
            var qas = await _databaseService.GetDocumentGenericQasByAnalysisIdAsync(id);
            return Ok(qas);
        }

        [HttpGet("{id}/qas")]
        public async Task<ActionResult<List<DocumentQa>>> GetQas(int id)
        {
            var qas = await _databaseService.GetDocumentQasByAnalysisIdAsync(id);
            return Ok(qas);
        }
    }

    public class CreateDocumentAnalysisDto
    {
        public int DocumentId { get; set; }
    }
}
