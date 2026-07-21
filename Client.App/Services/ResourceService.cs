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

    public async Task<IEnumerable<ProductGetDTO>> GetUserItems(string userId)
    {
        var response = await _apiService.PostDataAsync("api/Product/GetUserProduct",userId);

        using var responseStream = await response.Content.ReadAsStreamAsync();
        var doc = await JsonDocument.ParseAsync(responseStream);

        var userItems = new List<ProductGetDTO>();

        return userItems;
    }

    public async Task CreateNewItem(ProductGetDTO item)
    {

    }
}
