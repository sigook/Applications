using Covenant.Common.Entities.Accounting.Subcontractor;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Accounting.Subcontractor;

namespace Covenant.Common.Repositories.Accounting;

public interface ISubcontractorRepository : IBaseRepository<ReportSubcontractor>
{
    Task<List<ReportSubcontractorModel>> GetReportsSubcontractorSummary(DateTime weekEnding);
    Task<PaginatedList<PayrollSubContractorListModel>> GetPayrollsSubcontractor(Guid agencyId, Pagination pagination);
    Task<RegularWageWorker> GetSubcontractorRegularWages(Guid workerProfileId, DateTime holiday, DateTime start, DateTime end, IEnumerable<DateTime> qualifyingDays);
    Task<int> DeleteReportsByWeekEnding(Guid agencyId, DateTime weekEnding);
}
