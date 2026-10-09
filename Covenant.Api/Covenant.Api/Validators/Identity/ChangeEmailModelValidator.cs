using Covenant.Common.Models.Identity;
using FluentValidation;

namespace Covenant.Api.Validators.Identity;

public class ChangeEmailModelValidator : AbstractValidator<ChangeEmailModel>
{
    public ChangeEmailModelValidator()
    {
        RuleFor(m => m.NewEmail).NotEmpty().EmailAddress();
        RuleFor(m => m.ConfirmNewEmail).Equal(m => m.NewEmail);
    }
}
