using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_StopWordsService.Services;

namespace WebApi_StopWordsService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StopWordsController : BaseControllerClass
    {

        public StopWordsController(IDatabaseService databaseService) : base(databaseService)
        {
        }


        [HttpGet("get/{userId}")]
        public async Task<ActionResult<StopWords>> GetWordsByUserId(int userId)
        {
            var job = await _databaseService.GetStopWordsID(userId);
            return Ok(job);
        }

    }
}
