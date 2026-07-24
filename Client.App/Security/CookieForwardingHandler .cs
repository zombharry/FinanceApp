using Client.App.Services;

namespace Client.App.Security;

public class CookieForwardingHandler : DelegatingHandler
{
    private readonly CookieStore _cookieStore;

    public CookieForwardingHandler(CookieStore cookieStore)
    {
        _cookieStore = cookieStore;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(_cookieStore.CookieHeader))
        {
            request.Headers.Remove("Cookie");
            request.Headers.Add("Cookie", _cookieStore.CookieHeader);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
