using Covenant.Common.Models.Company;
using Covenant.Common.Repositories;
using Covenant.Common.Resources;
using Covenant.Infrastructure.Repositories;
using FluentValidation;

namespace Covenant.Api.Validators.Company
{
    public class CompanyProfileDetailValidator : AbstractValidator<CompanyProfileDetailModel>
    {
        public CompanyProfileDetailValidator(IUserRepository userRepository)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(c => c.BusinessName);

            RuleFor(c => c)
                .Must(c => !string.IsNullOrWhiteSpace(c.BusinessName) || !string.IsNullOrWhiteSpace(c.FullName))
                .WithMessage("Either BusinessName or FullName must be provided.");

            RuleFor(c => c.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(100)
                .MustAsync(async (email, cancellationToken) =>
                    !await userRepository.UserExists(email))
                        .WithMessage(ApiResources.EmailAlreadyTaken);

            RuleFor(c => c.Industry)
                .NotNull()
                .Must(c =>  c.Industry.Id != Guid.Empty);

            RuleFor(c=> c.Website)
                .MaximumLength(100)
                .Matches(@"^(https?:\/\/)?(www\.)?[a-zA-Z0-9\-]+\.[a-zA-Z]{2,}(?:\.[a-zA-Z]{2,})?(\/[a-zA-Z0-9\-\/]*)?$")
                .WithMessage("Website must be empty and be a valid URL.");

        }
    }
}
