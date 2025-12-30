using Covenant.Common.Utils.Extensions;

namespace Covenant.Common.Entities.Accounting.Invoice;

public class InvoiceDiscount
{
    private InvoiceDiscount() 
    { 
    }

    public InvoiceDiscount(double quantity, decimal unitPrice, string description, Guid id = default)
    {
        if (unitPrice <= decimal.Zero) throw new ArgumentOutOfRangeException(nameof(unitPrice));
        UnitPrice = unitPrice;
        Quantity = quantity <= 0 ? 1 : quantity;
        Amount = (new decimal(quantity) * UnitPrice).DefaultMoneyRound();
        Description = description;
        Id = id == default ? Guid.NewGuid() : id;
    }

    public Guid Id { get; private set; }
    public double Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Amount { get; private set; }
    public string Description { get; private set; }
    public Invoice Invoice { get; private set; }
    public Guid InvoiceId { get; private set; }

    public void AssignTo(Invoice invoice)
    {
        Invoice = invoice ?? throw new ArgumentNullException();
        InvoiceId = invoice.Id;
    }
}