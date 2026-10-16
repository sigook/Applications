using Covenant.Common.Models.Sales.Dashboard;
using FluentValidation;

namespace Covenant.Api.Validators.Sales;

public class GetDealsByStatusFilterValidator : AbstractValidator<GetDealsByStatusFilter>
{
    public GetDealsByStatusFilterValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(f => f.DateFrom)
            .NotNull()
            .When(f => f.DateTo.HasValue);
        RuleFor(f => f.DateTo)
            .NotNull()
            .When(f => f.DateFrom.HasValue);
        RuleFor(f => f.DateTo)
            .GreaterThanOrEqualTo(f => f.DateFrom)
            .When(f => f.DateFrom.HasValue && f.DateTo.HasValue);
        RuleForEach(f => f.Statuses)
            .IsInEnum();
    }
}
