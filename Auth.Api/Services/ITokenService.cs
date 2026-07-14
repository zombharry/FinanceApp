using Auth.Api.Data;
using Auth.Api.DTOs;

namespace Auth.Api.Services;

public interface ITokenService
{
    Task<string> CreateAccessTokenAsync(ApplicationUser user);

    Task<RefreshToken> CreateRefreshTokenAsync(string userId);

    Task<TokenPair> RefreshAccessTokenAsync(string refreshToken);

    Task RevokeRefreshTokenAsync(string userId);

}
