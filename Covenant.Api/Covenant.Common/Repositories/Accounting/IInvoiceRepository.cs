using Covenant.Common.Models;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Company;

namespace Covenant.Common.Repositories.Accounting;

public interface IInvoiceRepository
{
    Task Create<T>(T entity) where T : class;
    Task<List<InvoiceListModel>> GetAllInvoicesForAgency(IEnumerable<Guid> agencyIds, GetInvoicesFilterV2 filter);
    Task<List<InvoiceListModel>> GetAllInvoicesUSAForAgency(IEnumerable<Guid> agencyIds, GetInvoicesFilterV2 filter);
    Task<InvoiceListModelWithTotals> GetInvoicesForAgency(IEnumerable<Guid> agencyIds, GetInvoicesFilterV2 filter);
    Task<InvoiceListModelWithTotals> GetInvoicesUSAForAgency(IEnumerable<Guid> agencyIds, GetInvoicesFilterV2 filter);
    Task<(Guid InvoiceId, string InvoiceNumber)> DeleteInvoiceAndReportsSubcontractor(Guid invoiceId);
    Task<(Guid InvoiceId, string numberId)> DeleteInvoiceUSA(Guid invoiceId);
    Task SaveChangesAsync();
    Task<InvoiceSummaryModel> GetInvoiceSummaryById(Guid id);
    Task<InvoiceSummaryModel> GetInvoiceUSASummaryById(Guid id);
    Task<PaginatedList<InvoiceListModel>> GetInvoicesForCompany(Guid companyId, GetCompanyInvoiceFilter filter);
    Task<PaginatedList<InvoiceListModel>> GetInvoicesForCompanyUSA(Guid companyId, GetCompanyInvoiceFilter filter);
    Task<NextNumberModel> GetNextInvoiceNumber();
    Task<NextNumberModel> GetNextInvoiceUSANumber();
    Task<List<CompanyRegularChargesByWorker>> GetCompanyRegularCharges(ParamsToGetRegularWages p);
}