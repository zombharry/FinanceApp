using Client.App.DTO.AuthDtos;

namespace Client.App.Services;

public class CookieStore
{
    public string? CookieHeader { get; set; }

    public void SetTokenInsideCookie(AuthResponse tokens, HttpContext context)
    {
        context.Response.Cookies.Append("accessToken", tokens.AccessToken,
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(10),
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

        context.Response.Cookies.Append("refreshToken", tokens.RefreshToken,
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddHours(8),
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
    }
}
