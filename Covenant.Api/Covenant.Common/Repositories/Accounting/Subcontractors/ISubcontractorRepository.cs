using Covenant.Common.Models.Accounting.PayStubs;
using Covenant.Common.Models.Accounting.Subcontractors;
using Covenant.Common.Models;

namespace Covenant.Common.Repositories.Accounting.Subcontractors;

public interface ISubcontractorRepository : IBaseRepository
{
    Task<List<ReportSubcontractorModel>> GetReportsSubcontractorSummary(DateTime weekEnding);
    Task<PaginatedList<PayrollSubContractorListModel>> GetPayrollsSubcontractor(IEnumerable<Guid> agencyIds, Pagination pagination);
    Task<RegularWageWorker> GetSubcontractorRegularWages(Guid workerProfileId, DateTime holiday, DateTime start, DateTime end, IEnumerable<DateTime> qualifyingDays);
    Task<int> DeleteReportsByWeekEnding(IEnumerable<Guid> agencyIds, DateTime weekEnding);
}
