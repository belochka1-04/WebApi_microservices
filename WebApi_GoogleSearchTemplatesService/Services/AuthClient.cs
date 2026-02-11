using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace WebApi_GoogleSearchTemplatesService.Services;

public class AuthClientOptions
{
    public string BaseUrl { get; set; } = null!;
    public string TokenEndpoint { get; set; } = "/api/auth/token";
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string Scope { get; set; } = "user.read";
}

public interface IAuthTokenProvider
{
    Task<string> GetTokenAsync(CancellationToken cancellationToken = default);
}

public class AuthTokenProvider : IAuthTokenProvider
{
    private readonly HttpClient _httpClient;
    private readonly AuthClientOptions _options;
    private string? _cachedToken;
    private DateTime _expiresAt = DateTime.MinValue;

    public AuthTokenProvider(HttpClient httpClient, IOptions<AuthClientOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
    }

    public async Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        if (_cachedToken != null && DateTime.UtcNow < _expiresAt.AddSeconds(-30))
            return _cachedToken;

        var pairs = new List<KeyValuePair<string, string>>
        {
            new("grant_type", "client_credentials"),
            new("client_id", _options.ClientId),
            new("client_secret", _options.ClientSecret),
            new("scope", _options.Scope),
        };

        using var content = new FormUrlEncodedContent(pairs);
        var response = await _httpClient.PostAsync(_options.TokenEndpoint, content, cancellationToken);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        _cachedToken = doc.RootElement.GetProperty("access_token").GetString()!;
        var expiresIn = doc.RootElement.GetProperty("expires_in").GetInt32();
        _expiresAt = DateTime.UtcNow.AddSeconds(expiresIn);

        return _cachedToken;
    }
}
