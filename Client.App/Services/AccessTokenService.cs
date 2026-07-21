namespace Client.App.Services
{
    public class AccessTokenService
    {
        private readonly BlazoredLocalStorageService _storageService;
        private readonly string _tokenKey = "access_token";

        public AccessTokenService(BlazoredLocalStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            return await _storageService.GetAsync(_tokenKey);
        }

        public async Task SetAccessTokenAsync(string accessToken)
        {
            await _storageService.SetAsync(_tokenKey, accessToken);
        }

        public async Task RemoveAccessTokenAsync()
        {
            await _storageService.RemoveAsync(_tokenKey);
        }
    }
}
