using Covenant.Common.Models.Accounting.Deductions;
using FluentValidation;

namespace Covenant.Api.Validators.Deduction;

public class ImportCraTableFromBlobModelValidator : AbstractValidator<ImportCraTableFromBlobModel>
{
    public const int MinimumYear = 2000;
    public const int MaximumYear = 2100;

    public ImportCraTableFromBlobModelValidator()
    {
        RuleFor(m => m.BlobName).NotEmpty()
            .Must(name => name.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
            .WithMessage("Only PDF files can be imported");
        RuleFor(m => m.PayPeriod).IsInEnum();
        RuleFor(m => m.Year).InclusiveBetween(MinimumYear, MaximumYear);
    }
}
