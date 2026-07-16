using Api.Client.Client.DTOs;

namespace Api.Client.Client.Services
{
    public interface IAuthService
    {
        Task<TokenPair> LoginAsync(LoginRequest request);

        Task LogoutAsync();
    }
}