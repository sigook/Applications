using Covenant.Common.Enums;
using Covenant.Common.Models.Requests.Agencies;
using Covenant.Common.Models.Requests;
using FluentValidation;

namespace Covenant.Api.Validators.Requests;

public class ChangeRequestApplicantStatusModelValidator : AbstractValidator<ChangeRequestApplicantStatusModel>
{
    public ChangeRequestApplicantStatusModelValidator()
    {
        RuleFor(m => m.Status)
            .IsInEnum()
            .NotEqual(RequestApplicantStatus.Pending);
    }
}
