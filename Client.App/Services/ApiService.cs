using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Headers;

namespace Client.App.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuthService _authService;
    private readonly NavigationManager _navigationManager;
    public ApiService(
        IHttpClientFactory httpFactory,
        IHttpContextAccessor httpContextAccessor,
        AuthService authService,
        NavigationManager navigationManager
        )
    {
        _httpClient = httpFactory.CreateClient("ResourceClient");
        _httpContextAccessor = httpContextAccessor;
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
            var newResponse = await _httpClient.PostAsJsonAsync(endpoint,obj);

            return newResponse;
        }
        return responseMessage;

    }
    private void ForwardAuthCookie(HttpRequestMessage request)
    {
        var cookie = _httpContextAccessor.HttpContext?.Request.Headers.Cookie;
        if (!string.IsNullOrEmpty(cookie))
        {
            request.Headers.Add("Cookie", cookie.ToString());
        }
    }
}
