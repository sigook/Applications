namespace Covenant.Common.Models.Accounting.Invoices;

public class InvoiceListModelWithTotals
{
    public PaginatedList<InvoiceListModel> Detail { get; set; }
    public decimal Total { get; set; }
}
