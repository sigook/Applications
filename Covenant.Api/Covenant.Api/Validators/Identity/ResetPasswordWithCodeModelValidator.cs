using Covenant.Common.Models.Identity;
using FluentValidation;

namespace Covenant.Api.Validators.Identity;

public class ResetPasswordWithCodeModelValidator : AbstractValidator<ResetPasswordWithCodeModel>
{
    public ResetPasswordWithCodeModelValidator()
    {
        RuleFor(m => m.Email).NotEmpty().EmailAddress();
        RuleFor(m => m.Code).NotEmpty().Matches("^[0-9]{6}$");
        RuleFor(m => m.NewPassword).NotEmpty().MinimumLength(6);
    }
}
