namespace Covenant.Common.Entities.Accounting.Invoice;

public class InvoiceAdditionalDetail
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ClientSiteAddress { get; set; }

    public Guid? CanadaInvoiceId { get; set; }
    public Invoice CanadaInvoice { get; set; }

    public Guid? UsaInvoiceId { get; set; }
    public InvoiceUSA UsaInvoice { get; set; }
}
