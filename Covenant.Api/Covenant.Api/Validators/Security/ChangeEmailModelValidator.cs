using Covenant.Common.Models.Security;
using FluentValidation;

namespace Covenant.Api.Validators.Security;

public class ChangeEmailModelValidator : AbstractValidator<ChangeEmailModel>
{
    public ChangeEmailModelValidator()
    {
        RuleFor(m => m.NewEmail).NotEmpty().EmailAddress();
        RuleFor(m => m.ConfirmNewEmail).Equal(m => m.NewEmail);
    }
}
