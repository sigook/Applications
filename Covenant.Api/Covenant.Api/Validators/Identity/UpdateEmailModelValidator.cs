using Covenant.Common.Models.Identity;
using FluentValidation;

namespace Covenant.Api.Validators.Identity;

public class UpdateEmailModelValidator : AbstractValidator<UpdateEmailModel>
{
    public UpdateEmailModelValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(x => x.NewEmail)
            .NotEmpty()
            .EmailAddress();
    }
}
