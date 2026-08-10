using Covenant.Common.Constants;
using Covenant.Common.Models.Company;
using Covenant.Common.Resources;
using FluentValidation;

namespace Covenant.Api.Validators.Company;

public class CreateCompanyInteractionModelValidator : AbstractValidator<CreateCompanyInteractionModel>
{
    public CreateCompanyInteractionModelValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(m => m.CompanyProfileId)
            .NotEmpty()
            .WithName(ApiResources.Company);
        RuleFor(m => m.Description)
            .NotEmpty()
            .MaximumLength(5000)
            .WithName(ApiResources.Description);
        RuleFor(m => m.InteractionPurpose)
            .IsInEnum();
        RuleFor(m => m.InteractionType)
            .IsInEnum();
        RuleFor(m => m.InteractionStatus)
            .IsInEnum();
    }
}
