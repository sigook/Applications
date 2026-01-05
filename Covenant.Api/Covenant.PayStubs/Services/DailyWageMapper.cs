using Covenant.Common.Entities.Accounting.PayStub;
using Covenant.Common.Functionals;
using Covenant.TimeSheetTotal.Models;

namespace Covenant.PayStubs.Services
{
    public static class DailyWageMapper
    {
        public static List<Result<PayStubItem>> ToPayStubItems(this IReadOnlyCollection<TotalDailyWage> totals)
        {
            var items = new List<Result<PayStubItem>>();
            items.AddRange(Regular(totals));
            items.AddRange(OtherRegular(totals));
            items.AddRange(Overtime(totals));
            items.AddRange(HolidayPremiumPay(totals));
            items.AddRange(NightShift(totals));
            items.AddRange(Missing(totals));
            items.AddRange(MissingOvertime(totals));
            return items;
        }

        private static IEnumerable<Result<PayStubItem>> MissingOvertime(IEnumerable<TotalDailyWage> totals) =>
            totals.Where(w => w.MissingOvertime > decimal.Zero).GroupBy(w => w.MissingOvertimeRate)
                .Select(w => PayStubItem.CreateMissingOvertime(w.Sum(t => t.MissingOvertimeHours.TotalHours), w.Key));

        private static IEnumerable<Result<PayStubItem>> Missing(IEnumerable<TotalDailyWage> totals) =>
            totals.Where(w => w.Missing > decimal.Zero).GroupBy(w => w.MissingRateWorker)
                .Select(w => PayStubItem.CreateMissing(w.Sum(t => t.MissingHours.TotalHours), w.Key));

        private static IEnumerable<Result<PayStubItem>> NightShift(IEnumerable<TotalDailyWage> totals) =>
            totals.Where(w => w.NightShift > decimal.Zero).GroupBy(w => w.NightShiftRate)
                .Select(w => PayStubItem.CreateNightShift(w.Sum(t => t.NightShiftHours.TotalHours), w.Key));

        private static IEnumerable<Result<PayStubItem>> HolidayPremiumPay(IEnumerable<TotalDailyWage> totals) =>
            totals.Where(w => w.Holiday > decimal.Zero).GroupBy(w => w.HolidayRate)
                .Select(w => PayStubItem.CreateHolidayPremiumPay(w.Sum(t => t.HolidayHours.TotalHours), w.Key));

        private static IEnumerable<Result<PayStubItem>> Overtime(IEnumerable<TotalDailyWage> totals) =>
            totals.Where(w => w.Overtime > decimal.Zero).GroupBy(w => w.OvertimeRate)
                .Select(w => PayStubItem.CreateOvertime(w.Sum(t => t.OvertimeHours.TotalHours), w.Key));

        private static IEnumerable<Result<PayStubItem>> OtherRegular(IEnumerable<TotalDailyWage> totals) =>
            totals.Where(w => w.OtherRegular > decimal.Zero).GroupBy(w => w.WorkerRate)
                .Select(w => PayStubItem.CreateOtherRegular(w.Sum(t => t.OtherRegularHours.TotalHours), w.Key));

        private static IEnumerable<Result<PayStubItem>> Regular(IEnumerable<TotalDailyWage> totals) =>
            totals.Where(w => w.Regular > decimal.Zero).GroupBy(w => w.WorkerRate)
                .Select(w => PayStubItem.CreateRegular(w.Sum(t => t.RegularHours.TotalHours), w.Key));
    }
}