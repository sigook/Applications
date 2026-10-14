using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Models.Accounting.Invoice.Company;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Company;

namespace Covenant.Common.Repositories.Accounting.Invoices;

public interface IInvoiceRepository : IBaseRepository
{
    Task<List<InvoiceListModel>> GetAllInvoicesForAgency(IEnumerable<Guid> agencyIds, GetInvoicesFilter filter);
    Task<InvoiceListModelWithTotals> GetInvoicesForAgency(IEnumerable<Guid> agencyIds, GetInvoicesFilter filter);
    Task<Invoice> GetInvoiceForAgency(IEnumerable<Guid> agencyIds, Guid invoiceId);
    Task<IReadOnlyList<string>> MarkPendingInvoicesAsDue(DateTime today);
    Task<(Guid InvoiceId, string InvoiceNumber)> DeleteInvoiceAndReportsSubcontractor(Guid invoiceId);
    Task<InvoiceSummaryModel> GetInvoiceSummaryById(Guid id);
    Task<InvoiceSummaryModel> GetInvoiceSummaryForCompany(Guid companyId, Guid id);
    Task<PaginatedList<InvoiceListModel>> GetInvoicesForCompany(Guid companyId, GetCompanyInvoiceFilter filter);
    Task<NextNumberModel> GetNextInvoiceNumber(Guid agencyId);
    Task<List<CompanyRegularChargesByWorker>> GetCompanyRegularCharges(Guid companyProfileId, DateTime holiday, DateTime start, DateTime end, IEnumerable<DateTime> qualifyingDays);
}
