using FluentValidation;

namespace Covenant.Api.Shared.WorkerComment.Models
{
    public class CreateValidator : AbstractValidator<CreateCommentModel>
    {
        public CreateValidator()
        {
            RuleFor(m => m.Comment).NotEmpty().MaximumLength(5000);
            RuleFor(m => m.Rate).InclusiveBetween(0, 5);
        }
    }
}