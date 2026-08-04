using Covenant.Common.Models.Security;
using FluentValidation;

namespace Covenant.Api.Validators.Security;

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
