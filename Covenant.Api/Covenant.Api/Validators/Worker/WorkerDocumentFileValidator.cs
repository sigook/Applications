using Covenant.Common.Models;
using Covenant.Common.Utils.Extensions;
using FluentValidation;

namespace Covenant.Api.Validators.Worker;

public class WorkerDocumentFileValidator : AbstractValidator<CovenantFileModel>
{
    public WorkerDocumentFileValidator()
    {
        RuleFor(f => f.FileName).NotEmpty();
        RuleFor(f => f.Description).DocumentDescription();
    }
}
