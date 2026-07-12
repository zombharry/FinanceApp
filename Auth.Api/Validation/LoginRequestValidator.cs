using Auth.Api.DTOs;
using FluentValidation;

namespace Auth.Api.Validation;

public class LoginRequestValidator :AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Email is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
