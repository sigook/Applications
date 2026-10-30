using Covenant.Common.Configuration;
using Covenant.Common.Enums;
using Covenant.Common.Models.Accounting.PayStubs;
using Covenant.Common.Models.Requests.TimeSheets;
using Covenant.Common.Models.Requests.Workers;
using Covenant.Common.Models.Workers;
using Covenant.Common.Repositories.Requests;
using Covenant.Core.BL.Interfaces.Accounting.PayStubs;
using Covenant.Core.BL.Interfaces.Accounting;
using Covenant.Core.BL.Interfaces.Workers;

namespace Covenant.Core.BL.Services.Workers;

public class WorkerActivityService(
    ITimesheetRepository timesheetRepository,
    IPayStubService payStubService,
    ITimesheetCalculatorService calculatorService,
    TimeLimits timeLimits) : IWorkerActivityService
{
    public async Task<WorkerWeekTimeSheetsModel> GetWeekTimeSheets(Guid workerId, DateTime weekStart)
    {
        var start = weekStart.Date.AddDays(-(int)weekStart.DayOfWeek);
        var end = start.AddDays(7);
        var items = await timesheetRepository.GetWeekTimeSheetsForWorker(workerId, start, end);

        foreach (var request in items.GroupBy(ts => ts.RequestId))
        {
            var accumulated = TimeSpan.Zero;
            foreach (var timeSheet in request)
                CalculateHours(timeSheet, ref accumulated);
        }

        return new WorkerWeekTimeSheetsModel
        {
            WeekStart = start,
            WeekEnd = end.AddDays(-1),
            Items = items,
            Hours = new WorkerWeekHoursModel
            {
                Total = items.Sum(ts => ts.TotalHours),
                Regular = items.Sum(ts => ts.RegularHours),
                Overtime = items.Sum(ts => ts.OvertimeHours),
                Holiday = items.Sum(ts => ts.HolidayHours),
                PendingApproval = items.Where(ts => ts.Status == WorkerTimeSheetStatus.Pending).Sum(ts => ts.TotalHours)
            }
        };
    }

    public async Task<WorkerActivitySummaryModel> GetSummary(Guid workerId, DateTime weekStart)
    {
        var week = await GetWeekTimeSheets(workerId, weekStart);
        var payStubs = await payStubService.GetPayStubsForWorker(workerId, new GetWorkerPayStubsFilter { PageSize = 1 });

        return new WorkerActivitySummaryModel
        {
            WeekHours = week.Hours,
            LastPayStub = payStubs.Items.FirstOrDefault()
        };
    }

    private void CalculateHours(WorkerWeekTimeSheetModel timeSheet, ref TimeSpan accumulated)
    {
        var isApproved = timeSheet.TimeInApproved.HasValue && timeSheet.TimeOutApproved.HasValue;
        timeSheet.Status = timeSheet.IsPaid
            ? WorkerTimeSheetStatus.Paid
            : isApproved
                ? WorkerTimeSheetStatus.Approved
                : timeSheet.TimeOut.HasValue ? WorkerTimeSheetStatus.Pending : WorkerTimeSheetStatus.Open;
        if (timeSheet.Status == WorkerTimeSheetStatus.Open) return;

        var timeIn = isApproved ? timeSheet.TimeInApproved.Value : timeSheet.TimeIn;
        var timeOut = isApproved ? timeSheet.TimeOutApproved.Value : timeSheet.TimeOut.Value;
        var breakdown = calculatorService.CalculateHoursBreakdown(
            timeIn,
            timeOut,
            timeSheet.DurationBreak,
            timeSheet.BreakIsPaid,
            timeSheet.IsHoliday,
            timeSheet.HolidayIsPaid,
            ref accumulated,
            timeSheet.OvertimeStartsAfter,
            timeLimits.MaxHoursWeek);

        timeSheet.RegularHours = (breakdown.RegularHours + breakdown.OtherRegularHours).TotalHours;
        timeSheet.OvertimeHours = breakdown.OvertimeHours.TotalHours;
        timeSheet.HolidayHours = breakdown.HolidayHours.TotalHours;
        timeSheet.TotalHours = timeSheet.RegularHours + timeSheet.OvertimeHours + timeSheet.HolidayHours;
    }
}
