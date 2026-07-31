using Client.App.DTO.ItemDtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Client.App.Services;

public class ResourceService
{
    private readonly ApiService _apiService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public ResourceService(ApiService apiService, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _apiService = apiService;
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }
    private async Task<HttpClient> CreateAuthorizedClient()
    {
        var client = _httpClientFactory.CreateClient("ResourceClient"); 
        var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, "access_token");

        if (!string.IsNullOrEmpty(accessToken))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
        }
        return client;
    }

    public async Task<bool> Verify()
    {
        var response = await _apiService.GetAsync("api/Resource/verify");
        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<ProductGetDTO>> GetUserItems(Guid userIdentifier)
    {
        var client = await CreateAuthorizedClient();
        var response = await client.GetAsync($"api/product/getuserproduct?userId={userIdentifier}");

        if (!response.IsSuccessStatusCode)
        {
            return Enumerable.Empty<ProductGetDTO>();
        }

        await using var responseStream = await response.Content.ReadAsStreamAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var userItems = await JsonSerializer.DeserializeAsync<IEnumerable<ProductGetDTO>>(responseStream, options);

        return userItems ?? Enumerable.Empty<ProductGetDTO>();
    }

    public async Task CreateNewItem(ProductCreateDto item)
    {
        var client = await CreateAuthorizedClient();
        var response = await client.PostAsJsonAsync("api/product/create", item);

        response.EnsureSuccessStatusCode();
    }

    public async Task<ProductGetDTO> GetProductById(Guid productId)
    {
        var client = await CreateAuthorizedClient();
        var response = await client.GetAsync($"api/product/getbyid?productId={productId}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        await using var responseStream = await response.Content.ReadAsStreamAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var product = await JsonSerializer.DeserializeAsync<ProductGetDTO>(responseStream, options);

        return product;
    }

    public async Task EditProduct(ProductEditDTO item)
    {
        var client = await CreateAuthorizedClient();
        var response = await client.PostAsJsonAsync("api/product/edit", item);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteItem(Guid itemIdentifier)
    {
        var client = await CreateAuthorizedClient();
        var response = await client.DeleteAsync($"api/product/delete?id={itemIdentifier}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<IEnumerable<CategoryGetDTO>> GetCategories()
    {
        var client = await CreateAuthorizedClient();
        var response = await client.GetAsync("api/Category/getall");

        if (!response.IsSuccessStatusCode)
        {
            return Enumerable.Empty<CategoryGetDTO>();
        }

        var responseString = await response.Content.ReadAsStringAsync();

        await using var responseStream = await response.Content.ReadAsStreamAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var categories = await JsonSerializer.DeserializeAsync<IEnumerable<CategoryGetDTO>>(responseStream, options);

        return categories ?? Enumerable.Empty<CategoryGetDTO>();
    }
}
