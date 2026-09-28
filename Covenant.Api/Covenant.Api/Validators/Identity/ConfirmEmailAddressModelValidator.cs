using Covenant.Common.Models.Identity;
using FluentValidation;

namespace Covenant.Api.Validators.Identity;

public class ConfirmEmailAddressModelValidator : AbstractValidator<ConfirmEmailAddressModel>
{
    public ConfirmEmailAddressModelValidator()
    {
        RuleFor(m => m.Id).NotEmpty();
        RuleFor(m => m.Token).NotEmpty();
    }
}
