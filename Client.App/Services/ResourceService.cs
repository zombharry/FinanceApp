namespace Client.App.Services
{
    public class ResourceService
    {
        private readonly ApiService _apiService;
        public ResourceService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<bool> Verify()
        {
            var result = await _apiService.GetAsync("api/Resource/verify");
            return result.IsSuccessStatusCode;
        }
    }
}
