using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Api.Client.Client.Authentication;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ITokenStore _tokenStore;

    public JwtAuthenticationStateProvider(
        ITokenStore tokenStore)
    {
        _tokenStore = tokenStore;
    }
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        ClaimsIdentity identity;

        if (string.IsNullOrWhiteSpace(_tokenStore.AccessToken))
        {
            identity = new ClaimsIdentity();
        }
        else
        {
            var claims = JwtParser.ParseClaims(_tokenStore.AccessToken);

            identity = new ClaimsIdentity(claims, authenticationType:"jwt");
        }

        var principal = new ClaimsPrincipal(identity);

        return Task.FromResult(
            new AuthenticationState(principal));
    }
    public void NotifyUserAuthentication()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void NotifyUserLogout()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
