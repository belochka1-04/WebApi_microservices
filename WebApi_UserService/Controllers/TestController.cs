using Microsoft.AspNetCore.Mvc;
using WebApi_UserService.Services;

namespace WebApi_UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IDatabaseService _userService;

        public TestController(IDatabaseService userService) => _userService = userService;

        [HttpGet("user/{telegramId}")]
        public async Task<IActionResult> GetUser(int telegramId)
        {
            var user = await _userService.GetUserByTgAsync(telegramId);
            return Ok(user);
        }
    }

}
