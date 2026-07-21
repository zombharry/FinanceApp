using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Client.App.Services;

public class RefreshTokenService
{
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly string key = "refresh_token";
    public RefreshTokenService(ProtectedLocalStorage protectedLocalStorage)
    {
        _protectedLocalStorage = protectedLocalStorage;
    }

    public async Task SetAsync(string value)
    {
        await _protectedLocalStorage.SetAsync(key, value);
    }

    public async Task<string> GetAsync()
    {
        var result = await _protectedLocalStorage.GetAsync<string>(key);
        if (result.Success)
        {
            return result.Value;
        }

        return string.Empty;
    }

    public async Task DeleteAsync()
    {
        await _protectedLocalStorage.DeleteAsync(key);
    }

}
