using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Accounting.Subcontractor;

namespace Covenant.Core.BL.Interfaces;

public interface IAccountingService
{
    Task<InvoiceListModelWithTotals> GetInvoices(GetInvoicesFilterV2 filter);
    Task<ResultGenerateDocument<byte[]>> GetInvoicesFile(GetInvoicesFilterV2 filter);
    Task<Result<InvoicePreviewModel>> PreviewInvoice(CreateInvoiceModel model);
    Task<Result<Guid>> CreateInvoice(CreateInvoiceModel model);
    Task<PaginatedList<PayStubListModel>> GetPayStubs(GetPayStubsFilter filter);
    Task<ResultGenerateDocument<byte[]>> GetPayStubsFile(GetPayStubsFilter filter);
    Task DeletePayStub(Guid payStubId);
    Task<Result> GeneratePayStubs(IEnumerable<Guid> workerIds);
    Task<PaginatedList<WeeklyPayrollModel>> GetWeeklyPayrollGroupByPaymentDate(Pagination pagination);
    Task<Result<ResultGenerateDocument<byte[]>>> GetWeeklyPayrollGroupByPaymentDateFile(string weekEnding);
    Task<PaginatedList<PayrollSubContractorListModel>> GetSubcontractors(Pagination filter);
    Task<Result<ResultGenerateDocument<byte[]>>> GetSubcontractorFile(string weekEnding);
    Task<Result> CreateSkipPayrollNumber(BaseModel<Guid> model);
}
