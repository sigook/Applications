using Covenant.Common.Configuration;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Core.BL.Interfaces.Accounting.Invoices;

namespace Covenant.Core.BL.Services.Accounting.Invoices;

public class CanadaInvoiceStrategy(Rates rates) : IInvoiceCountryStrategy
{
    public bool IsUSA => false;
    public string NumberPrefix => "AI";
    public string TaxName => "HST";
    public bool BillsUnworkedHolidays => true;

    public decimal CalculateTax(decimal subTotal, decimal timesheetsTax, CreateInvoiceModel model) => subTotal * rates.Hst;

    public void Decorate(InvoiceSummaryModel summary)
    {
        summary.TaxName = TaxName;
        summary.InvoiceColor = InvoiceColor.Covenant;
        summary.InvoicePayroll = InvoicePayroll.Covenant;
    }
}
