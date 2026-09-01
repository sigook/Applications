using Covenant.Common.Models.Agency;
using FluentValidation;

namespace Covenant.Api.Validators.Agency;

public class AgencyPersonnelModelValidator : AbstractValidator<AgencyPersonnelModel>
{
    public AgencyPersonnelModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(20);
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(50);
        RuleFor(x => x.Role)
            .NotEmpty();
    }
}
