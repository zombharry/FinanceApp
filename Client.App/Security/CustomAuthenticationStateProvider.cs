using Client.App.DTO.AuthDtos;
using Client.App.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
        //var response = await _httpClient.GetFromJsonAsync<UserInfo>("api/auth/me");
        

        var userInfo = new UserInfo();

        if (userInfo is null)
        {
            _currentState = Anonymous();
            return _currentState;
        }
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.GivenName, userInfo.Username)
        };
        var identity = new ClaimsIdentity(claims, "cookie");
        _currentState = new AuthenticationState(new ClaimsPrincipal(identity));
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
