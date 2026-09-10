using Covenant.Common.Models.Company.SalesDashboard;
using FluentValidation;

namespace Covenant.Api.Validators.Company;

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
