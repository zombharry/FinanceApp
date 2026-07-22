using Client.App.DTO;
using Client.App.Security;
using Microsoft.AspNetCore.Components;
using System.Text;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace Client.App.Services;

public class AuthService
{
    private readonly AccessTokenService _accessTokenService; 
    private readonly RefreshTokenService _refreshTokenService;
    private readonly JwtAuthenticationStateProvider _authenticationStateProvider;
    private readonly ILogger<AuthService> _logger;
    private readonly NavigationManager _navigationManager;
    private HttpClient _httpClient;

    private readonly string _tokenEndpoint;

    public AuthService(
        AccessTokenService accessTokenService,
        RefreshTokenService refreshTokenService,
        JwtAuthenticationStateProvider authenticationStateProvider,
        NavigationManager navigationManager,
        ILogger<AuthService> logger,
        IConfiguration config,
        IHttpClientFactory httpClientFactory)
    {
        _accessTokenService = accessTokenService;
        _refreshTokenService = refreshTokenService;
        _authenticationStateProvider = authenticationStateProvider;
        _navigationManager = navigationManager;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _tokenEndpoint = config["AuthApi:TokenEndpoint"] ?? "/api/auth/login";
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
                _navigationManager.NavigateTo("/login");
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "AuthApi login failed and reading body threw");
                _navigationManager.NavigateTo("/login");
            }
            return null;
        }

        string accessToken = string.Empty;
        string refreshToken = string.Empty;

        using var responseStream = await response.Content.ReadAsStreamAsync();
        var doc = await JsonDocument.ParseAsync(responseStream);

        foreach (var prop in doc.RootElement.EnumerateObject())
        {
            if (string.Equals(prop.Name, "accessToken", StringComparison.OrdinalIgnoreCase))
            {
                if (prop.Value.ValueKind == JsonValueKind.String)
                    accessToken = prop.Value.GetString();

                if (prop.Value.ValueKind == JsonValueKind.Object)
                {
                    foreach (var inner in prop.Value.EnumerateObject())
                    {
                        if (inner.Value.ValueKind == JsonValueKind.String)
                            accessToken = inner.Value.GetString();
                    }
                }
            }
            if (string.Equals(prop.Name, "refreshToken", StringComparison.OrdinalIgnoreCase))
            {
                if (prop.Value.ValueKind == JsonValueKind.String)
                    refreshToken = prop.Value.GetString();

                if (prop.Value.ValueKind == JsonValueKind.Object)
                {
                    foreach (var inner in prop.Value.EnumerateObject())
                    {
                        if (inner.Value.ValueKind == JsonValueKind.String)
                            refreshToken = inner.Value.GetString();
                    }
                }
            }
        }
        //if (string.IsNullOrEmpty(await _accessTokenService.GetAccessTokenAsync()))
        //{
        //    await _accessTokenService.RemoveAccessTokenAsync();
        //}
        await _accessTokenService.SetAccessTokenAsync(accessToken);
        await _refreshTokenService.SetAsync(refreshToken);

        return accessToken;
        //return inner.Value.GetString();
    }

    public async Task<DTO.UserInfo?> GetUserInfoAsync()
    {
        var token = await _accessTokenService.GetAccessTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var userInfo = new DTO.UserInfo();
            // common claim types: name, unique_name, sub, email
            userInfo.UserId = Guid.Parse(jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
            userInfo.Username = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.GivenName || c.Type == "given_name" || c.Type == "name" )?.Value;
            userInfo.Email = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email || c.Type == "email")?.Value;

            foreach (var c in jwt.Claims)
            {
                if (!userInfo.Claims.ContainsKey(c.Type))
                    userInfo.Claims[c.Type] = c.Value;
            }

            return userInfo;
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "GetUserInfo: failed to read token");
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

        if (resp.IsSuccessStatusCode)
        {
            return (true, null);
        }

        var body = await resp.Content.ReadAsStringAsync();
        return (false, string.IsNullOrWhiteSpace(body) ? resp.ReasonPhrase : body);
    }

    public async Task<bool> RefreshTokenAsync()
    {
        var refreshToken = await _refreshTokenService.GetAsync();
        _httpClient.DefaultRequestHeaders.Add("Cookie", $"refreshtopken={refreshToken}");
        var payload = new { refreshToken };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/api/auth/refresh", content);

        if (response.IsSuccessStatusCode)
        {
            var token = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(token))
            {
                var result = JsonSerializer.Deserialize<AuthResponse>(token);
                await _accessTokenService.SetAccessTokenAsync(result.AccessToken);
                await _refreshTokenService.SetAsync(result.RefreshToken);

                return true;
            }
        }
        return false;
    }

    public async Task LogOut(string username)
    {
        var payload = new { username };
        try
        {
            var refreshToken = await _refreshTokenService.GetAsync();
            if (!string.IsNullOrEmpty(refreshToken))
            {
                _httpClient.DefaultRequestHeaders.Add("Cookie", $"refreshtopken={refreshToken}");
            }
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/auth/logout", content);

            if (response.IsSuccessStatusCode)
            {
                await _accessTokenService.RemoveAccessTokenAsync();
                await _refreshTokenService.DeleteAsync();
                _navigationManager.NavigateTo("/login", forceLoad: true);
            }
            else
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger?.LogWarning("Logout failed: {Status} {Body}", response.StatusCode, body);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Logout exception: {Message}", ex.Message);
            // Even if logout API fails, clear local tokens and redirect
            await _accessTokenService.RemoveAccessTokenAsync();
            await _refreshTokenService.DeleteAsync();
            _navigationManager.NavigateTo("/login", forceLoad: true);
        }
    }
}
