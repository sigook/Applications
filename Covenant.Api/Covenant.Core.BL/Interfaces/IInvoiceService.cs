using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.Invoice;

namespace Covenant.Core.BL.Interfaces;

public interface IInvoiceService
{
    Task<InvoiceListModelWithTotals> GetInvoices(GetInvoicesFilterV2 filter);
    Task<ResultGenerateDocument<byte[]>> GetInvoicesFile(GetInvoicesFilterV2 filter);
    Task<Result<InvoicePreviewModel>> PreviewInvoice(CreateInvoiceModel model);
    Task<Result<Guid>> CreateInvoice(CreateInvoiceModel model);
    Task<InvoiceDocument> GetInvoicePdf(Guid invoiceId);
    Task<Result> SendInvoiceEmail(Guid invoiceId, InvoiceEmailModel model);
    Task DeleteInvoice(Guid invoiceId, DeleteInvoiceModel model);
}
