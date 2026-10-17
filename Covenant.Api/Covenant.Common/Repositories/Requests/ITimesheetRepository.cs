using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Request.TimeSheet.Agency;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Request.TimeSheet;
using System.Linq.Expressions;

namespace Covenant.Common.Repositories.Requests;

public interface ITimesheetRepository : IBaseRepository
{
    Task<PaginatedList<TimeSheetListModel>> GetTimeSheetsForWorker(Guid workerId, Guid requestId, Pagination pagination);
    Task<TimeSheet> GetTimeSheet(Guid id);
    Task<IEnumerable<TimeSheet>> GetTimeSheets(Guid workerId, Guid requestId, Expression<Func<Common.Entities.Request.Request, bool>> requestCondition);
    Task<IEnumerable<TimeSheetListModel>> GetTimeSheetsListModel(Guid workerId, Guid requestId, DateTime? startDate, DateTime? endDate);
    Task<List<TimeSheetApprovedBillingModel>> GetTimeSheetForCreatingInvoice(IEnumerable<Guid> agencyIds, CreateInvoiceModel model);
    Task<List<TimeSheetApprovedPayrollModel>> GetTimeSheetForCreatingReportsSubcontractor(IEnumerable<Guid> agencyIds, Guid companyProfileId);
    Task<List<TimeSheetApprovedPayrollModel>> GetTimeSheetForCreatingPayStubs(IEnumerable<Guid> agencyIds, Guid workerId);
    Task<List<TimeSheetApprovedPayrollModel>> GetApprovedTimeSheetsInRange(Guid workerProfileId, DateTime start, DateTime end);
    Task<PaginatedList<TimeSheetHistoryModel>> GetTimeSheetHistory(Guid agencyId, Guid workerProfileId, GetWorkerTimeSheetHistoryFilter filter);
    Task<List<WorkerReadyForPayStubModel>> GetWorkersReadyForPayStub(IEnumerable<Guid> agencyIds);
    Task<TimeSheetUsagesModel> GetTimeSheetUsages(Guid requestId, Guid workerProfileId, Guid id);
    Task<IEnumerable<HoursWorkedResponse>> GetHoursWorked(Guid agencyId, HoursWorkedFilter filter);
    Task<IEnumerable<TimesheetsReportResponse>> GetTimesheetsReport(Guid agencyId, TimesheetsReportFilter filter);
    Task<IEnumerable<CompanyProfileJobPositionRateModel>> GetJobPositions(Guid companyProfileId, DateTime startDate, DateTime endDate);
    Task<TimeSheet> GetLatestTimesheet(Expression<Func<WorkerProfile, bool>> condition);
    Task<TimeSheet> GetLatestTimesheet(Guid workerId, Guid requestId);
    Task<List<TimeSheet>> GetTimesheetsSince(Guid workerId, Guid requestId, DateTime since);
    Task<bool> TimesheetUsedByAccounting(Guid id);
    Task<List<RequestTimeSheetModel>> GetRequestTimeSheet(Guid requestId);
}