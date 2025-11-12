using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_document.Services;

namespace WebApi_document.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : BaseControllerClass
    {
        
        public DocumentController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        [HttpGet("get/{Id}")]
        public async Task<ActionResult<Document>> GetDocById(int Id)
        {
            var result = await _databaseService.GetDocByIdAsync(Id);
            return Ok(result);
        }

        
    }
}
