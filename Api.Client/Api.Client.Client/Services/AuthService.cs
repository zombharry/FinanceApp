using Api.Client.Client.Authentication;
using Api.Client.Client.DTOs;
using System.Net.Http.Json;

namespace Api.Client.Client.Services;

public class AuthService
{
    private readonly TokenStore _tokenStore;
    private readonly HttpClient _httpClient;

    public AuthService(TokenStore tokenStore, HttpClient httpClient)
    {
        _tokenStore = tokenStore;
        _httpClient = httpClient;
    }
    public async Task LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

        response.EnsureSuccessStatusCode();

        var tokenPair = await response.Content.ReadFromJsonAsync<TokenPair>();

        _tokenStore.Set(tokenPair!);
    }

    Task RegisterAsync()
    { 
        return null; 
    }

    Task LogoutAsync()
    { 
        return null; 
    }
}
