namespace Covenant.Common.Models.Accounting.Invoice;

public class DeleteInvoiceModel
{
    public IEnumerable<Guid> PayStubs { get; set; }
}