using Auth.Api.Data;
using Auth.Api.DTOs;
using Auth.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Auth.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var existing = await _userManager.FindByNameAsync(req.Username);
        if (existing is not null)
        {
            return Conflict("User already exists");
        }

        var user = new ApplicationUser
        {
            UserName = req.Username,
            Email = req.Email
        };
        var result = await _userManager.CreateAsync(user, req.Password);
        
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return BadRequest(errors);
        }

        return Ok(_tokenService.CreateAccessTokenAsync(user));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByNameAsync(req.Username);
        if (user is null)
        { 
            return Unauthorized();
        }

        var valid = await _signInManager.CheckPasswordSignInAsync(user, req.Password, false);
        if (!valid.Succeeded)
        {
            return Unauthorized();
        }

        var accesToken = await _tokenService.CreateAccessTokenAsync(user);
        var refreshToken = await _tokenService.CreateRefreshTokenAsync(user.Id);
        var tokenPair = new TokenPair(accesToken, refreshToken.Token);

        _tokenService.SetTokenInsideCookie(tokenPair, HttpContext);

        return Ok(tokenPair);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequest req)
    {
        var user = await _userManager.FindByNameAsync(req.Username);
        if (user is null)
        {
            return Unauthorized();
        }
        await _tokenService.RevokeRefreshTokenAsync(user.Id);
        await _signInManager.SignOutAsync();
        return Ok();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        HttpContext.Request.Cookies.TryGetValue("accessToken", out var accesToken);
        HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken);

        var tokenPair = await _tokenService.RefreshAccessTokenAsync(refreshToken);
        _tokenService.SetTokenInsideCookie(tokenPair, HttpContext);

        return Ok();
    }

    [HttpGet("getUserId")]
    public async Task<IActionResult> GetUserId(string username)
    {
        var user = await _userManager.FindByNameAsync(username);

        var response = new UserResponse
        {
            UserId = Guid.Parse(user.Id)
        };

        return Ok(response);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = User.FindFirstValue(ClaimTypes.GivenName);

        if (userId is null)
        {
            return Unauthorized();
        }

        return Ok(new UserResponse
        {
            UserId = Guid.Parse(userId),
            UserName = userName
        });
    }
}
