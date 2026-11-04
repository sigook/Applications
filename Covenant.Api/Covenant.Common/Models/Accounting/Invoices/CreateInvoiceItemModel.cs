namespace Covenant.Common.Models.Accounting.Invoices;

public class CreateInvoiceItemModel(double quantity, decimal unitPrice, string description)
{
    public double Quantity { get; set; } = quantity;
    public decimal UnitPrice { get; set; } = unitPrice;
    public string Description { get; set; } = description;
    public decimal Total => new decimal(Quantity) * UnitPrice;
}
