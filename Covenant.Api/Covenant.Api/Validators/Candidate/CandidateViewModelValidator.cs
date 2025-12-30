using Covenant.Common.Models.WebSite;
using Covenant.Common.Repositories;
using Covenant.Common.Resources;
using Covenant.Infrastructure.Repositories;
using FluentValidation;

namespace Covenant.Api.Validators.Candidate;

public class CandidateViewModelValidator : AbstractValidator<CandidateViewModel>
{
    public CandidateViewModelValidator(
            IUserRepository workerRepository
        )
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(c => c.FullName)
            .MaximumLength(60);
        RuleFor(c => c.Email)
            .NotEmpty()
            .EmailAddress()
            .MinimumLength(6)
            .MaximumLength(100);
        RuleFor(c => c.Phone)
            .Matches(@"^\d{3} \d{3}-\d{4}$")
            .WithMessage("The correct format for phone number is: ### ###-####");
        RuleFor(c => c.Address)
            .MaximumLength(100);
        RuleFor(c => c.Skills)
            .ForEach(skill =>
        skill
            .MaximumLength(20));
    }
}