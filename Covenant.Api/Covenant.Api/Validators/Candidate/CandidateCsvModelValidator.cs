using Covenant.Common.Interfaces;
using Covenant.Common.Models.Candidate;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Candidate;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Resources;
using FluentValidation;

namespace Covenant.Api.Validators.Candidate;

public class CandidateCsvModelValidator : AbstractValidator<CandidateCsvModel>
{
    public CandidateCsvModelValidator(
        IUserRepository userRepository, 
        ICandidateRepository candidateRepository,
        IRequestRepository requestRepository,
        ITeamsService teamsService)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(c => c.FullName)
            .NotEmpty()
            .MaximumLength(250);
        RuleFor(c => c.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(100)
            .MustAsync(async (c, cancellationToken) => !await userRepository.UserExists(c))
            .WithMessage(ApiResources.EmailAlreadyTaken);
        RuleFor(c => c.Phone)
            .NotEmpty()
            .Matches(@"^\d{3} \d{3}-\d{4}$")
            .WithMessage("The correct format for phone number is: ### ###-####")
            .MustAsync(async (c, cancellationToken) => !await candidateRepository.CandidatePhonesExists(new string[] { c }))
            .WithMessage("Phone number already exists");
        RuleFor(c => c.Address)
            .NotEmpty()
            .MaximumLength(250);
        RuleFor(c => c.OrderID)
            .Must(c => int.TryParse(c, out var result) && result > 0)
            .When(c => !string.IsNullOrWhiteSpace(c.OrderID))
            .WithMessage("This is not a valid number");
        RuleFor(c => c.Skills)
            .NotEmpty()
            .When(x => string.IsNullOrWhiteSpace(x.OrderID) && string.IsNullOrWhiteSpace(x.UrlResume));
        RuleFor(c => c.UrlResume)
            .MustAsync(async (c, cancellationToken) => await teamsService.ExistsTeamsFile(c))
            .When(c => !string.IsNullOrWhiteSpace(c.UrlResume))
            .WithMessage("File does not exist in teams");
    }
}
