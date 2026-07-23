using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;


namespace Client.App.Services;

public class LocalStorageService
{
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly ILogger<LocalStorageService> _logger;

    private const string AccessTokenKey = "access_token";
    private const string RefreshTokenKey = "refresh_token";

    public LocalStorageService(ProtectedLocalStorage protectedLocalStorage, ILogger<LocalStorageService> logger)
    {
        _protectedLocalStorage = protectedLocalStorage;
        _logger = logger;
    }

    public async Task<string> GetAsync(string key)
    {
        try
        {
            var result = await _protectedLocalStorage.GetAsync<string>(key);
            if (result.Success)
            {
                _logger.LogInformation($"Retrieved {key} from storage");
                return result.Value ?? string.Empty;
            }
            _logger.LogWarning($"Failed to retrieve {key} from storage");
            return string.Empty;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, $"Cannot access storage during prerendering for key {key}");
            return string.Empty;
        }
    }

    public async Task SetAsync(string key, string value)
    {
        try
        {
            await _protectedLocalStorage.SetAsync(key, value);
            _logger.LogInformation($"Set {key} in storage");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, $"Cannot access storage during prerendering for key {key}");
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _protectedLocalStorage.DeleteAsync(key);
            _logger.LogInformation($"Removed {key} from storage");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, $"Cannot access storage during prerendering for key {key}");
        }
    }
}
