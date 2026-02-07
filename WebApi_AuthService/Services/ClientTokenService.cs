using KameraData.Data.Models;
using Microsoft.IdentityModel.Tokens;
using SharedMicroserviceLibrary.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;

namespace WebApi_AuthService.Services
{
   
    public class ClientTokenService : IClientTokenService
    {
        private readonly IDatabaseService _databaseService;
        private readonly JwtSettings _jwtSettings;
        private readonly NLog.ILogger _logger;

        public ClientTokenService(
            IDatabaseService databaseService,
            IOptions<JwtSettings> jwtOptions)
        {
            _databaseService = databaseService;
            _jwtSettings = jwtOptions.Value;
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public async Task<string?> GenerateTokenAsync(string clientId, string clientSecret, string? scope)
        {
            var client = await _databaseService.GetApiClientByIdAsync(clientId);
            if (client == null)
            {
                _logger.Warn("Auth failed: client not found or inactive. clientId={clientId}", clientId);
                return null;
            }

            // проверяем секрет (в БД хранится хеш)
            if (!BCrypt.Net.BCrypt.Verify(clientSecret, client.ClientSecretHash))
            {
                _logger.Warn("Auth failed: invalid secret for clientId={clientId}", clientId);
                return null;
            }

            // формируем список scope
            var allowedScopes = (client.AllowedScopes ?? string.Empty)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var requestedScopes = (scope ?? string.Empty)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var finalScopes = requestedScopes.Any()
                ? requestedScopes.Where(s => allowedScopes.Contains(s)).ToArray()
                : allowedScopes.ToArray();

            // claims
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, client.ClientId),
                new("client_id", client.ClientId),
                new("client_name", client.Name),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            foreach (var s in finalScopes)
                claims.Add(new Claim("scope", s));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
