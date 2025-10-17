using FluentValidation;
using YAPA.Models.Auth;

namespace YAPA.Validators;
public class TokenRefreshRequestValidator : AbstractValidator<TokenRefreshRequest>
{
    public TokenRefreshRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .Must(GuidValidateToken).WithErrorCode("Not a Guid");
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email need to be validate and not empty");
    }
    private bool GuidValidateToken(string token)
    {
        return Guid.TryParse(token, out _);
    }
}
