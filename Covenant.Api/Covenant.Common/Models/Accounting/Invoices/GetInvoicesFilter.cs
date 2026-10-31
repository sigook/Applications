using Covenant.Common.Enums;

namespace Covenant.Common.Models.Accounting.Invoices;

public enum GetInvoicesFilterSortBy : byte
{
    InvoiceNumber,
    CreatedAt,
    CompanyFullName,
    Status
}

public class GetInvoicesFilter : Pagination
{
    public string InvoiceNumber { get; set; }
    public DateTime? CreatedAtFrom { get; set; }
    public DateTime? CreatedAtTo { get; set; }
    public DateTime? DueDateFrom { get; set; }
    public DateTime? DueDateTo { get; set; }
    public DateTime? PaidAtFrom { get; set; }
    public DateTime? PaidAtTo { get; set; }
    public string CompanyFullName { get; set; }
    public InvoiceStatus? Status { get; set; }

    public GetInvoicesFilterSortBy? SortBy { get; set; }
}
