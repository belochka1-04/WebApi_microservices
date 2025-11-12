using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_document.Services;

namespace WebApi_document.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentPdfTextController : BaseControllerClass
    {
        
        public DocumentPdfTextController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        [HttpGet("get/{docId}")]
        public async Task<ActionResult<DocumentPdfText>> GetDocById(int docId)
        {
            var result = await _databaseService.GetDocPdfByIdAsync(docId);
            return Ok(result);
        }

        
    }
}
