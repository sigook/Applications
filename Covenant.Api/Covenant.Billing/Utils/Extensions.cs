using Covenant.Common.Models.Request.TimeSheet;

namespace Covenant.Billing.Utils
{
    internal static class Extensions
    {
        internal static IEnumerable<IGrouping<double, TimeSheetApprovedBillingModel>> GroupTimeSheetByWeek(this IEnumerable<TimeSheetApprovedBillingModel> timeSheet) =>
            timeSheet.GroupBy(m => m.Week).OrderBy(m => m.Key).ToList();

        internal static IEnumerable<IGrouping<Guid, TimeSheetApprovedBillingModel>> GroupTimeSheetByWorker(this IEnumerable<TimeSheetApprovedBillingModel> timeSheet) =>
            timeSheet.GroupBy(w => w.WorkerId).ToList();
    }
}