namespace Covenant.Common.Entities.Accounting.Invoice;

public interface IInvoiceLineItem
{
    double Quantity { get; set; }
    decimal UnitPrice { get; set; }
    string Description { get; set; }
    decimal AgencyRate { get; set; }
    decimal Regular { get; set; }
    decimal Overtime { get; set; }
    decimal Holiday { get; set; }
    decimal Total { get; set; }
    decimal TotalGross { get; set; }
    decimal TotalNet { get; set; }
}
