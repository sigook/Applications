using Covenant.Common.Constants;
using Covenant.Common.Models.Request;
using Covenant.Common.Resources;
using FluentValidation;

namespace Covenant.Api.Validators.Request;

public class RequestCreateModelValidator : AbstractValidator<RequestCreateModel>
{
    private const int MaximumLengthJobTitle = 500;
    private const int MaximumLengthJobCosting = 100;
    private const int MaximumLengthDescription = 5000;
    private const int MaximumIncentive = 720;
    private const int MaximumLengthIncentiveDescription = 5000;
    private const int MinimumDurationBreakMinutes = 0;
    private const int MaximumDurationBreakMinutes = 60;

    public RequestCreateModelValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(m => m.JobTitle)
            .NotEmpty()
            .MaximumLength(MaximumLengthJobTitle)
            .WithName(ApiResources.JobTitle);
        RuleFor(m => m.BillingTitle)
            .MaximumLength(MaximumLengthJobTitle);
        RuleFor(m => m.JobCosting)
            .MaximumLength(MaximumLengthJobCosting);
        RuleFor(m => m.Description)
            .NotEmpty()
            .MaximumLength(MaximumLengthDescription)
            .WithName(ApiResources.Description);
        RuleFor(m => m.Requirements)
            .NotEmpty()
            .MaximumLength(CovenantConstants.Validation.MaximumLengthRequirements)
            .WithName(ApiResources.Requirements);
        RuleFor(m => m.Incentive)
            .LessThanOrEqualTo(MaximumIncentive)
            .WithName(ApiResources.Incentive);
        RuleFor(m => m.IncentiveDescription)
            .MaximumLength(MaximumLengthIncentiveDescription)
            .WithName(ApiResources.IncentiveDescription);
        RuleFor(m => m.DurationBreak)
            .InclusiveBetween(
                TimeSpan.FromMinutes(MinimumDurationBreakMinutes),
                TimeSpan.FromMinutes(MaximumDurationBreakMinutes));
    }
}
