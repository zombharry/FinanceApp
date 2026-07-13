using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FinanceApp.Services;

public class AuthApiClient
{
    private readonly HttpClient _http;
    private readonly string _tokenEndpoint;
    private readonly ILogger<AuthApiClient> _logger;

    public AuthApiClient(IHttpClientFactory httpClientFactory, IConfiguration config, ILogger<AuthApiClient> logger)
    {
        _http = httpClientFactory.CreateClient("AuthApi");
        _tokenEndpoint = config["AuthApi:TokenEndpoint"] ?? "/api/Auth/login";
        _logger = logger;
    }

    public async Task<string> GetTokenAsync(string username, string password)
    {
        var payload = new { username, password };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _http.PostAsync(_tokenEndpoint, content);
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
        // Look for common token property names in a case-insensitive way.
        foreach (var prop in doc.RootElement.EnumerateObject())
        {
            if (string.Equals(prop.Name, "access_token", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(prop.Name, "token", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(prop.Name, "Token", StringComparison.OrdinalIgnoreCase))
            {
                if (prop.Value.ValueKind == JsonValueKind.String)
                    return prop.Value.GetString();

                if (prop.Value.ValueKind == JsonValueKind.Object)
                {
                    foreach (var inner in prop.Value.EnumerateObject())
                    {
                        if (inner.Value.ValueKind == JsonValueKind.String)
                            return inner.Value.GetString();
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
        var resp = await _http.PostAsync("/api/auth/register", content);

        if (resp.IsSuccessStatusCode)
        {
            return (true, null);
        }

        var body = await resp.Content.ReadAsStringAsync();
        return (false, string.IsNullOrWhiteSpace(body) ? resp.ReasonPhrase : body);
    }

    public async Task<(bool Success, string? Error)> LogOut()
    {
       var response = _http.PostAsync("/api/auth/logout", null);

        if (response.IsCompletedSuccessfully)
        {
            return (true, null);
        }

        var body = await response.Result.Content.ReadAsStringAsync();
        return (false, string.IsNullOrWhiteSpace(body) ? response.IsFaulted.ToString() : body);
    }
}
