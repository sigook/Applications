using Covenant.Common.Enums;
using Covenant.Common.Models.Request.Agency;
using Covenant.Common.Models.Request;
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
