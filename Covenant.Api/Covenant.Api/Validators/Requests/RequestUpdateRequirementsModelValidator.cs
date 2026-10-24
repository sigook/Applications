using Covenant.Common.Constants;
using Covenant.Common.Models.Requests.Companies;
using Covenant.Common.Models.Requests;
using Covenant.Common.Resources;
using FluentValidation;

namespace Covenant.Api.Validators.Requests;

public class RequestUpdateRequirementsModelValidator : AbstractValidator<RequestUpdateRequirementsModel>
{
    public RequestUpdateRequirementsModelValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(m => m.Requirements)
            .NotEmpty()
            .MaximumLength(CovenantConstants.Validation.MaximumLengthRequirements)
            .WithName(ApiResources.Requirements);
    }
}
