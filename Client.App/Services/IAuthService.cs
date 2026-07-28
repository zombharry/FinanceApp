using Client.App.DTO.AuthDtos;

namespace Client.App.Services
{
    public interface IAuthService
    {
        Task<Guid> GetUserId(string username);
        Task<UserInfo?> GetUserInfoAsync();
        Task<(bool Success, string? Error)> RegisterAsync(string username, string email, string password);
    }
}