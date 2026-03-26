using Covenant.Common.Models.Request.TimeSheet;

namespace Covenant.TimeSheetTotal.Models;

[Obsolete]
public static class TimeSheetTotalMapper
{
    public static IEnumerable<TotalizatorCreatePayStubParams> TotalizatorParams(this IEnumerable<TimeSheetApprovedPayrollModel> listSource) =>
        listSource.Select(source => new TotalizatorCreatePayStubParams(
            source.RequestId, source.Date, source.TimeInApproved.GetValueOrDefault(),
            source.TimeOutApproved.GetValueOrDefault(), source.MissingHours,
            source.MissingHoursOvertime,
            source.WorkerRate, source.MissingRateWorker, source.IsHoliday,
            source.HolidayIsPaid, source.BreakIsPaid, source.DurationBreak,
            source.TimeSheetId)).ToList();

    public static CalculateTimeSheetTotalParams ToCalculateTimeSheetTotalParams(this TotalizatorCreatePayStubParams t, TimeSpan maxHoursWeek, TimeSpan overtimeStartsFrom) =>
        new CalculateTimeSheetTotalParams(
            t.BreakIsPaid,
            t.DurationBreak,
            t.HolidayIsPaid,
            t.TimeSheetId,
            t.TimeInApproved,
            t.TimeOutApproved,
            t.IsHoliday, maxHoursWeek, overtimeStartsFrom);
}