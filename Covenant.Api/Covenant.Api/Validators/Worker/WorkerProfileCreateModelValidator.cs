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
            .MaximumLength(50);
        RuleFor(c => c.MiddleName)
            .MaximumLength(50);
        RuleFor(c => c.LastName)
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(c => c.SecondLastName)
            .MaximumLength(50);
        RuleFor(c => c.BirthDay)
            .NotEmpty()
            .LessThan(DateTime.Now)
            .WithMessage(bd => ValidationMessages.AgeMsg(DateTime.Today.Year - bd.BirthDay.Year));
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
        RuleForEach(c => c.Skills)
            .ChildRules(skill =>
            {
                skill.RuleFor(x => x.Skill)
                    .MaximumLength(20);
            });
        RuleFor(c => c.ContactEmergencyName)
            .MaximumLength(50);
        RuleFor(c => c.ContactEmergencyLastName)
            .MaximumLength(50);
        RuleFor(c => c.ContactEmergencyPhone)
            .Matches(@"^\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}$")
            .WithMessage("Invalid emergency contact phone format");
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
    }
}