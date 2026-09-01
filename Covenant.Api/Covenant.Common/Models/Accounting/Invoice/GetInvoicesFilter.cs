namespace Covenant.Common.Models.Accounting.Invoice;

public enum GetInvoicesFilterSortBy : byte
{
    InvoiceNumber,
    CreatedAt,
    CompanyFullName,
    SalesRepresentative
}

public class GetInvoicesFilterV2 : Pagination
{
    public string InvoiceNumber { get; set; }
    public DateTime? CreatedAtFrom { get; set; }
    public DateTime? CreatedAtTo { get; set; }
    public string CompanyFullName { get; set; }
    public string SalesRepresentative { get; set; }

    public GetInvoicesFilterSortBy? SortBy { get; set; }
}
