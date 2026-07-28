using Client.App.DTO.AuthDtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Client.App.Services;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private HttpClient _httpClient;

    public AuthService(
        ILogger<AuthService> logger,
        IConfiguration config,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    public async Task<UserInfo?> GetUserInfoAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<UserInfo>("api/auth/me");
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "GetUserInfo: failed to get user informations");
            return null;
        }
    }

    public async Task<Guid> GetUserId(string username)
    {
        var response = await _httpClient.GetAsync($"api/auth/getUserId?username={username}");

        if (response.IsSuccessStatusCode)
        {
            try
            {
                var stream = await response.Content.ReadAsStreamAsync();
                using var doc = await JsonDocument.ParseAsync(stream);
                if (doc.RootElement.TryGetProperty("userId", out var idProp))
                {
                    if (idProp.ValueKind == JsonValueKind.String && Guid.TryParse(idProp.GetString(), out var guid))
                    {
                        return guid;
                    }
                    if (idProp.ValueKind == JsonValueKind.Null)
                    {
                        return Guid.Empty;
                    }
                }

                if (doc.RootElement.ValueKind == JsonValueKind.String && Guid.TryParse(doc.RootElement.GetString(), out var rootGuid))
                {
                    return rootGuid;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "GetUserId: failed to parse response");
            }
        }

        return Guid.Empty;
    }

    public async Task<(bool Success, string? Error)> RegisterAsync(string username, string email, string password)
    {
        var payload = new { username, email, password };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var resp = await _httpClient.PostAsync("/api/auth/register", content);

        if (!resp.IsSuccessStatusCode)
        {
            var error = await resp.Content.ReadAsStringAsync();
            return (false, error);
        }

        return (true, null);
    }

}
