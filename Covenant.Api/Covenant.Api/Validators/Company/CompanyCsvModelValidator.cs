using Covenant.Common.Entities;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Resources;
using Covenant.Infrastructure.Repositories;
using FluentValidation;

public class CompanyCsvModelValidator : AbstractValidator<CompanyCsvModel>
{
    public CompanyCsvModelValidator(ICompanyRepository companyRepository, ICatalogRepository catalogRepository, IUserRepository userRepository)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(c => c.CompanyName)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(c => c.Website)
            .MaximumLength(200)
            .Matches(@"^(https?:\/\/)?(www\.)?[a-zA-Z0-9\-]+\.[a-zA-Z]{2,}(?:\.[a-zA-Z]{2,})?(\/[a-zA-Z0-9\-\/]*)?$")
            .When(c => !string.IsNullOrWhiteSpace(c.Website))
            .WithMessage("Invalid website URL");

        RuleFor(c => c.CompanyHQPhone)
            .Matches(@"^\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}$") 
            .When(c => !string.IsNullOrWhiteSpace(c.CompanyHQPhone))
            .WithMessage("Invalid company HQ phone format");

        RuleFor(c => c.Fax)
            .Matches(@"^\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}$")
            .When(c => !string.IsNullOrWhiteSpace(c.Fax))
            .WithMessage("Invalid fax number format");

        RuleFor(c => c.PrimaryIndustry)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Invalid industry");

        RuleFor(c => c.CompanyCity)
            .MaximumLength(100)
            .WithMessage("Invalid City");

        RuleFor(c => c.CompanyState)
            .MaximumLength(100);

        RuleFor(c => c.CompanyZipCode)
            .MaximumLength(20);

        RuleFor(c => c.CompanyCountry)
            .MaximumLength(100);

        RuleFor(c => c.FullAddress)
            .MaximumLength(500);

        RuleFor(c => c.ContactName)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(c => c.ContactEmail)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(100)
            .MustAsync(async (email, cancellationToken) =>
                !await userRepository.UserExists(email))
            .WithMessage(ApiResources.EmailAlreadyTaken);

        RuleFor(c => c.Title)
            .MaximumLength(250);
    }
}
