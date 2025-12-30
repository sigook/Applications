using Covenant.Common.Entities.Request;

namespace Covenant.Common.Entities.Accounting.Invoice;

public class InvoiceUSATimeSheetTotal
{
    public Guid InvoiceUSAId { get; set; }        
    public Guid TimeSheetTotalId { get; set; }
    public InvoiceUSA InvoiceUSA { get; set; }
    public TimeSheetTotal TimeSheetTotal { get; set; }
}