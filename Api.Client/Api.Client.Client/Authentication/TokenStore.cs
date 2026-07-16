using Api.Client.Client.DTOs;

namespace Api.Client.Client.Authentication
{
    public class TokenStore : ITokenStore
    {
        public string? AccessToken { get; private set; }
        public string? RefreshToken { get; private set; }

        public void Set(TokenPair tokenPair)
        {
            AccessToken = tokenPair.AccessToken;
            RefreshToken = tokenPair.RefreshToken;
        }

        public void Clear()
        {
            AccessToken = null;
            RefreshToken = null;
        }
    }
}
