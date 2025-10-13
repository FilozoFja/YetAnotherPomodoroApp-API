using FluentValidation;
using YAPA.Models.Auth;

namespace YAPA.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email is required");
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .Must(x => !string.IsNullOrWhiteSpace(x));
    }
}