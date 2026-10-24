using Covenant.Common.Enums;
using Covenant.Common.Models.Accounting.Invoices;
using FluentValidation;

namespace Covenant.Api.Validators.Accounting.Invoices;

public class ChangeInvoiceStatusModelValidator : AbstractValidator<ChangeInvoiceStatusModel>
{
    public ChangeInvoiceStatusModelValidator()
    {
        RuleFor(m => m.Status)
            .IsInEnum()
            .NotEqual(InvoiceStatus.Due)
            .WithMessage("Due is assigned automatically when the payment term expires");

        RuleFor(m => m.PaidAt)
            .Null()
            .When(m => m.Status != InvoiceStatus.Paid)
            .WithMessage("Payment date only applies when marking the invoice as Paid");
    }
}
