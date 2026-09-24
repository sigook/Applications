using Covenant.Common.Models.Worker;
using Covenant.Common.Resources;
using Covenant.Common.Utils.Extensions;
using FluentValidation;

namespace Covenant.Api.Validators.Worker;

public class WorkerProfileLicenseModelValidator : AbstractValidator<WorkerProfileLicenseModel>
{
    public WorkerProfileLicenseModelValidator()
    {
        RuleFor(l => l.License)
            .NotNull()
            .WithMessage(ValidationMessages.RequiredMsg(ApiResources.LicenseFile));
        RuleFor(l => l.License.Description)
            .DocumentDescription()
            .When(l => l.License != null);
    }
}
