namespace Covenant.Common.Models.Accounting.Invoices;

public class DeleteInvoiceModel
{
    public IEnumerable<Guid> PayStubs { get; set; }
}