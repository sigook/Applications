using Covenant.Common.Enums;

namespace Covenant.Common.Models.Accounting.Invoices;

public class ChangeInvoiceStatusModel
{
    public InvoiceStatus Status { get; set; }
    public DateTime? PaidAt { get; set; }
}
