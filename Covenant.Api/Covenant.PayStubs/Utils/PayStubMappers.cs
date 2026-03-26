using Covenant.Common.Entities.Accounting.PayStub;
using Covenant.Common.Entities.Request;
using Covenant.Common.Enums;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Utils.Extensions;
using Covenant.TimeSheetTotal.Models;

namespace Covenant.PayStubs.Utils
{
    public static class PayStubMappers
    {
        public static PayStubWageDetail ToWageDetail(this TotalDailyWage source, ITimeSheetTotal timeSheetTotal) =>
            new PayStubWageDetail(
                source.WorkerRate,
                source.Regular,
                source.OtherRegular,
                source.Missing,
                source.MissingOvertime,
                source.NightShift,
                source.Holiday,
                source.Overtime,
                timeSheetTotal.Id)
            {
                TimeSheetTotal = new TimeSheetTotalPayroll(timeSheetTotal)
            };

        internal static IEnumerable<IGrouping<double, TimeSheetApprovedPayrollModel>> GroupTimeSheetByWeek(this IEnumerable<TimeSheetApprovedPayrollModel> timeSheet) =>
            TimeSheetApprovedPayrollModel.GroupTimeSheetByWeek(timeSheet);

        internal static decimal GetRegularWage(this IEnumerable<PayStubItem> items) =>
            items.Where(i => i.Type == PayStubItemType.Regular).Sum(i => i.Total).DefaultMoneyRound();
    }
}