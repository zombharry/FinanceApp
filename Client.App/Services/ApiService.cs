using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Headers;

namespace Client.App.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly NavigationManager _navigationManager;
    public ApiService(
        IHttpClientFactory httpFactory,
        AuthService authService,
        NavigationManager navigationManager
        )
    {
        _httpClient = httpFactory.CreateClient("ResourceClient");
        _authService = authService;
        _navigationManager = navigationManager;
    }

    public async Task<HttpResponseMessage> GetAsync(string endpoint)
    {
       
        var responseMessage = await _httpClient.GetAsync(endpoint);

        if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
        {

            var newResponse = await _httpClient.GetAsync(endpoint);

            return newResponse;
        }
        return responseMessage;
    }

    public async Task<HttpResponseMessage> PostDataAsync(string endpoint, object obj)
    {
        var responseMessage = await _httpClient.PostAsJsonAsync(endpoint, obj);

        if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            var newResponse = await _httpClient.GetAsync(endpoint);

            return newResponse;
        }
        return responseMessage;

    }
}
