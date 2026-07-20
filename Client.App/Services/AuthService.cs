using Client.App.DTO;
using Microsoft.AspNetCore.Components;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace Client.App.Services;

public class AuthService
{
    private readonly AccessTokenService _accessTokenService; 
    private readonly ILogger<AuthService> _logger;
    private HttpClient _httpClient;

    private readonly string _tokenEndpoint;

    public AuthService(
        AccessTokenService accessTokenService,
        ILogger<AuthService> logger,
        IConfiguration config,
        IHttpClientFactory httpClientFactory)
    {
        _accessTokenService = accessTokenService;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _tokenEndpoint = config["AuthApi:TokenEndpoint"] ?? "/api/Auth/login";
    }

    public async Task<string> LoginAsync(string username, string password)
    {
        var payload = new { username, password };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_tokenEndpoint, content);
        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger?.LogWarning("AuthApi login failed: {Status} {Body}", response.StatusCode, body);
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "AuthApi login failed and reading body threw");
            }
            return null;
        }

        using var responseStream = await response.Content.ReadAsStreamAsync();
        var doc = await JsonDocument.ParseAsync(responseStream);

        foreach (var prop in doc.RootElement.EnumerateObject())
        {
            if (string.Equals(prop.Name, "access_token", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(prop.Name, "token", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(prop.Name, "Token", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(prop.Name, "accessToken", StringComparison.OrdinalIgnoreCase))
            {
                if (prop.Value.ValueKind == JsonValueKind.String)
                {
                    await _accessTokenService.SetAccessTokenAsync(prop.Value.GetString());
                    return prop.Value.GetString();
                }

                if (prop.Value.ValueKind == JsonValueKind.Object)
                {
                    foreach (var inner in prop.Value.EnumerateObject())
                    {
                        if (inner.Value.ValueKind == JsonValueKind.String)
                        {
                            await _accessTokenService.SetAccessTokenAsync(inner.Value.GetString());
                            return inner.Value.GetString();
                        }
                    }
                }
            }
        }

        return null;
    }

    public async Task<(bool Success, string? Error)> RegisterAsync(string username, string email, string password)
    {
        var payload = new { username, email, password };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var resp = await _httpClient.PostAsync("/api/auth/register", content);

        if (resp.IsSuccessStatusCode)
        {
            return (true, null);
        }

        var body = await resp.Content.ReadAsStringAsync();
        return (false, string.IsNullOrWhiteSpace(body) ? resp.ReasonPhrase : body);
    }

    public async Task<(bool Success, string? Error)> LogOut()
    {
        var response = _httpClient.PostAsync("/api/auth/logout", null);

        if (response.IsCompletedSuccessfully)
        {
            return (true, null);
        }

        var body = await response.Result.Content.ReadAsStringAsync();
        return (false, string.IsNullOrWhiteSpace(body) ? response.IsFaulted.ToString() : body);
    }
}
