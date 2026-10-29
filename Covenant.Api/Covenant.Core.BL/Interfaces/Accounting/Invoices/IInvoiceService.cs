using Covenant.Common.Functionals;
using Covenant.Common.Models.Accounting.Invoices.Companies;
using Covenant.Common.Models.Accounting.Invoices;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models;

namespace Covenant.Core.BL.Interfaces.Accounting.Invoices;

public interface IInvoiceService
{
    Task<InvoiceListModelWithTotals> GetInvoices(GetInvoicesFilter filter);
    Task<ResultGenerateDocument<byte[]>> GetInvoicesFile(GetInvoicesFilter filter);
    Task<ResultGenerateDocument<byte[]>> GetInvoicesPaymentsReport(GetInvoicesFilter filter);
    Task<int> MarkOverdueInvoices();
    Task<Result<InvoicePreviewModel>> PreviewInvoice(CreateInvoiceModel model);
    Task<Result<Guid>> CreateInvoice(CreateInvoiceModel model);
    Task<PaginatedList<InvoiceListModel>> GetCompanyInvoices(GetCompanyInvoiceFilter filter);
    Task<InvoiceDocument> GetInvoicePdf(Guid invoiceId);
    Task<InvoiceDocument> RegenerateInvoicePdf(Guid invoiceId);
    Task<Result> SendInvoiceEmail(Guid invoiceId, InvoiceEmailModel model);
    Task DeleteInvoice(Guid invoiceId, DeleteInvoiceModel model);
    Task<Result> ChangeInvoiceStatus(Guid invoiceId, ChangeInvoiceStatusModel model);
}
