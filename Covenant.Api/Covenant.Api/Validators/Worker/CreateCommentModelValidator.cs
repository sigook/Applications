using Covenant.Common.Models.Worker;
using FluentValidation;

namespace Covenant.Api.Validators.Worker;

public class CreateCommentModelValidator : AbstractValidator<CreateCommentModel>
{
    public CreateCommentModelValidator()
    {
        RuleFor(m => m.Comment).NotEmpty().MaximumLength(5000);
        RuleFor(m => m.Rate).InclusiveBetween(0, 5);
    }
}
