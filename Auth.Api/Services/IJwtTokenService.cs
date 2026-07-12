using Auth.Api.Data;
using Auth.Api.DTOs;

namespace Auth.Api.Services;

public interface IJwtTokenService
{
    Task<string> CreateTokenAsync(ApplicationUser user);
}
