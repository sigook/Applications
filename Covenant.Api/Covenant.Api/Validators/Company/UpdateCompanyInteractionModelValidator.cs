using Covenant.Common.Constants;
using Covenant.Common.Models.Company;
using Covenant.Common.Resources;
using FluentValidation;

namespace Covenant.Api.Validators.Company;

public class UpdateCompanyInteractionModelValidator : AbstractValidator<UpdateCompanyInteractionModel>
{
    public UpdateCompanyInteractionModelValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(m => m.Description)
            .NotEmpty()
            .MaximumLength(CovenantConstants.Validation.MaximumLengthInteractionDescription)
            .WithName(ApiResources.Description);
        RuleFor(m => m.InteractionPurpose)
            .IsInEnum();
        RuleFor(m => m.InteractionType)
            .IsInEnum();
        RuleFor(m => m.InteractionStatus)
            .IsInEnum();
    }
}
