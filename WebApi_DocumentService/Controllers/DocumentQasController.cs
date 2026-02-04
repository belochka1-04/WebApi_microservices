// Controllers/DocumentQasController.cs
using KameraData.Data.Dtos;
using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_document.Services;

namespace WebApi_document.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentQasController : BaseControllerClass
    {
        public DocumentQasController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentQa>> GetById(int id)
        {
            var qa = await _databaseService.GetDocumentQaByIdAsync(id);
            if (qa == null) return NotFound();
            return Ok(qa);
        }

        [HttpPost]
        public async Task<ActionResult<DocumentQa>> Create([FromBody] CreateDocumentQaDto dto)
        {
            var qa = await _databaseService.CreateDocumentQaAsync(dto.AnalysisId, dto.Question);
            return CreatedAtAction(nameof(GetById), new { id = qa.Id }, qa);
        }

        [HttpGet("by-analysis/{analysisId}")]
        public async Task<ActionResult<List<DocumentQa>>> GetByAnalysis(int analysisId)
        {
            var qas = await _databaseService.GetDocumentQasByAnalysisIdAsync(analysisId);
            return Ok(qas);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] int status)
        {
            await _databaseService.UpdateDocumentQaStatusAsync(id, status);
            return NoContent();
        }
    }

   
}
