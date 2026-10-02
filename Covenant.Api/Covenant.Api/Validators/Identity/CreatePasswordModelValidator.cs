using Covenant.Common.Models.Identity;
using FluentValidation;

namespace Covenant.Api.Validators.Identity;

public class CreatePasswordModelValidator : AbstractValidator<CreatePasswordModel>
{
    public CreatePasswordModelValidator()
    {
        RuleFor(m => m.Id).NotEmpty();
        RuleFor(m => m.Token).NotEmpty();
        RuleFor(m => m.Password).NotEmpty().MinimumLength(6);
    }
}
