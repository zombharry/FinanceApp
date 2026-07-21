using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;

namespace Client.App.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly AccessTokenService _tokenService;
    private readonly AuthService _authService;
    private readonly NavigationManager _navigationManager;
    public ApiService(
        IHttpClientFactory httpFactory,
        AccessTokenService accessTokenService,
        AuthService authService,
        NavigationManager navigationManager
        )
    {
        _httpClient = httpFactory.CreateClient("ApiClient"); 
        _tokenService = accessTokenService;
        _authService = authService;
        _navigationManager = navigationManager;
    }

    public async Task<HttpResponseMessage> GetAsync(string endpoint)
    {
        var token = await _tokenService.GetAccessTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var responseMessage = await _httpClient.GetAsync(endpoint);

        if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {

            var refreshTokenResult = await _authService.RefreshTokenAsync();
            if (!refreshTokenResult)
            {
                //await _authService.LogOut();
            }

            var newToken = await _tokenService.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", newToken);

            var newResponse = await _httpClient.GetAsync(endpoint);

            return newResponse;
        }
        return responseMessage;
    }

    //public async Task<HttpResponseMessage> PostDataAsync(string ednpoint, object obj)
    //{ 
    //}
}
