using Client.App.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Client.App.Security;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly AccessTokenService _accessTokenService;

    public JwtAuthenticationStateProvider(AccessTokenService accessTokenService)
    {
        _accessTokenService = accessTokenService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _accessTokenService.GetAccessTokenAsync();

            if (string.IsNullOrWhiteSpace(token))
            {
                return await MarkAsUnAouthorized();
            }
            var readJwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var identity = new ClaimsIdentity(readJwt.Claims, "JWT");

            var principal = new ClaimsPrincipal(identity);

            return await Task.FromResult(new AuthenticationState(principal));
        }
        catch (Exception)
        {

            return await MarkAsUnAouthorized();
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
