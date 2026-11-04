namespace Covenant.Common.Models.Accounting.Invoices;

public class InvoicePreviewModel
{
    public decimal SubTotal { get; set; }
    public decimal Tax { get; set; }
    public string TaxName { get; set; }
    public decimal Total { get; set; }
    public List<InvoiceSummaryItemModel> Items { get; set; } = [];
    public List<InvoiceSummaryItemModel> Discounts { get; set; } = [];
}
