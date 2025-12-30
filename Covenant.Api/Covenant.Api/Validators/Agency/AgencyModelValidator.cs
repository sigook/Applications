using Covenant.Common.Models.Agency;
using Covenant.Common.Repositories;
using FluentValidation;

namespace Covenant.Api.Validators.Agency;

public class AgencyModelValidator : AbstractValidator<AgencyModel>
{
    public AgencyModelValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(async (x, token) => !await userRepository.UserExists(x))
            .WithMessage("Email already exists");
    }
}
