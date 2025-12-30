using Covenant.Common.Utils.Extensions;

namespace Covenant.Common.Entities.Accounting.Invoice;

public class InvoiceUSADiscount
{
    private InvoiceUSADiscount()
    {
    }

    public InvoiceUSADiscount(double quantity, decimal unitPrice, string description, Guid id = default)
    {
        UnitPrice = unitPrice;
        Quantity = quantity <= 0 ? 1 : quantity;
        Total = (new decimal(quantity) * UnitPrice).DefaultMoneyRound();
        Description = description;
        InvoiceUSAId = id == default ? Guid.NewGuid() : id;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public double Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Total { get; private set; }
    public string Description { get; private set; }
    public InvoiceUSA InvoiceUSA { get; private set; }
    public Guid InvoiceUSAId { get; private set; }

    public void AssignTo(InvoiceUSA invoice)
    {
        InvoiceUSA = invoice ?? throw new ArgumentNullException(nameof(invoice));
        InvoiceUSAId = invoice.Id;
    }
}