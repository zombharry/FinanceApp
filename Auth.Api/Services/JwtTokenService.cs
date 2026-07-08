using Auth.Api.Data;
using Auth.Api.DTOs;
using Auth.Api.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.Api.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;
    private readonly UserManager<ApplicationUser> _userManager;
    public JwtTokenService(JwtOptions options, UserManager<ApplicationUser> userManager)
    {
        _options = options;
        _userManager = userManager;
    }
    public async Task<AuthResponse> CreateTokenAsync(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, user.Id),
            new (JwtRegisteredClaimNames.Email, user.Email!),
            new (ClaimTypes.NameIdentifier, user.Id),
            new (ClaimTypes.Name, user.UserName!)
        };

        var roles = await _userManager.GetRolesAsync(user);

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTime.UtcNow.AddMinutes(_options.ExpirationInMinutes);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt
        };
    }
}
