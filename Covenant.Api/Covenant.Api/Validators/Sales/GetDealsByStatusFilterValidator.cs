using Covenant.Common.Models.Sales.Dashboard;
using FluentValidation;

namespace Covenant.Api.Validators.Sales;

public class GetDealsByStatusFilterValidator : AbstractValidator<GetDealsByStatusFilter>
{
    public GetDealsByStatusFilterValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(f => f.Period)
            .IsInEnum();
        RuleForEach(f => f.Statuses)
            .IsInEnum();
    }
}
