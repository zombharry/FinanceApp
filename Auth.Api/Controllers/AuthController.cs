using Auth.Api.Data;
using Auth.Api.DTOs;
using Auth.Api.Exceptions;
using Auth.Api.Services;
using Auth.Api.Validation;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Controllers;

public class AuthController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwtService;
    private readonly IValidator<RegisterRequest> _registerRequestValidator;
    private readonly IValidator<LoginRequest> _loginRequestValidator;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtService,
        IValidator<LoginRequest> loginRequestValidator,
        IValidator<RegisterRequest> registerRequestValidator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _loginRequestValidator = loginRequestValidator;
        _registerRequestValidator = registerRequestValidator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        //Fluent Validátor a jelszavak egyezőségét itt ellenőrizni fogja
        var validationResult = await _registerRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationFailedException(validationResult.Errors);
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded) 
        { 
            throw new ValidationFailedException(
                result.Errors.Select(e =>
                new ValidationFailure(e.Code, e.Description))); 
        }

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var validationResult = await _loginRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        var validPassword = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!validPassword)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        var token = await _jwtService.CreateTokenAsync(user);

        return Ok(token);
    }
}
