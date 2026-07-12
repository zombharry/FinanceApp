using FinanceApp.Models;
using FinanceApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FinanceApp.Controllers;

public class AccountController : Controller
{
    private readonly AuthApiClient _authApi;
    private readonly TokenValidationParameters _tvp;

    public AccountController(AuthApiClient authApi, TokenValidationParameters tvp)
    {
        _authApi = authApi;
        _tvp = tvp;
    }

    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var token = await _authApi.GetTokenAsync(model.Username, model.Password);
        if (token is null)
        {
            ModelState.AddModelError("Username or password was incorrect", "Login failed");
            return View(model);
        }

        var handler = new JwtSecurityTokenHandler();
        try
        {
            handler.ValidateToken(token, _tvp, out var validatedToken);
            var jwt = handler.ReadJwtToken(token);
            var claims = jwt.Claims.ToList();
            if (!claims.Any(c => c.Type == ClaimTypes.Name))
            {
                claims.Add(new Claim(ClaimTypes.Name, model.Username));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var props = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);
            return RedirectToAction("Index", "Home");
        }
        catch (SecurityTokenException)
        {
            ModelState.AddModelError("", "Invalid token");
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Register(string returnUrl = null)
    {
        return View(new Models.RegisterViewModelcs { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(Models.RegisterViewModelcs model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var (success, error) = await _authApi.RegisterAsync(model.Username, model.Email, model.Password);
        if (!success)
        {
            ModelState.AddModelError("", error ?? "Registration failed");
            return View(model);
        }
        return RedirectToAction("Login", new { returnUrl = model.ReturnUrl });

    }
}
