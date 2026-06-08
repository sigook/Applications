using Covenant.Common.Entities.Company;

namespace Covenant.Common.Entities.Accounting.Invoice;

public class Invoice
{
    public const string PrefixInvoiceNumber = "AI";

    public Guid Id { get; set; } = Guid.NewGuid();
    public long NumberId { get; set; }
    public long InvoiceNumber { get; set; }
    public Guid CompanyId { get; set; }
    public CompanyProfile Company { get; set; }
    public string Email { get; set; }
    public decimal NightShiftRate { get; set; }
    public decimal HolidayRate { get; set; }
    public decimal OverTimeRate { get; set; }
    public decimal VacationsRate { get; set; }
    public decimal HstRate { get; set; }
    public decimal BonusRate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Hst { get; set; }
    public decimal TotalNet { get; set; }
    public IEnumerable<InvoiceTotal> InvoiceTotals { get; set; } = new List<InvoiceTotal>();
    public IEnumerable<InvoiceDiscount> Discounts { get; set; } = new List<InvoiceDiscount>();
    public IEnumerable<InvoiceHoliday> Holidays { get; set; } = new List<InvoiceHoliday>();
    public IEnumerable<InvoiceAdditionalItem> AdditionalItems { get; set; } = new List<InvoiceAdditionalItem>();
    public InvoiceAdditionalDetail AdditionalDetail { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? WeekEnding { get; set; }

    public decimal TotalOvertime => InvoiceTotals.Sum(c => c.Overtime);
    public decimal TotalRegular => InvoiceTotals.Sum(c => c.Regular);

    public void AddInvoiceTotals(IEnumerable<InvoiceTotal> invoiceTotals)
    {
        InvoiceTotals = new List<InvoiceTotal>(invoiceTotals);
        foreach (InvoiceTotal item in InvoiceTotals) item.AssignTo(this);
    }

    public void AddAdditionalItems(IEnumerable<InvoiceAdditionalItem> additionalItems)
    {
        AdditionalItems = new List<InvoiceAdditionalItem>(additionalItems);
        foreach (InvoiceAdditionalItem item in AdditionalItems) item.AssignTo(this);
    }

    public void AddHolidays(IEnumerable<InvoiceHoliday> invoiceHolidays)
    {
        Holidays = new List<InvoiceHoliday>(invoiceHolidays);
        foreach (InvoiceHoliday item in Holidays) item.AssignTo(this);
    }

    public void AddDiscounts(IEnumerable<InvoiceDiscount> discounts)
    {
        Discounts = new List<InvoiceDiscount>(discounts);
        foreach (InvoiceDiscount item in Discounts) item.AssignTo(this);
    }

    public string DisplayInvoiceNumber() => BuildInvoiceNumber(InvoiceNumber, CreatedAt);

    public static string BuildInvoiceNumber(long number, DateTime date) => $"{PrefixInvoiceNumber}-{number:0000}-{date:yy}";
}
