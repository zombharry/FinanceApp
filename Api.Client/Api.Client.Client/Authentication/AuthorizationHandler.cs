using System.Net.Http.Headers;

namespace Api.Client.Client.Authentication;

public class AuthorizationHandler : DelegatingHandler
{
    private readonly ITokenStore _tokenStore;

    public AuthorizationHandler(ITokenStore tokenStore)
    {
        _tokenStore = tokenStore;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_tokenStore.AccessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenStore.AccessToken);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
