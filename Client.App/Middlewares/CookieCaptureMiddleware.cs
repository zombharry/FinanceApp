using Client.App.Services;

namespace Client.App.Middlewares;

public class CookieCaptureMiddleware
{
    private readonly RequestDelegate _next;

    public CookieCaptureMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, CookieStore cookieStore)
    {
        // This runs during the real HTTP request — HttpContext is available here
        var cookieHeader = context.Request.Headers["Cookie"].ToString();
        if (!string.IsNullOrEmpty(cookieHeader))
        {
            cookieStore.CookieHeader = cookieHeader;
        }

        await _next(context);
    }
}
