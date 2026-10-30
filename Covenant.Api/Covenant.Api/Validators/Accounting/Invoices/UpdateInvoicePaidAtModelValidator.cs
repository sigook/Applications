using Covenant.Common.Models.Accounting.Invoices;
using FluentValidation;

namespace Covenant.Api.Validators.Accounting.Invoices;

public class UpdateInvoicePaidAtModelValidator : AbstractValidator<UpdateInvoicePaidAtModel>
{
    public UpdateInvoicePaidAtModelValidator()
    {
        RuleFor(m => m.PaidAt)
            .NotNull()
            .WithMessage("Payment date is required");
    }
}
