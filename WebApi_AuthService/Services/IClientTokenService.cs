namespace WebApi_AuthService.Services
{
    public interface IClientTokenService
    {
        Task<string?> GenerateTokenAsync(string clientId, string clientSecret, string? scope);
    }
}
