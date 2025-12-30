using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Request.TimeSheet;
using System.Linq.Expressions;

namespace Covenant.Common.Repositories.Request;

public interface ITimeSheetRepository
{
    Task SaveChangesAsync();
    Task Create<T>(T entity) where T : class;
    void Delete<T>(T entity) where T : class;
    Task<PaginatedList<TimeSheetListModel>> GetTimeSheetsForWorker(Guid workerId, Guid requestId, Pagination pagination);
    Task<TimeSheet> GetTimeSheet(Guid id);
    Task<IEnumerable<TimeSheet>> GetTimeSheets(Guid workerId, Guid requestId, Expression<Func<Common.Entities.Request.Request, bool>> requestCondition);
    Task<IEnumerable<TimeSheetListModel>> GetTimeSheetsListModel(Guid workerId, Guid requestId, DateTime? startDate, DateTime? endDate);
    Task<List<TimeSheetApprovedBillingModel>> GetTimeSheetForCreatingInvoice(IEnumerable<Guid> agencyIds, CreateInvoiceModel model);
    Task<List<TimeSheetApprovedPayrollModel>> GetTimeSheetForCreatingReportsSubcontractor(IEnumerable<Guid> agencyIds, Guid companyId);
    Task<List<TimeSheetApprovedPayrollModel>> GetTimeSheetForCreatingPayStubs(IEnumerable<Guid> agencyIds, Guid workerId);
    Task<PaginatedList<TimeSheetHistoryModel>> GetTimeSheetHistory(Guid workerProfileId, Pagination pagination);
    Task<TimesheetHistoryAccumulated> GetTimesheetHistoryAccumulated(Guid workerProfileId, int rowNumber);
    Task<List<WorkerReadyForPayStubModel>> GetWorkersReadyForPayStub(IEnumerable<Guid> agencyIds);
    Task<TimeSheetUsagesModel> GetTimeSheetUsages(Guid requestId, Guid workerId, Guid id);
    Task<IEnumerable<HoursWorkedResponse>> GetHoursWorked(Guid agencyId, HoursWorkedFilter filter);
    Task<IEnumerable<CompanyProfileJobPositionRateModel>> GetJobPositions(Guid companyId, DateTime startDate, DateTime endDate);
    Task<TimeSheet> GetLatestTimesheet(Expression<Func<WorkerProfile, bool>> condition);
    Task<TimeSheet> GetLatestTimesheet(Guid workerId, Guid requestId, DateTime from);
    Task<TimeSheet> GetTimeSheeFromTheLast14Hours(Guid workerId, Guid requestId, DateTime now);
    Task<bool> TimesheetUsedByAccounting(Guid id);
    Task<List<RequestTimeSheetModel>> GetRequestTimeSheet(Guid requestId);
}