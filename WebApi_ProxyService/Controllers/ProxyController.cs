using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_ProxyService.Services;

namespace WebApi_ProxyService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProxyController : BaseControllerClass
    {

        public ProxyController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        [HttpGet("get")]
        public async Task<ActionResult<Proxy>> GetProxyAsync()
        {
            var result = await _databaseService.GetProxyAsync();
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] Proxy data)
        {
            if (data == null || data.Id <= 0)
            {
                return BadRequest("Некорректные данные proxy");
            }

            try
            {
                await _databaseService.UpdateProxyAsync(data);
                return Ok("Proxy успешно обновлен");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при обновлении Proxy: {ex.Message}");
            }
        }

    }
}
