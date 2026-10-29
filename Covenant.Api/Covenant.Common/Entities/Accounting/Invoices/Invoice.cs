using Covenant.Common.Entities.Companies;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;

namespace Covenant.Common.Entities.Accounting.Invoices;

public class Invoice
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public long NumberId { get; private set; }
    public long InvoiceNumberId { get; internal set; }
    public string InvoiceNumber { get; internal set; }
    public Guid CompanyProfileId { get; internal set; }
    public CompanyProfile CompanyProfile { get; private set; }
    public string Email { get; internal set; }
    public decimal SubTotal { get; internal set; }
    public decimal Tax { get; internal set; }
    public decimal TotalNet { get; internal set; }
    public IEnumerable<InvoiceTotal> InvoiceTotals { get; private set; } = new List<InvoiceTotal>();
    public IEnumerable<InvoiceDiscount> Discounts { get; private set; } = new List<InvoiceDiscount>();
    public IEnumerable<InvoiceHoliday> Holidays { get; private set; } = new List<InvoiceHoliday>();
    public IEnumerable<InvoiceAdditionalItem> AdditionalItems { get; private set; } = new List<InvoiceAdditionalItem>();
    public InvoiceAdditionalDetail AdditionalDetail { get; private set; }
    public DateTime CreatedAt { get; internal set; } = DateTime.Now;
    public DateTime? WeekEnding { get; internal set; }
    public InvoiceStatus Status { get; private set; } = InvoiceStatus.Pending;
    public DateTime? PaidAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public User UpdatedByUser { get; private set; }
    public DateTime? PdfGeneratedAt { get; internal set; }

    public decimal TotalOvertime => InvoiceTotals.Sum(c => c.Overtime);
    public decimal TotalRegular => InvoiceTotals.Sum(c => c.Regular);

    public Result ChangeStatus(InvoiceStatus next, Guid changedBy, DateTime now, DateTime? paidAt = null)
    {
        if (Status == next) return Result.Fail($"Invoice is already {next}");
        Status = next;
        PaidAt = next == InvoiceStatus.Paid ? paidAt ?? now : null;
        UpdatedAt = now;
        UpdatedBy = changedBy;
        return Result.Ok();
    }

    public Result UpdatePaidAt(DateTime paidAt, Guid changedBy, DateTime now)
    {
        if (Status != InvoiceStatus.Paid) return Result.Fail("Only paid invoices have a payment date");
        PaidAt = paidAt;
        UpdatedAt = now;
        UpdatedBy = changedBy;
        return Result.Ok();
    }

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

    public void SetAdditionalDetail(string clientSiteAddress)
    {
        if (string.IsNullOrWhiteSpace(clientSiteAddress)) return;
        AdditionalDetail = new InvoiceAdditionalDetail
        {
            ClientSiteAddress = clientSiteAddress,
            InvoiceId = Id,
            Invoice = this
        };
    }

    public static Invoice Create(Guid companyProfileId, long invoiceNumberId, string invoiceNumber, string email, decimal subTotal, decimal tax, DateTime createdAt, DateTime? weekEnding) =>
        new()
        {
            CompanyProfileId = companyProfileId,
            InvoiceNumberId = invoiceNumberId,
            InvoiceNumber = invoiceNumber,
            Email = email,
            SubTotal = subTotal,
            Tax = tax,
            TotalNet = subTotal + tax,
            CreatedAt = createdAt,
            WeekEnding = weekEnding
        };
}
