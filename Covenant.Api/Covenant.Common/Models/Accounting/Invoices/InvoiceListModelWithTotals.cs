namespace Covenant.Common.Models.Accounting.Invoice;

public class InvoiceListModelWithTotals
{
    public PaginatedList<InvoiceListModel> Detail { get; set; }
    public decimal Total { get; set; }
}
