namespace Covenant.Common.Entities.Accounting.Invoice;

public class InvoiceTotal : IInvoiceLineItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid InvoiceId { get; set; }
    public Guid TimeSheetTotalId { get; set; }
    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Description { get; set; }
    public decimal AgencyRate { get; set; }
    public decimal Regular { get; set; }
    public decimal OtherRegular { get; set; }
    public decimal Missing { get; set; }
    public decimal MissingOvertime { get; set; }
    public decimal NightShift { get; set; }
    public decimal Holiday { get; set; }
    public decimal Overtime { get; set; }
    public decimal TotalGross { get; set; }
    public decimal TotalNet { get; set; }
    public decimal Total { get; set; }
    public Request.TimeSheetTotal TimeSheetTotal { get; set; }
    public Invoice Invoice { get; set; }

    public void AssignTo(Invoice invoice)
    {
        Invoice = invoice ?? throw new ArgumentNullException();
        InvoiceId = invoice.Id;
    }

    public decimal RegularPlusOtherRegular => Regular + OtherRegular;
}