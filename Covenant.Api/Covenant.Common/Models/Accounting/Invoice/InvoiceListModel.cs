using Covenant.Common.Enums;

namespace Covenant.Common.Models.Accounting.Invoice;

public class InvoiceListModel
{
    private string _invoiceNumber;

    public Guid Id { get; set; }
    public long NumberId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string AgencyFullName { get; set; }
    public string CompanyFullName { get; set; }
    public string Email { get; set; }
    public decimal TotalNet { get; set; }
    public long InvoiceNumberId { get; set; }
    public DateTime? WeekEnding { get; set; }
    public Guid CompanyProfileId { get; set; }
    public string SalesRepresentative { get; set; }
    public InvoiceStatus Status { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string UpdatedByName { get; set; }
    public string InvoiceNumber
    {
        get =>
            string.IsNullOrEmpty(_invoiceNumber) ? Entities.Accounting.Invoice.Invoice.BuildInvoiceNumber(InvoiceNumberId, CreatedAt)
                : _invoiceNumber;
        set => _invoiceNumber = value;
    }
}
