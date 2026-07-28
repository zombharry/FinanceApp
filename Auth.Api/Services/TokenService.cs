using Auth.Api.Data;
using Auth.Api.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Auth.Api.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _cfg;
    private readonly AuthDbContext _dbContext;

    public TokenService(IConfiguration cfg, AuthDbContext dbContext)
    {
        _cfg = cfg;
        _dbContext = dbContext;
    }

    public async Task<string> CreateAccessTokenAsync(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.GivenName, user.UserName),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var jwt = _cfg.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer:jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpirationInMinutes"])),
            signingCredentials: creds
            );

        return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(tokenDescriptor));
    }

    public async Task<RefreshToken> CreateRefreshTokenAsync(string userId)
    {     
        var lastToken = await _dbContext.RefreshTokens.OrderByDescending(t => t.CreatedAtUtc).FirstOrDefaultAsync(t => t.UserId.Equals(userId));
        if (lastToken is not null && !lastToken.IsRevoked)
        {
            lastToken.IsRevoked = true;
        }

        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            UserId = userId,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
        };

        await _dbContext.RefreshTokens.AddAsync(refreshToken);
        await _dbContext.SaveChangesAsync();

        return await Task.FromResult(refreshToken);
    }

    public async Task<TokenPair> RefreshAccessTokenAsync(string refreshToken)
    {
        var storedToken = await _dbContext.RefreshTokens.OrderByDescending(t => t.CreatedAtUtc).FirstOrDefaultAsync(rt => rt.Token.Equals(refreshToken) && rt.IsActive);

        if (storedToken is null || !storedToken.IsActive)
        {
            throw new UnauthorizedAccessException("Refresh token is invalid or has expired.");
        }

        storedToken.IsRevoked = true;

        var newRefreshToken = await CreateRefreshTokenAsync(storedToken.UserId);
        await _dbContext.RefreshTokens.AddAsync(newRefreshToken);
        await _dbContext.SaveChangesAsync();

        return new TokenPair(
            AccessToken: await CreateAccessTokenAsync(storedToken.User),
            RefreshToken: newRefreshToken.Token
            );
    }

    public async Task RevokeRefreshTokenAsync(string userId)
    {
        var tokens = await _dbContext.RefreshTokens.Where(t => t.UserId == userId && !t.IsRevoked).ToListAsync();
        foreach (var token in tokens)
        {
            token.IsRevoked = true;
        }
        await _dbContext.SaveChangesAsync();
    }

    public void SetTokenInsideCookie(TokenPair tokens, HttpContext context)
    {
        context.Response.Cookies.Append("accessToken", tokens.AccessToken,
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(10),
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Domain = "https://localhost:7237/"
            });

        context.Response.Cookies.Append("refreshToken", tokens.RefreshToken,
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddHours(8),
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Domain = "https://localhost:7237/"
            });
    }

    public void UnSetCookies(TokenPair tokens, HttpContext context)
    {
        context.Response.Cookies.Delete(tokens.AccessToken);
        //context.Response.Cookies.Delete(tokens.RefreshToken);
    }
}
