namespace Covenant.Common.Models.Accounting.Invoice;

public class DeleteInvoiceModel
{
    public string VerificationCode { get; set; }
    public IEnumerable<Guid> PayStubs { get; set; }
}