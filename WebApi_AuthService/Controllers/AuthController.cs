using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApi_AuthService.Services;

namespace WebApi_AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseControllerClass
    {
        private readonly IClientTokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IDatabaseService databaseService,
            IClientTokenService tokenService,
            ILogger<AuthController> logger)
            : base(databaseService)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        public class ClientCredentialsRequest
        {
            public string grant_type { get; set; } = "client_credentials";
            public string client_id { get; set; } = null!;
            public string client_secret { get; set; } = null!;
            public string? scope { get; set; }
        }

        [HttpPost("token")]
        [EnableRateLimiting("token")]
        public async Task<IActionResult> Token([FromForm] ClientCredentialsRequest request)
        {
            if (!string.Equals(request.grant_type, "client_credentials", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { error = "unsupported_grant_type" });

            var token = await _tokenService.GenerateTokenAsync(
                request.client_id,
                request.client_secret,
                request.scope);

            if (token == null)
                return Unauthorized(new { error = "invalid_client" });

            _logger.LogInformation("Client {ClientId} obtained token", request.client_id);

            return Ok(new
            {
                access_token = token,
                token_type = "Bearer",
                expires_in = 60 * 60 // секунд, если ExpirationMinutes = 60
            });
        }
    }
}
