namespace Covenant.Common.Models.Accounting.Invoice;

public class GetInvoicesFilter : Pagination
{
    public string Criteria { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
