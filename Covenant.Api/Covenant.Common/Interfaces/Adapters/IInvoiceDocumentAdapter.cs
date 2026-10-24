using Covenant.Common.Models.Accounting.Invoices;

namespace Covenant.Common.Interfaces.Adapters;

public interface IInvoiceDocumentAdapter
{
    InvoiceViewModel MapToInvoiceViewModel(InvoiceSummaryModel model);

    InvoiceEmailViewModel MapToInvoiceEmailViewModel(InvoiceSummaryModel model, string message);
}
