using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Client.App.DTO.AuthDtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;

namespace Client.App.Endpoints;

public static class ClientEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/signin", async (
            HttpContext httpContext,
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            TokenValidationParameters tvp) =>
        {
            var form = await httpContext.Request.ReadFormAsync();
            var username = form["username"].ToString();
            var password = form["password"].ToString();
            var returnUrl = form["returnUrl"].ToString();

            var client = httpClientFactory.CreateClient("ApiClient");
            var tokenEndpoint = config["AuthApi:TokenEndpoint"] ?? "/api/auth/login";
            var response = await client.PostAsJsonAsync(tokenEndpoint, new { username, password });

            if (!response.IsSuccessStatusCode)
            {
                return Results.Redirect(BuildLoginRedirect(returnUrl, error: true));
            }

            var tokens = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (string.IsNullOrEmpty(tokens?.AccessToken))
            {
                return Results.Redirect(BuildLoginRedirect(returnUrl, error: true));
            }

            var handler = new JwtSecurityTokenHandler();
            ClaimsPrincipal validatedPrincipal;
            try
            {
                validatedPrincipal = handler.ValidateToken(tokens.AccessToken, tvp, out _);
            }
            catch (SecurityTokenException)
            {
                return Results.Redirect(BuildLoginRedirect(returnUrl, error: true));
            }

            var claims = validatedPrincipal.Claims.ToList();
            if (!claims.Any(c => c.Type == ClaimTypes.Name))
            {
                claims.Add(new Claim(ClaimTypes.Name, username));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            });

            return Results.Redirect(IsLocalUrl(returnUrl) ? returnUrl : "/");
        });

        app.MapGet("/logout", async (HttpContext httpContext, IHttpClientFactory httpClientFactory) =>
        {
            var username = httpContext.User.Identity?.Name;
            if (!string.IsNullOrEmpty(username))
            {
                var client = httpClientFactory.CreateClient("ApiClient");
                await client.PostAsJsonAsync("/api/auth/logout", new { username });
            }

            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Redirect("/login");
        });
    }
    private static bool IsLocalUrl(string? url) =>
        !string.IsNullOrEmpty(url) && url.StartsWith('/') && !url.StartsWith("//") && !url.StartsWith("/\\");

    private static string BuildLoginRedirect(string? returnUrl, bool error)
    {
        var url = "/login";
        var query = new List<string>();
        if (error)
        {
            query.Add("error=1");
        }
        if (!string.IsNullOrEmpty(returnUrl))
        {
            query.Add($"returnUrl={Uri.EscapeDataString(returnUrl)}");
        }
        return query.Count == 0 ? url : $"{url}?{string.Join("&", query)}";
    }
}
