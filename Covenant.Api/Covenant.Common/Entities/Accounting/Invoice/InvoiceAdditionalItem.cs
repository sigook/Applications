using Covenant.Common.Utils.Extensions;

namespace Covenant.Common.Entities.Accounting.Invoice;

public class InvoiceAdditionalItem
{
    private InvoiceAdditionalItem() 
    { 
    }

    public InvoiceAdditionalItem(double quantity, decimal unitPrice, string description, Guid id = default)
    {
        Quantity = quantity < 0 ? 0 : quantity;
        UnitPrice = unitPrice < 0 ? 0 : unitPrice;
        Description = description;
        if (id == default) Id = Guid.NewGuid();
    }

    public Guid Id { get; private set; }
    public double Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public string Description { get; private set; }
    public Guid InvoiceId { get; private set; }
    public Invoice Invoice { get; set; }
    public decimal Total => ((decimal)Quantity * UnitPrice).DefaultMoneyRound();

    public void AssignTo(Invoice invoice)
    {
        Invoice = invoice ?? throw new ArgumentNullException();
        InvoiceId = invoice.Id;
    }
}