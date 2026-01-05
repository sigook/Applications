using Covenant.Common.Utils.Extensions;

namespace Covenant.Common.Entities.Accounting.Invoice;

public class InvoiceUSAItem : IInvoiceLineItem
{
    public InvoiceUSAItem()
    {
    }

    public InvoiceUSAItem(double quantity, decimal unitPrice, string description, Guid id = default)
    {
        Quantity = quantity < 0 ? 0 : quantity;
        UnitPrice = unitPrice < 0 ? 0 : unitPrice;
        Description = description;
        Total = ((decimal)Quantity * UnitPrice).DefaultMoneyRound();
        if (id == default) Id = Guid.NewGuid();
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Description { get; set; }
    public Guid? TimeSheetTotalId { get; set; }
    public decimal AgencyRate { get; set; }
    public decimal Regular { get; set; }
    public decimal OtherRegular { get; set; }
    public decimal Missing { get; set; }
    public decimal MissingOvertime { get; set; }
    public decimal NightShift { get; set; }
    public decimal Holiday { get; set; }
    public decimal Overtime { get; set; }
    public decimal TotalGross { get; set; }
    public decimal TotalNet { get; set; }
    public Request.TimeSheetTotal TimeSheetTotal { get; set; }
    public InvoiceUSA InvoiceUSA { get; private set; }
    public Guid InvoiceUSAId { get; private set; }
    public decimal Total { get; set; }

    public void AssignTo(InvoiceUSA invoice)
    {
        InvoiceUSA = invoice ?? throw new ArgumentNullException(nameof(invoice));
        InvoiceUSAId = invoice.Id;
    }
}