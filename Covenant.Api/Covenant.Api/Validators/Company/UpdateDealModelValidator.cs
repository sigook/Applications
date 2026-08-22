using Covenant.Common.Constants;
using Covenant.Common.Models.Company;
using Covenant.Common.Resources;
using FluentValidation;

namespace Covenant.Api.Validators.Company;

public class UpdateDealModelValidator : AbstractValidator<UpdateDealModel>
{
    public UpdateDealModelValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(m => m.Title)
            .NotEmpty()
            .MaximumLength(500)
            .WithName(ApiResources.Title);
        RuleFor(m => m.Date)
            .NotEmpty();
        RuleFor(m => m.Value)
            .GreaterThanOrEqualTo(0);
        RuleFor(m => m.Type)
            .IsInEnum();
        RuleFor(m => m.Status)
            .IsInEnum();
        RuleFor(m => m.DocumentId)
            .Must(documentId => documentId != Guid.Empty)
            .WithMessage("Invalid document");
    }
}
