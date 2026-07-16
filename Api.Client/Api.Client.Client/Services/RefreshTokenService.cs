using Api.Client.Client.Authentication;
using Api.Client.Client.DTOs;
using System.Net.Http.Json;

namespace Api.Client.Client.Services;

public class RefreshTokenService
{
    private readonly TokenStore _tokenStore;
    private readonly HttpClient _httpClient;

    public RefreshTokenService(TokenStore tokenStore, HttpClient httpClient)
    {
        _tokenStore = tokenStore;
        _httpClient = httpClient;
    }
    public async Task<bool> RefreshAsync()
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/refresh", _tokenStore.RefreshToken);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var tokenPair = await response.Content.ReadFromJsonAsync<TokenPair>();

        _tokenStore.Set(tokenPair!);

        return true;
    }

}
