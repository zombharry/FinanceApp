using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Client.App.Services;

public class RefreshTokenService
{
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly ILogger<RefreshTokenService> _logger;
    private readonly string key = "refresh_token";

    public RefreshTokenService(ProtectedLocalStorage protectedLocalStorage, ILogger<RefreshTokenService> logger)
    {
        _protectedLocalStorage = protectedLocalStorage;
        _logger = logger;
    }

    public async Task SetAsync(string value)
    {
        try
        {
            await _protectedLocalStorage.SetAsync(key, value);
            _logger.LogInformation("Set refresh token in storage");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot access storage during prerendering for refresh token");
        }
    }

    public async Task<string> GetAsync()
    {
        try
        {
            var result = await _protectedLocalStorage.GetAsync<string>(key);
            if (result.Success)
            {
                _logger.LogInformation("Retrieved refresh token from storage");
                return result.Value ?? string.Empty;
            }
            _logger.LogWarning("Failed to retrieve refresh token from storage");
            return string.Empty;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot access storage during prerendering for refresh token");
            return string.Empty;
        }
    }

    public async Task DeleteAsync()
    {
        try
        {
            await _protectedLocalStorage.DeleteAsync(key);
            _logger.LogInformation("Deleted refresh token from storage");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot access storage during prerendering for refresh token");
        }
    }

}
