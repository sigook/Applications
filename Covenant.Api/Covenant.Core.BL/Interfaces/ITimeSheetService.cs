using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Models.Worker;

namespace Covenant.Core.BL.Interfaces;

public interface ITimesheetService
{
    Task<Result<RegisterTimeSheetResultModel>> AddClockIn(Guid requestId, Guid workerId, TimeSpan clockIn);
    Task<Result<Guid>> CreateTimesheet(Guid workerId, Guid requestId, TimeSheetModel timeSheetModel);
    Task<Result> UpdateTimesheet(Guid timeSheetId, TimeSheetModel timeSheetModel);
    Task<Result<RegisterTimeSheetResultModel>> Register(Guid requestId, WorkerLocationModel workerLocationModel);
    Task<IEnumerable<CompanyProfileJobPositionRateModel>> GetJobPositions(Guid companyId, DateTime startDate, DateTime endDate);
    Task<HoursWorkedResume> GetHoursWorked(HoursWorkedFilter filter);
    Task<ResultGenerateDocument<MemoryStream>> GetHoursWorkedFile(HoursWorkedFilter filter);
    Task<Result<ClockType>> GetClockType(Guid requestId, DateTime? date);
    Task<Result> RemoveTimeSheet(Guid id);
    Task<ResultGenerateDocument<MemoryStream>> GetRequestTimesheetFile(Guid requestId);
}
