using Covenant.Common.Models.Request;
using FluentValidation;

namespace Covenant.Api.Validators.Request;

public class CompleteApplicantComplianceItemModelValidator : AbstractValidator<CompleteApplicantComplianceItemModel>
{
    private const int MaximumLengthFileName = 500;
    private const int MinimumLengthIdentificationNumber = 5;
    private const int MaximumLengthIdentificationNumber = 15;
    private const int MinimumLengthSocialInsuranceNumber = 9;
    private const int MaximumLengthSocialInsuranceNumber = 15;

    public CompleteApplicantComplianceItemModelValidator()
    {
        RuleFor(m => m.FileName)
            .MaximumLength(MaximumLengthFileName);
        RuleFor(m => m.IdentificationNumber)
            .Length(MinimumLengthIdentificationNumber, MaximumLengthIdentificationNumber)
            .When(m => !string.IsNullOrEmpty(m.IdentificationNumber));
        RuleFor(m => m.SocialInsuranceNumber)
            .Length(MinimumLengthSocialInsuranceNumber, MaximumLengthSocialInsuranceNumber)
            .When(m => !string.IsNullOrEmpty(m.SocialInsuranceNumber));
    }
}
