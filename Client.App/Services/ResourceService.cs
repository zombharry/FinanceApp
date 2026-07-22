using Client.App.DTO.ItemDtos;
using System.Text.Json;

namespace Client.App.Services;

public class ResourceService
{
    private readonly ApiService _apiService;
    public ResourceService(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<bool> Verify()
    {
        var response = await _apiService.GetAsync("api/Resource/verify");
        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<ProductGetDTO>> GetUserItems(Guid userIdentifier)
    {
        //var endpoint = $"api/Product/getuserproduct?userId={Uri.EscapeDataString(userIdentifier)}";
        var response = await _apiService.GetAsync($"api/product/getuserproduct?userId={userIdentifier}");

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
        var response = await _apiService.PostDataAsync("api/Product/create", item);

        response.EnsureSuccessStatusCode();
    }

    public async Task<ProductGetDTO> GetProductById(Guid productId)
    {
        var response = await _apiService.GetAsync($"api/product/getbyid?productId={productId}");

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
        var response = await _apiService.PostDataAsync("api/product/edit", item);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteItem(Guid itemIdentifier) { }

    public async Task<IEnumerable<CategoryGetDTO>> GetCategories()
    {
        var response = await _apiService.GetAsync("api/Category/getall");

        if (!response.IsSuccessStatusCode)
        {
            return Enumerable.Empty<CategoryGetDTO>();
        }

        await using var responseStream = await response.Content.ReadAsStreamAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var categories = await JsonSerializer.DeserializeAsync<IEnumerable<CategoryGetDTO>>(responseStream, options);

        return categories ?? Enumerable.Empty<CategoryGetDTO>();
    }
}
