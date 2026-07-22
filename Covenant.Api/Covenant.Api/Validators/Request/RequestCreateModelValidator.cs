using Covenant.Common.Constants;
using Covenant.Common.Models.Request;
using Covenant.Common.Resources;
using FluentValidation;

namespace Covenant.Api.Validators.Request;

public class RequestCreateModelValidator : AbstractValidator<RequestCreateModel>
{
    public RequestCreateModelValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(m => m.JobTitle)
            .NotEmpty()
            .MaximumLength(CovenantConstants.Validation.MaximumLengthJobTitle)
            .WithName(ApiResources.JobTitle);
        RuleFor(m => m.BillingTitle)
            .MaximumLength(CovenantConstants.Validation.MaximumLengthJobTitle);
        RuleFor(m => m.JobCosting)
            .MaximumLength(CovenantConstants.Validation.MaximumLengthJobCosting);
        RuleFor(m => m.Description)
            .NotEmpty()
            .MaximumLength(CovenantConstants.Validation.MaximumLengthDescription)
            .WithName(ApiResources.Description);
        RuleFor(m => m.Requirements)
            .NotEmpty()
            .MaximumLength(CovenantConstants.Validation.MaximumLengthRequirements)
            .WithName(ApiResources.Requirements);
        RuleFor(m => m.Incentive)
            .LessThanOrEqualTo(CovenantConstants.Validation.MaximumIncentive)
            .WithName(ApiResources.Incentive);
        RuleFor(m => m.IncentiveDescription)
            .MaximumLength(CovenantConstants.Validation.MaximumLengthIncentiveDescription)
            .WithName(ApiResources.IncentiveDescription);
        RuleFor(m => m.DurationBreak)
            .InclusiveBetween(
                TimeSpan.FromMinutes(CovenantConstants.Validation.MinimumDurationBreakMinutes),
                TimeSpan.FromMinutes(CovenantConstants.Validation.MaximumDurationBreakMinutes));
    }
}
