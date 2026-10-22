using Covenant.Common.Enums;
using Covenant.Common.Models.Requests.Agencies;
using Covenant.Common.Models.Requests;
using FluentValidation;

namespace Covenant.Api.Validators.Requests;

public class ChangeApplicantsStatusModelValidator : AbstractValidator<ChangeApplicantsStatusModel>
{
    public ChangeApplicantsStatusModelValidator()
    {
        RuleFor(m => m.ApplicantIds)
            .NotEmpty();

        RuleFor(m => m.Status)
            .IsInEnum()
            .NotEqual(RequestApplicantStatus.Pending);
    }
}
