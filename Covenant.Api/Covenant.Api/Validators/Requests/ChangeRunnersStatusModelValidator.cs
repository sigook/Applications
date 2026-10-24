using Covenant.Common.Enums;
using Covenant.Common.Models.Requests.Runners;
using FluentValidation;

namespace Covenant.Api.Validators.Requests;

public class ChangeRunnersStatusModelValidator : AbstractValidator<ChangeRunnersStatusModel>
{
    public ChangeRunnersStatusModelValidator()
    {
        RuleFor(m => m.RunnerIds)
            .NotEmpty();

        RuleFor(m => m.Status)
            .IsInEnum()
            .NotEqual(RunnerStatus.InterviewRescheduled);

        RuleFor(m => m.StartDate)
            .NotNull()
            .When(m => m.Status == RunnerStatus.Hired);
    }
}
