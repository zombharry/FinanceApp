using Api.Client.Client.DTOs;

namespace Api.Client.Client.Authentication
{
    public interface ITokenStore
    {
        string? AccessToken { get; }
        string? RefreshToken { get; }

        void Clear();
        void Set(TokenPair tokenPair);
    }
}