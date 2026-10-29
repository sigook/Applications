using Covenant.Common.Models.Requests;
using FluentValidation;

namespace Covenant.Api.Validators.Requests;

public class FinishAssignmentModelValidator : AbstractValidator<FinishAssignmentModel>
{
    public FinishAssignmentModelValidator()
    {
        RuleFor(m => m.LastDay).NotEmpty();
        RuleFor(m => m.Reason).IsInEnum();
        RuleFor(m => m.Comments).MaximumLength(500);
    }
}
