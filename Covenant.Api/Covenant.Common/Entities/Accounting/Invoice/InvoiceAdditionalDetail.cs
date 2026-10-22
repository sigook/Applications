namespace Covenant.Common.Entities.Accounting.Invoice;

public class InvoiceAdditionalDetail
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string ClientSiteAddress { get; internal set; }
    public Guid InvoiceId { get; internal set; }
    public Invoice Invoice { get; internal set; }
}
