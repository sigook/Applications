using Covenant.Common.Models.Accounting.Invoice;
using FluentValidation;

namespace Covenant.Api.Validators.Accounting.Invoices;

public class ChangeInvoiceStatusModelValidator : AbstractValidator<ChangeInvoiceStatusModel>
{
    public ChangeInvoiceStatusModelValidator()
    {
        RuleFor(m => m.Status).IsInEnum();
    }
}
