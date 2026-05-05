using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Models;

namespace Covenant.Common.Entities.Accounting.Invoice;

public class InvoiceUSA
{
    public const string PrefixInvoiceNumber = "US";

    public Guid Id { get; set; } = Guid.NewGuid();
    public long NumberId { get; set; }
    public long InvoiceNumberId { get; set; }
    public DateTime? WeekEnding { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CompanyProfileId { get; set; }
    public CompanyProfile CompanyProfile { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Tax { get; set; }
    public decimal TotalNet { get; set; }
    public string HtmlNotes { get; set; }
    public string BillToEmail { get; set; }
    public string BillToAddress { get; set; }
    public string BillToPhone { get; set; }
    public string BillToFax { get; set; }
    public string BillFromAddress { get; set; }
    public string BillFromPhone { get; set; }
    public string BillFromFax { get; set; }
    public string InvoiceNumber { get; set; }
    public IEnumerable<InvoiceUSAItem> Items { get; set; } = new List<InvoiceUSAItem>();
    public IEnumerable<InvoiceUSADiscount> Discounts { get; set; } = new List<InvoiceUSADiscount>();
    public IEnumerable<InvoiceUSATimeSheetTotal> TimeSheetTotals { get; set; } = new List<InvoiceUSATimeSheetTotal>();
    public InvoiceAdditionalDetail AdditionalDetail { get; set; }

    public void AddTimesheetTotals(IEnumerable<ITimeSheetTotal> totals)
    {
        TimeSheetTotals = new List<InvoiceUSATimeSheetTotal>(totals.Select(s => new InvoiceUSATimeSheetTotal
        {
            InvoiceUSA = this,
            TimeSheetTotal = new TimeSheetTotal(s)
        }));
    }
}