namespace Covenant.Common.Models.Accounting;

public class CreateInvoiceItemModel
{
    public CreateInvoiceItemModel(double quantity, decimal unitPrice, string description)
    {
        Quantity = quantity;
        UnitPrice = unitPrice;
        Description = description;
    }

    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Description { get; set; }
    public decimal Total => new decimal(Quantity) * UnitPrice;
}
