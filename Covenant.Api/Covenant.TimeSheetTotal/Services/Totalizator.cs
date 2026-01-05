using Covenant.Common.Configuration;
using Covenant.Common.Entities.Request;
using Covenant.TimeSheetTotal.Models;

namespace Covenant.TimeSheetTotal.Services
{
    public static class Totalizator
    {
        public static IReadOnlyCollection<Totals> GetWorkerTotals(this IEnumerable<TotalizatorParams> list, Rates rates, TimeSpan maxHoursWeek, TimeSpan overtimeStartsFrom)
        {
            return list.GroupBy(m => m.RequestId).SelectMany(m =>
            { //This is because to calculate overtime we need to accumulate by request
                TimeSpan accumulatedRegularHours = TimeSpan.Zero;
                return m.OrderBy(o => o.Date)
                    .Select(t => GetTotal(t, rates, maxHoursWeek, overtimeStartsFrom, ref accumulatedRegularHours));
            }).ToList();
        }

        public static Totals GetTotal(TotalizatorParams t, Rates rates, TimeSpan maxHoursWeek, TimeSpan overtimeStartsFrom, ref TimeSpan accumulatedRegularHours)
        {
            ITimeSheetTotal tst = TimeSheetTotalCalculator
                .Calculate(t.ToCalculateTimeSheetTotalParams(maxHoursWeek, overtimeStartsFrom), ref accumulatedRegularHours);

            var totalDailyPrice = new TotalDailyPrice(rates, t.AgencyRate, t.MissingRateAgency, t.MissingHours, t.MissingHoursOvertime,
                tst.RegularHours, tst.OtherRegularHours, tst.NightShiftHours, tst.HolidayHours, tst.OvertimeHours);

            var totalDailyWage = new TotalDailyWage(rates, t.WorkerRate, t.MissingRateWorker, t.MissingHours, t.MissingHoursOvertime,
                tst.RegularHours, tst.OtherRegularHours, tst.NightShiftHours, tst.HolidayHours, tst.OvertimeHours);
            return new Totals(tst, totalDailyPrice, totalDailyWage);
        }

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

        public static IEnumerable<(ITimeSheetTotal tst, TotalDailyPrice totalDailyPrice)> GetInvoiceTotals(
            this IEnumerable<TotalizatorCreateInvoiceParams> list, Rates rates, TimeSpan maxHoursWeek, TimeSpan overtimeStartsFrom)
        {
            return list.GroupBy(m => m.RequestId).SelectMany(m =>
            { //This is because to calculate overtime we need to accumulate by request
                TimeSpan accumulatedRegularHours = TimeSpan.Zero;
                return m.OrderBy(o => o.Date)
                    .Select(t =>
                    {
                        var parametersToCalculate = t.ToCalculateTimeSheetTotalParams(maxHoursWeek, overtimeStartsFrom);
                        var tst = TimeSheetTotalCalculator.Calculate(parametersToCalculate, ref accumulatedRegularHours);
                        var totalDailyPrice = new TotalDailyPrice(
                            rates,
                            t.AgencyRate,
                            t.MissingRateAgency,
                            t.MissingHours,
                            t.MissingHoursOvertime,
                            tst.RegularHours,
                            tst.OtherRegularHours,
                            tst.NightShiftHours,
                            tst.HolidayHours,
                            tst.OvertimeHours);
                        return (tst, totalDailyPrice);
                    });
            }).ToList();
        }
    }
}