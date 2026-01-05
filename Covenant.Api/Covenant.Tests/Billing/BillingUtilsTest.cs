using Covenant.Common.Configuration;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Utils.Extensions;

namespace Covenant.Tests.Billing
{
    public static class BillingUtilsTest
    {
        internal static IEnumerable<TimeSheetApprovedModel> GetTimeSheet(
            int workers = 1,
            int days = 7,
            bool isSubcontractor = false,
            decimal workerRate = 1,
            decimal bonusOrOthers = 0,
            decimal otherDeductions = 0,
            bool paidHolidays = false)
        {
            return Enumerable.Range(1, workers).SelectMany(_ =>
            {
                var workerId = Guid.NewGuid();
                return Enumerable.Range(12, days).Select((i, index) =>
                {
                    var timeInApproved = new DateTime(2020, 01, i);
                    return new TimeSheetApprovedModel(
                        default,
                        default, default, default,
                        paidHolidays, TimeLimits.DefaultTimeLimits.MaxHoursWeek,
                        default, 1,
                        workerRate, default,
                        default, default, default,
                        default, default, workerId, default,
                        default, default, isSubcontractor,
                        default, timeInApproved.GetWeekOfYearStartSunday(), default,
                        timeInApproved,
                        timeInApproved.AddHours(8), default,
                        default, default, default,
                        default, bonusOrOthers, default, otherDeductions, default);
                }).ToList();
            });
        }

        internal static IEnumerable<TimeSheetApprovedBillingModel> GetTimeSheetApproved(
            int workers = 1, int days = 7, bool paidHolidays = false, decimal agencyRate = 1)
        {
            return Enumerable.Range(1, workers).SelectMany(_ =>
            {
                var workerId = Guid.NewGuid();
                return Enumerable.Range(12, days).Select((i, index) =>
                {
                    var timeInApproved = new DateTime(2020, 01, i);
                    return new TimeSheetApprovedBillingModel
                    {
                        PaidHolidays = paidHolidays,
                        OvertimeStartsAfter = TimeLimits.DefaultTimeLimits.MaxHoursWeek,
                        RequestId = default,
                        JobTitle = default,
                        AgencyRate = agencyRate,
                        BreakIsPaid = default,
                        DurationBreak = default,
                        HolidayIsPaid = default,
                        WorkerId = workerId,
                        TimeSheetId = default,
                        Date = timeInApproved.Date,
                        Week = timeInApproved.GetWeekOfYearStartSunday(),
                        TimeInApproved = timeInApproved,
                        TimeOutApproved = timeInApproved.AddHours(8),
                        MissingHours = default,
                        MissingHoursOvertime = default,
                        MissingRateAgency = default,
                        IsHoliday = default
                    };
                }).ToList();
            });
        }

        internal static Queue<NextNumberModel> NumbersQ(int quantity = 0) =>
            new Queue<NextNumberModel>(Numbers(quantity));

        internal static List<NextNumberModel> Numbers(int quantity = 0) =>
            Enumerable.Range(1, quantity)
                .Select(i => new NextNumberModel { NextNumber = i }).ToList();

        internal static List<IGrouping<Guid, TimeSheetApprovedModel>> GroupByWorkerId(this IEnumerable<TimeSheetApprovedModel> timeSheet) =>
            timeSheet.GroupBy(g => g.WorkerId).ToList();
    }
}