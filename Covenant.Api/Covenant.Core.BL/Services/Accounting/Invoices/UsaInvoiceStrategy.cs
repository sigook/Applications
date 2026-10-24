using Covenant.Common.Models.Accounting.Invoices;
using Covenant.Common.Models.Accounting;
using Covenant.Core.BL.Interfaces.Accounting.Invoices;

namespace Covenant.Core.BL.Services.Accounting.Invoices;

public class UsaInvoiceStrategy : IInvoiceCountryStrategy
{
    public bool IsUSA => true;
    public string NumberPrefix => "US";
    public string TaxName => "Tax";
    public bool BillsUnworkedHolidays => false;

    public decimal CalculateTax(decimal subTotal, decimal timesheetsTax, CreateInvoiceModel model) =>
        model.DirectHiring ? subTotal * (model.TaxPercentage ?? 0m) / 100m : timesheetsTax;

    public void Decorate(InvoiceSummaryModel summary)
    {
        summary.TaxName = TaxName;
        summary.HstNumber = null;
        summary.InvoiceColor = InvoiceColor.Sigook;
        summary.InvoicePayroll = InvoicePayroll.Sigook;
    }
}
