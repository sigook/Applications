namespace Covenant.Common.Models.Accounting.Invoice;

public class InvoiceSummaryDiscountModel
{
	public decimal Amount { get; set; }
	public string Description { get; set; }
	public double Quantity { get; set; }
	public decimal UnitPrice { get; set; }
}