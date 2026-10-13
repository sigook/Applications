using Covenant.Common.Enums;

namespace Covenant.Common.Models.Accounting.Invoice;

public class InvoiceListModel
{
    public Guid Id { get; set; }
    public long NumberId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string AgencyFullName { get; set; }
    public string CompanyFullName { get; set; }
    public string Email { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Tax { get; set; }
    public decimal TotalNet { get; set; }
    public long InvoiceNumberId { get; set; }
    public string InvoiceNumber { get; set; }
    public DateTime? WeekEnding { get; set; }
    public Guid CompanyProfileId { get; set; }
    public string SalesRepresentative { get; set; }
    public InvoiceStatus Status { get; set; }
    public int PaymentTermDays { get; set; }
    public DateTime DueDate => CreatedAt.Date.AddDays(PaymentTermDays);
    public int DueDays { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string UpdatedByName { get; set; }

    public void SetDueDays(DateTime today) =>
        DueDays = Status == InvoiceStatus.Paid ? 0 : Math.Max(0, (today.Date - DueDate).Days);
}
