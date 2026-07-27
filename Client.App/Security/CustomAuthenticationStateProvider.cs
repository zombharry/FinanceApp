using Client.App.DTO.AuthDtos;
using Client.App.DTO.ItemDtos;
using Client.App.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace Client.App.Security;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    
    private readonly ILogger<CustomAuthenticationStateProvider> _logger;
    private AuthenticationState _currentState = Anonymous();
    private readonly HttpClient _httpClient;

    public CustomAuthenticationStateProvider(IHttpClientFactory httpClientFactory, ILogger<CustomAuthenticationStateProvider> logger)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _logger = logger;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var response = await _httpClient.GetAsync("api/auth/me");

        if (!response.IsSuccessStatusCode)
        {
            _currentState = Anonymous();
            return _currentState;
        }

        await using var responseStream = await response.Content.ReadAsStreamAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var userInfo = await JsonSerializer.DeserializeAsync<UserInfo>(responseStream, options);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userInfo.UserId.ToString()),
            new Claim(ClaimTypes.GivenName, userInfo.Username)
        };

        var identity = new ClaimsIdentity(claims, "cookie");
        _currentState = new AuthenticationState(new ClaimsPrincipal(identity));

        var isAuthenticated = identity.IsAuthenticated;
        var test = _currentState.User.Identity?.IsAuthenticated;

        return _currentState;
    }

    public async Task NotifyAuthenticationStateChangedAsync()
    {
        var state = await GetAuthenticationStateAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(state));

    }

    public async Task NotifyUserLoggedOut()
    {
        _currentState = Anonymous();
        NotifyAuthenticationStateChanged(Task.FromResult(_currentState));
    }

    private static AuthenticationState Anonymous() =>
        new(new ClaimsPrincipal(new ClaimsIdentity()));
}
