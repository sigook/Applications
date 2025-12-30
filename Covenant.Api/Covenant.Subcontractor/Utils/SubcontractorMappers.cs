using Covenant.Common.Entities.Accounting.Subcontractor;
using Covenant.Common.Entities.Request;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.TimeSheetTotal.Models;

namespace Covenant.Subcontractor.Utils
{
    public static class SubcontractorMappers
    {
        public static ReportSubcontractorWageDetail ToSubcontractorWageDetail(this TotalDailyWage source, ITimeSheetTotal timeSheetTotal) =>
            new ReportSubcontractorWageDetail(
                    source.WorkerRate,
                    source.Regular,
                    source.OtherRegular,
                    source.Missing,
                    source.MissingOvertime,
                    source.NightShift,
                    source.Holiday,
                    source.Overtime)
            {
                TimeSheetTotal = timeSheetTotal is TimeSheetTotalPayroll total
                    ? total
                    : new TimeSheetTotalPayroll(timeSheetTotal)
            };

        internal static async Task<IReadOnlyCollection<TResult>> ForEachHoliday<TInput, TResult>(this IEnumerable<TInput> holidays, Func<TInput, Task<TResult>> action)
        {
            var items = new List<TResult>();
            if (holidays is null) return items;
            foreach (TInput holiday in holidays)
            {
                TResult item = await action(holiday);
                if (item == null) continue;
                items.Add(item);
            }
            return items;
        }

        internal static IEnumerable<IGrouping<double, TimeSheetApprovedPayrollModel>> GroupTimeSheetByWeek(this IEnumerable<TimeSheetApprovedPayrollModel> timeSheet) =>
            TimeSheetApprovedPayrollModel.GroupTimeSheetByWeek(timeSheet);
    }
}