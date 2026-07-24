using Client.App.DTO.AuthDtos;
using Client.App.Security;
using Microsoft.AspNetCore.Components;
using System.Text;
using System.Text.Json;

namespace Client.App.Services;

public class AuthService
{
    private readonly CustomAuthenticationStateProvider _authenticationStateProvider;
    private readonly ILogger<AuthService> _logger;
    private readonly NavigationManager _navigationManager;
    private HttpClient _httpClient;

    private readonly string _tokenEndpoint;

    public AuthService(
        CustomAuthenticationStateProvider authenticationStateProvider,
        NavigationManager navigationManager,
        ILogger<AuthService> logger,
        IConfiguration config,
        IHttpClientFactory httpClientFactory)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _navigationManager = navigationManager;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _tokenEndpoint = config["AuthApi:TokenEndpoint"] ?? "/api/auth/login";
    }

    public async Task<(bool Success, string? Error)> LoginAsync(string username, string password)
    {
        var payload = new { username, password };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_tokenEndpoint, content);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return(false, error);
        }

        await _authenticationStateProvider.NotifyAuthenticationStateChangedAsync();

        return (true,null);
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

    //public async Task<bool> RefreshTokenAsync()
    //{
    //    var refreshToken = await _refreshTokenService.GetAsync();
    //    _httpClient.DefaultRequestHeaders.Add("Cookie", $"refreshtoken={refreshToken}");
    //    var payload = new { refreshToken };
    //    var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
    //    var response = await _httpClient.PostAsync("/api/auth/refresh", content);

    //    if (response.IsSuccessStatusCode)
    //    {
    //        var token = await response.Content.ReadAsStringAsync();
    //        if (!string.IsNullOrEmpty(token))
    //        {
    //            var result = JsonSerializer.Deserialize<AuthResponse>(token);
    //            await _accessTokenService.SetAccessTokenAsync(result.AccessToken);
    //            await _refreshTokenService.SetAsync(result.RefreshToken);

    //            return true;
    //        }
    //    }
    //    return false;
    //}

    public async Task LogoutAsync(string username)
    {
        var payload = new { username };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        await _httpClient.PostAsync("api/auth/logout", content);

        await _authenticationStateProvider.NotifyUserLoggedOut();
    }
}
