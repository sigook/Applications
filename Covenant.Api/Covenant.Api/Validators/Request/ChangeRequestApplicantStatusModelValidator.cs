using Covenant.Common.Enums;
using Covenant.Common.Models.Request;
using FluentValidation;

namespace Covenant.Api.Validators.Request;

public class ChangeRequestApplicantStatusModelValidator : AbstractValidator<ChangeRequestApplicantStatusModel>
{
    public ChangeRequestApplicantStatusModelValidator()
    {
        RuleFor(m => m.Status)
            .IsInEnum()
            .NotEqual(RequestApplicantStatus.Pending);
    }
}
