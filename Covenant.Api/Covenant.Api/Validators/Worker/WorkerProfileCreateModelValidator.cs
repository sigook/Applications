using Covenant.Common.Constants;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Resources;
using FluentValidation;

namespace Covenant.Api.Validators.Worker;

public class WorkerProfileCreateModelValidator : AbstractValidator<WorkerProfileCreateModel>
{
    public WorkerProfileCreateModelValidator(
            IUserRepository userRepository,
            IWorkerRepository workerRepository
        )
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(c => c.FirstName)
            .NotEmpty()
            .Length(CovenantConstants.Validation.FirstNameMinLength, CovenantConstants.Validation.FirstNameMaxLength);
        RuleFor(c => c.MiddleName)
            .Length(CovenantConstants.Validation.FirstNameMinLength, CovenantConstants.Validation.FirstNameMaxLength)
            .When(c => !string.IsNullOrEmpty(c.MiddleName));
        RuleFor(c => c.LastName)
            .NotEmpty()
            .Length(CovenantConstants.Validation.LastNameMinLength, CovenantConstants.Validation.LastNameMaxLength);
        RuleFor(c => c.SecondLastName)
            .Length(CovenantConstants.Validation.FirstNameMinLength, CovenantConstants.Validation.FirstNameMaxLength)
            .When(c => !string.IsNullOrEmpty(c.SecondLastName));
        RuleFor(c => c.BirthDay)
            .NotEmpty()
            .LessThanOrEqualTo(_ => DateTime.Now.Date.AddYears(-CovenantConstants.Validation.BirthdayMinimumAge))
            .WithMessage(ValidationMessages.AgeMsg(CovenantConstants.Validation.BirthdayMinimumAge));
        RuleFor(c => c.Gender)
            .NotNull();
        RuleFor(c => c.MobileNumber)
            .NotNull()
            .Matches(@"^\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}$")
            .When(c => !string.IsNullOrWhiteSpace(c.MobileNumber))
            .WithMessage("Invalid mobile phone format");
        RuleFor(c => c.Phone)
            .Matches(@"^\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}$")
            .WithMessage("Invalid phone format");
        RuleFor(c => c.Location)
            .NotNull()
            .WithMessage(ValidationMessages.RequiredMsg(ApiResources.Location));
        RuleFor(c => c.Location.City)
            .NotNull()
            .When(c => c.Location != null);
        RuleFor(c => c.Location.Address)
            .NotEmpty()
            .When(c => c.Location != null);
        RuleFor(c => c.Location.PostalCode)
            .NotEmpty()
            .When(c => c.Location != null);
        RuleForEach(c => c.Skills)
            .ChildRules(skill =>
            {
                skill.RuleFor(x => x.Skill)
                    .MaximumLength(20);
            });
        RuleFor(c => c.ContactEmergencyName)
            .Length(CovenantConstants.Validation.ContactEmergencyMinLength, CovenantConstants.Validation.ContactEmergencyMaxLength)
            .When(c => !string.IsNullOrEmpty(c.ContactEmergencyName));
        RuleFor(c => c.ContactEmergencyLastName)
            .Length(CovenantConstants.Validation.ContactEmergencyMinLength, CovenantConstants.Validation.ContactEmergencyMaxLength)
            .When(c => !string.IsNullOrEmpty(c.ContactEmergencyLastName));
        RuleFor(c => c.ContactEmergencyPhone)
            .Matches(@"^\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}$")
            .WithMessage("Invalid emergency contact phone format");
        RuleFor(c => c.HealthProblem)
            .MaximumLength(CovenantConstants.Validation.HealthProblemMaxLength);
        RuleFor(c => c.OtherHealthProblem)
            .MaximumLength(CovenantConstants.Validation.HealthProblemMaxLength);
        RuleFor(c => c.Email)
            .NotEmpty()
            .EmailAddress()
            .MinimumLength(6)
            .MaximumLength(100)
            .WithMessage("Invalid email format")
            .MustAsync(async (email, cancellation) =>
            {
                return !await userRepository.UserExists(email);
            }).WithMessage("Email already exists");
        RuleFor(c => c.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(100)
            .WithMessage("Password must be between 6 and 100 characters")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[^\s]{6,}$")
            .WithName(ApiResources.Password)
            .WithMessage("Password must be at least 6 characters long and contain at least one uppercase letter, one lowercase letter, and one number. Special characters are optional.");
        RuleFor(c => c.ConfirmPassword)
            .NotEmpty()
            .Equal(c => c.Password)
            .WithName(ApiResources.ConfirmPassword)
            .WithMessage("Passwords do not match");

        RuleFor(c => c.IdentificationNumber1)
            .NotEmpty()
            .MustAsync(async (number, cancellation) =>
            {
                return !await workerRepository.InfoIsAlreadyTaken(
                w => w.IdentificationNumber1 == number);
            }).WithMessage(f => string.Format(ApiResources.IdentificationNumberAlreadyTaken, f.IdentificationNumber1));

        RuleFor(c => c.IdentificationNumber2)
            .MustAsync(async (number, cancellation) =>
            {
                return !await workerRepository.InfoIsAlreadyTaken(
                        w => w.IdentificationNumber2 == number);
            }).WithMessage(ApiResources.IdentificationNumberAlreadyTaken)
            .When(c => !string.IsNullOrEmpty(c.IdentificationNumber2));

        RuleForEach(c => c.Licenses)
            .ChildRules(license =>
            {
                license.RuleFor(l => l.License).NotNull()
                    .WithMessage(ValidationMessages.RequiredMsg(ApiResources.LicenseFile));
                license.RuleFor(l => l.License.FileName).NotEmpty()
                    .When(l => l.License != null)
                    .WithMessage(ValidationMessages.RequiredMsg(ApiResources.LicenseFile));
            });

        RuleForEach(c => c.Certificates)
            .ChildRules(certificate =>
            {
                certificate.RuleFor(c => c.FileName).NotEmpty()
                    .WithMessage(ValidationMessages.RequiredMsg(ApiResources.Certificates));
            });
    }
}