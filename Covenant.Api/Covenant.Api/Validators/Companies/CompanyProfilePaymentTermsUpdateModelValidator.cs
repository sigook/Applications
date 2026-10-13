using Covenant.Common.Models.Company.Agency;
using FluentValidation;

namespace Covenant.Api.Validators.Companies;

public class CompanyProfilePaymentTermsUpdateModelValidator : AbstractValidator<CompanyProfilePaymentTermsUpdateModel>
{
    public const int MinimumDays = 1;
    public const int MaximumDays = 365;

    public CompanyProfilePaymentTermsUpdateModelValidator()
    {
        RuleFor(m => m.PaymentTermDays).InclusiveBetween(MinimumDays, MaximumDays);
    }
}
