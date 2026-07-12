using Auth.Api.Data;
using Auth.Api.DTOs;
using Auth.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwt;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtTokenService jwt)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwt = jwt;
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

        return Ok(_jwt.CreateTokenAsync(user));
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

        var token = await _jwt.CreateTokenAsync(user);
        return Ok(new { access_token = token });
    }
}
