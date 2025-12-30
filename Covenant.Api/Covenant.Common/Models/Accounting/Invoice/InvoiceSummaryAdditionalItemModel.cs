namespace Covenant.Common.Models.Accounting.Invoice;

public class InvoiceSummaryAdditionalItemModel
{
	public InvoiceSummaryAdditionalItemModel()
	{
	}
	public InvoiceSummaryAdditionalItemModel(double quantity, decimal unitPrice, decimal total, string description)
	{
		Quantity = quantity;
		UnitPrice = unitPrice;
		Total = total;
		Description = description;
	}
	public double Quantity { get; set; }
	public decimal UnitPrice { get; set; }
	public decimal Total { get; set; }
	public string Description { get; set; }
}