namespace Client.App.Services
{
    public class AccessTokenService
    {
        private readonly CookieService _cookieService;
        private readonly string _tokenKey = "access_token";
        public AccessTokenService(CookieService cookieService)
        {
            _cookieService = cookieService;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            return await _cookieService.GetCookie(_tokenKey);
        }

        public async Task SetAccessTokenAsync(string accessToken)
        {
            await _cookieService.SetCookie(_tokenKey, accessToken, 7);
        }

        public async Task RemoveAccessTokenAsync()
        {
            await _cookieService.RemoveCookie(_tokenKey);
        }
    }
}
