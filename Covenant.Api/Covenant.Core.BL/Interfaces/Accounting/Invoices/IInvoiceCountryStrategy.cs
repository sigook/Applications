using Covenant.Common.Models.Accounting.Invoice;

namespace Covenant.Core.BL.Interfaces.Accounting.Invoices;

public interface IInvoiceCountryStrategy
{
    bool IsUSA { get; }
    string NumberPrefix { get; }
    string TaxName { get; }
    bool BillsUnworkedHolidays { get; }
    decimal CalculateTax(decimal subTotal, decimal timesheetsTax, CreateInvoiceModel model);
    void Decorate(InvoiceSummaryModel summary);
    string BuildInvoiceNumber(long number, DateTime date) => $"{NumberPrefix}-{number:0000}-{date:yy}";
}
