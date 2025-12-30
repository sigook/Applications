namespace Covenant.Common.Models.Accounting.Invoice;

public class InvoicePreviewModel
{
    public decimal SubTotal { get; set; }
    public decimal Hst { get; set; }
    public decimal Total { get; set; }
    public List<InvoiceSummaryItemModel> Items { get; set; } = new List<InvoiceSummaryItemModel>();
    public List<InvoiceSummaryItemModel> Discounts { get; set; } = new List<InvoiceSummaryItemModel>();
}