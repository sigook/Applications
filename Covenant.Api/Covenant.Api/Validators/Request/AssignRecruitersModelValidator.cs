using Covenant.Common.Models.Request.WeeklyBoard;
using FluentValidation;

namespace Covenant.Api.Validators.Request;

public class AssignRecruitersModelValidator : AbstractValidator<AssignRecruitersModel>
{
    public AssignRecruitersModelValidator()
    {
        RuleFor(m => m.RequestId).NotEmpty();
        RuleFor(m => m.WorkDates).NotEmpty();
        RuleFor(m => m.RecruiterIds).NotEmpty();
    }
}
