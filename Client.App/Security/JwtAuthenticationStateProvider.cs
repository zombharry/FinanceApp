using Client.App.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Client.App.Security;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly AccessTokenService _accessTokenService;
    private readonly ILogger<JwtAuthenticationStateProvider> _logger;

    public JwtAuthenticationStateProvider(AccessTokenService accessTokenService, ILogger<JwtAuthenticationStateProvider> logger)
    {
        _accessTokenService = accessTokenService;
        _logger = logger;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _accessTokenService.GetAccessTokenAsync();
            _logger.LogInformation($"GetAuthenticationStateAsync: Token retrieved, isEmpty: {string.IsNullOrWhiteSpace(token)}");

            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogInformation("GetAuthenticationStateAsync: Token is empty, marking as unauthorized");
                return await MarkAsUnAouthorized();
            }
            var readJwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var identity = new ClaimsIdentity(readJwt.Claims, "JWT");

            var principal = new ClaimsPrincipal(identity);
            _logger.LogInformation($"GetAuthenticationStateAsync: User authenticated as {identity.Name}");

            return await Task.FromResult(new AuthenticationState(principal));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAuthenticationStateAsync: Exception occurred");
            return await MarkAsUnAouthorized();
        }
    }

    public async Task NotifyAuthenticationStateChangedAsync()
    {
        try
        {
            _logger.LogInformation("NotifyAuthenticationStateChangedAsync: Starting notification");
            var token = await _accessTokenService.GetAccessTokenAsync();
            _logger.LogInformation($"NotifyAuthenticationStateChangedAsync: Token retrieved, isEmpty: {string.IsNullOrWhiteSpace(token)}");

            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogInformation("NotifyAuthenticationStateChangedAsync: Token is empty, marking as unauthorized");
                await MarkAsUnAouthorized();
            }
            else
            {
                var readJwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var identity = new ClaimsIdentity(readJwt.Claims, "JWT");
                var principal = new ClaimsPrincipal(identity);
                var state = new AuthenticationState(principal);
                _logger.LogInformation($"NotifyAuthenticationStateChangedAsync: Notifying change for user {identity.Name}");
                NotifyAuthenticationStateChanged(Task.FromResult(state));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "NotifyAuthenticationStateChangedAsync: Exception occurred");
            await MarkAsUnAouthorized();
        }
    }

    private async Task<AuthenticationState> MarkAsUnAouthorized()
    {
        try
        {
            var state = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            NotifyAuthenticationStateChanged(Task.FromResult(state));
            return state;
        }
        catch (Exception)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }
}
