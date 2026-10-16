namespace Covenant.Common.Entities.Accounting.Invoice;

public class InvoiceAdditionalDetail
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ClientSiteAddress { get; set; }
    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; }
}
