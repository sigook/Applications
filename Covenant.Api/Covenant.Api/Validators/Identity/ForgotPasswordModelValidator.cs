using Covenant.Common.Models.Identity;
using FluentValidation;

namespace Covenant.Api.Validators.Identity;

public class ForgotPasswordModelValidator : AbstractValidator<ForgotPasswordModel>
{
    public ForgotPasswordModelValidator()
    {
        RuleFor(m => m.Email).NotEmpty().EmailAddress();
    }
}
