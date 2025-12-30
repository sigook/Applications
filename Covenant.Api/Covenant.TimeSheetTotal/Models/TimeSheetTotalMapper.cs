using Covenant.Common.Models.Request.TimeSheet;

namespace Covenant.TimeSheetTotal.Models
{
    public static class TimeSheetTotalMapper
    {
        private static TotalizatorParams TotalizatorParams(this TimeSheetApprovedModel source) =>
            new TotalizatorParams(source.RequestId, source.Date, source.TimeInApproved.GetValueOrDefault(),
                source.TimeOutApproved.GetValueOrDefault(), source.MissingHours,
                source.MissingHoursOvertime, source.AgencyRate, source.MissingRateAgency,
                source.WorkerRate, source.MissingRateWorker, source.IsHoliday,
                source.HolidayIsPaid, source.BreakIsPaid, source.DurationBreak, source.PayNightShift,
                source.TimeStartNightShift, source.TimeEndNightShift, source.TimeSheetId);

        public static IEnumerable<TotalizatorCreatePayStubParams> TotalizatorParams(this IEnumerable<TimeSheetApprovedPayrollModel> listSource) =>
            listSource.Select(source => new TotalizatorCreatePayStubParams(
                source.RequestId, source.Date, source.TimeInApproved.GetValueOrDefault(),
                source.TimeOutApproved.GetValueOrDefault(), source.MissingHours,
                source.MissingHoursOvertime,
                source.WorkerRate, source.MissingRateWorker, source.IsHoliday,
                source.HolidayIsPaid, source.BreakIsPaid, source.DurationBreak,
                source.TimeSheetId)).ToList();

        public static IReadOnlyCollection<TotalizatorParams> TotalizatorParams(this IEnumerable<TimeSheetApprovedModel> source) =>
            source.Select(s => s.TotalizatorParams()).ToList();

        public static IEnumerable<TotalizatorCreateInvoiceParams> TotalizatorParams(this IEnumerable<TimeSheetApprovedBillingModel> sourceList) =>
            sourceList.Select(source =>
                new TotalizatorCreateInvoiceParams(source.RequestId, source.Date, source.TimeInApproved.GetValueOrDefault(),
                source.TimeOutApproved.GetValueOrDefault(), source.MissingHours,
                source.MissingHoursOvertime, source.AgencyRate, source.MissingRateAgency,
                source.IsHoliday, source.HolidayIsPaid, source.BreakIsPaid, source.DurationBreak,
                source.TimeSheetId, source.JobTitle)).ToList();

        public static CalculateTimeSheetTotalParams ToCalculateTimeSheetTotalParams(this TotalizatorParams t, TimeSpan maxHoursWeek, TimeSpan overtimeStartsFrom) =>
            new CalculateTimeSheetTotalParams(
                t.BreakIsPaid,
                t.DurationBreak,
                t.HolidayIsPaid,
                t.PayNightShift,
                t.TimeStartNightShift,
                t.TimeEndNightShift,
                t.TimeSheetId,
                t.TimeInApproved,
                t.TimeOutApproved,
                t.IsHoliday, maxHoursWeek, overtimeStartsFrom);

        public static CalculateTimeSheetTotalParams ToCalculateTimeSheetTotalParams(this TotalizatorCreatePayStubParams t, TimeSpan maxHoursWeek, TimeSpan overtimeStartsFrom) =>
            new CalculateTimeSheetTotalParams(
                t.BreakIsPaid,
                t.DurationBreak,
                t.HolidayIsPaid,
                t.TimeSheetId,
                t.TimeInApproved,
                t.TimeOutApproved,
                t.IsHoliday, maxHoursWeek, overtimeStartsFrom);

        public static CalculateTimeSheetTotalParams ToCalculateTimeSheetTotalParams(this TotalizatorCreateInvoiceParams t, TimeSpan maxHoursWeek, TimeSpan overtimeStartsFrom) =>
            new CalculateTimeSheetTotalParams(
                t.BreakIsPaid,
                t.DurationBreak,
                t.HolidayIsPaid,
                t.TimeSheetId,
                t.TimeInApproved,
                t.TimeOutApproved,
                t.IsHoliday, maxHoursWeek, overtimeStartsFrom);
    }
}