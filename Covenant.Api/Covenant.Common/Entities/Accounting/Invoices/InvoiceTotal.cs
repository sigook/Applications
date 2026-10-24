using Covenant.Common.Entities.Requests;
namespace Covenant.Common.Entities.Accounting.Invoices;

public class InvoiceTotal
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid InvoiceId { get; private set; }
    public Guid TimeSheetTotalId { get; internal set; }
    public double Quantity { get; internal set; }
    public decimal UnitPrice { get; internal set; }
    public string Description { get; internal set; }
    public decimal AgencyRate { get; internal set; }
    public decimal Regular { get; private set; }
    public decimal OtherRegular { get; private set; }
    public decimal Missing { get; private set; }
    public decimal MissingOvertime { get; private set; }
    public decimal NightShift { get; private set; }
    public decimal Holiday { get; private set; }
    public decimal Overtime { get; private set; }
    public decimal TotalGross { get; private set; }
    public decimal TotalNet { get; private set; }
    public decimal Total { get; internal set; }
    public TimeSheetTotal TimeSheetTotal { get; private set; }
    public Invoice Invoice { get; private set; }

    public void AssignTo(Invoice invoice)
    {
        Invoice = invoice ?? throw new ArgumentNullException();
        InvoiceId = invoice.Id;
    }

    public decimal RegularPlusOtherRegular => Regular + OtherRegular;

    public static InvoiceTotal Create(TimeSheetTotal timeSheetTotal, double quantity, decimal unitPrice, string description, decimal agencyRate, decimal amount) =>
        new()
        {
            TimeSheetTotalId = timeSheetTotal.Id,
            TimeSheetTotal = timeSheetTotal,
            Quantity = quantity,
            UnitPrice = unitPrice,
            Description = description,
            AgencyRate = agencyRate,
            Total = amount,
            TotalGross = amount,
            TotalNet = amount
        };

    public void ChargeRegular(decimal amount) => Regular = amount;

    public void ChargeOvertime(decimal amount) => Overtime = amount;

    public void ChargeHoliday(decimal amount) => Holiday = amount;

    public void ChargeMissing(decimal amount) => Missing = amount;

    public void ChargeMissingOvertime(decimal amount) => MissingOvertime = amount;
}
