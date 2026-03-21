using Covenant.Common.Configuration;
using Covenant.Common.Entities.Request;
using Covenant.TimeSheetTotal.Models;

namespace Covenant.TimeSheetTotal.Services;

[Obsolete]
public static class Totalizator
{
    public static List<(ITimeSheetTotal tst, TotalDailyWage totalDailyWage)> GetPayStubTotals(
        this IEnumerable<TotalizatorCreatePayStubParams> list, Rates rates, TimeSpan maxHoursWeek, TimeSpan overtimeStartsFrom)
    {
        return list.GroupBy(m => m.RequestId).SelectMany(m =>
        { //This is because to calculate overtime we need to accumulate by request
            TimeSpan accumulatedRegularHours = TimeSpan.Zero;
            return m.OrderBy(o => o.Date)
                .Select(t =>
                {
                    ITimeSheetTotal tst = TimeSheetTotalCalculator
                        .Calculate(t.ToCalculateTimeSheetTotalParams(maxHoursWeek, overtimeStartsFrom), ref accumulatedRegularHours);

                    var totalDailyWage = new TotalDailyWage(rates, t.WorkerRate, t.MissingRateWorker, t.MissingHours, t.MissingHoursOvertime,
                        tst.RegularHours, tst.OtherRegularHours, tst.NightShiftHours, tst.HolidayHours, tst.OvertimeHours);

                    return (tst, totalDailyWage);
                });
        }).ToList();
    }
}