using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.PayStub;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.PayStubs.Services;
using Covenant.TimeSheetTotal.Models;
using Xunit;

namespace Covenant.Tests.Billing.PayStubTest
{
    public class TestDailyWageMapper
    {
        private static readonly Rates FakeRates = new Rates { OverTime = 1, Holiday = 1, NightShift = 1 };

        [Fact]
        public void ToPayStubItems_Regular()
        {
            const int workerRate = 1;
            TimeSpan regularHours = TimeSpan.FromHours(1);
            var totals = new[]{new TotalDailyWage(FakeRates,workerRate,
                default,default,
                default, regularHours,
                default, default,
                default, default)};
            var sub = totals.ToPayStubItems();
            var item = sub.Single(r => r.Value.Type == PayStubItemType.Regular).Value;
            Assert.Equal(PayStubItem.RegularHoursLabel, item.Description);
            Assert.Equal(regularHours.TotalHours, item.Quantity);
            Assert.Equal(workerRate, item.UnitPrice);
            Assert.Equal((decimal)regularHours.TotalHours, item.Total);
        }

        [Fact]
        public void ToPayStubItems_OtherRegular()
        {
            const int workerRate = 1;
            TimeSpan otherRegularHours = TimeSpan.FromHours(1);
            var totals = new[]{new TotalDailyWage(FakeRates,workerRate,
                default,default,
                default, default,
                otherRegularHours, default,
                default, default)};
            List<Result<PayStubItem>> sub = totals.ToPayStubItems();
            Assert.Single(sub);
            PayStubItem item = sub.Single(r => r.Value.Type == PayStubItemType.OtherRegular).Value;
            Assert.Equal(PayStubItem.OtherRegularHoursLabel, item.Description);
            Assert.Equal(otherRegularHours.TotalHours, item.Quantity);
            Assert.Equal(workerRate, item.UnitPrice);
            Assert.Equal((decimal)otherRegularHours.TotalHours, item.Total);
        }

        [Fact]
        public void ToPayStubItems_Overtime()
        {
            const int workerRate = 1;
            TimeSpan overtime = TimeSpan.FromHours(1);
            var totals = new[]{new TotalDailyWage(FakeRates,workerRate,
                default,default,
                default, default,
                default, default,
                default, overtime)};
            List<Result<PayStubItem>> sub = totals.ToPayStubItems();
            Assert.Single(sub);
            PayStubItem item = sub.Single(r => r.Value.Type == PayStubItemType.Overtime).Value;
            Assert.Equal(PayStubItem.OvertimeHoursLabel, item.Description);
            Assert.Equal(overtime.TotalHours, item.Quantity);
            Assert.Equal(workerRate, item.UnitPrice);
            Assert.Equal((decimal)overtime.TotalHours, item.Total);
        }

        [Fact]
        public void ToPayStubItems_Holiday_Premium_Pay()
        {
            const int workerRate = 1;
            TimeSpan holiday = TimeSpan.FromHours(1);
            var totals = new[]{new TotalDailyWage(FakeRates,workerRate,
                default,default,
                default, default,
                default, default,
                holiday, default)};
            List<Result<PayStubItem>> sub = totals.ToPayStubItems();
            Assert.Single(sub);
            PayStubItem item = sub.Single(r => r.Value.Type == PayStubItemType.HolidayPremiumPay).Value;
            Assert.Equal(PayStubItem.HolidayPremiumPayHoursLabel, item.Description);
            Assert.Equal(holiday.TotalHours, item.Quantity);
            Assert.Equal(workerRate, item.UnitPrice);
            Assert.Equal((decimal)holiday.TotalHours, item.Total);
        }

        [Fact]
        public void ToPayStubItems_Night_Shift_Hours()
        {
            const int workerRate = 1;
            TimeSpan nightShiftHours = TimeSpan.FromHours(1);
            var totals = new[]{new TotalDailyWage(FakeRates,workerRate,
                default,default,
                default, default,
                default, nightShiftHours,
                default, default)};
            List<Result<PayStubItem>> sub = totals.ToPayStubItems();
            Assert.Single(sub);
            PayStubItem item = sub.Single(r => r.Value.Type == PayStubItemType.NightShift).Value;
            Assert.Equal(PayStubItem.NightShiftHoursLabel, item.Description);
            Assert.Equal(nightShiftHours.TotalHours, item.Quantity);
            Assert.Equal(workerRate, item.UnitPrice);
            Assert.Equal((decimal)nightShiftHours.TotalHours, item.Total);
        }

        [Fact]
        public void ToPayStubItems_MissingHours()
        {
            const int workerRate = 1;
            TimeSpan missing = TimeSpan.FromHours(1);
            var totals = new[]{new TotalDailyWage(FakeRates,workerRate,
                default,missing,
                default, default,
                default, default,
                default, default)};
            List<Result<PayStubItem>> sub = totals.ToPayStubItems();
            Assert.Single(sub);
            PayStubItem item = sub.Single(r => r.Value.Type == PayStubItemType.Missing).Value;
            Assert.Equal(PayStubItem.MissingHoursLabel, item.Description);
            Assert.Equal(missing.TotalHours, item.Quantity);
            Assert.Equal(workerRate, item.UnitPrice);
            Assert.Equal((decimal)missing.TotalHours, item.Total);
        }

        [Fact]
        public void ToPayStubItems_MissingHoursOvertime()
        {
            const int workerRate = 1;
            TimeSpan missingOvertime = TimeSpan.FromHours(1);
            var totals = new[]{new TotalDailyWage(FakeRates,workerRate,
                default,default,
                missingOvertime, default,
                default, default,
                default, default)};
            List<Result<PayStubItem>> sub = totals.ToPayStubItems();
            Assert.Single(sub);
            PayStubItem item = sub.Single(r => r.Value.Type == PayStubItemType.MissingOvertime).Value;
            Assert.Equal(PayStubItem.MissingOvertimeHoursLabel, item.Description);
            Assert.Equal(missingOvertime.TotalHours, item.Quantity);
            Assert.Equal(workerRate, item.UnitPrice);
            Assert.Equal((decimal)missingOvertime.TotalHours, item.Total);
        }
    }
}