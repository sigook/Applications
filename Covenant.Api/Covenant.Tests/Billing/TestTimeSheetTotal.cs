using Covenant.Common.Entities.Request;
using Xunit;

namespace Covenant.Tests.Billing
{
    public class TestTimeSheetTotal
    {
        [Fact]
        public void CreateTotal()
        {
            var timeSheetId = Guid.NewGuid();
            TimeSpan totalHours = TimeSpan.FromHours(1);
            TimeSpan regularHours = TimeSpan.FromHours(2);
            TimeSpan overtimeHours = TimeSpan.FromHours(3);
            TimeSpan nightShiftHours = TimeSpan.FromHours(4);
            TimeSpan accumulatedHours = TimeSpan.FromHours(5);
            var sub = Covenant.Common.Entities.Request.TimeSheetTotal.CreateTotal(timeSheetId, totalHours,
                regularHours, overtimeHours, nightShiftHours, accumulatedHours);

            Assert.Equal(timeSheetId, sub.TimeSheetId);
            Assert.Equal(totalHours, sub.TotalHours);
            Assert.Equal(regularHours, sub.RegularHours);
            Assert.Equal(overtimeHours, sub.OvertimeHours);
            Assert.Equal(nightShiftHours, sub.NightShiftHours);
            Assert.Equal(accumulatedHours, sub.AccumulateWeekHours);
            Assert.Equal(TimeSpan.Zero, sub.OtherRegularHours);
            Assert.Equal(TimeSpan.Zero, sub.HolidayHours);
            Assert.Null(sub.TimeSheet);
            Assert.NotEqual(Guid.Empty, sub.Id);
        }

        [Fact]
        public void CreateTotalWithOtherRegular()
        {
            var timeSheetId = Guid.NewGuid();
            TimeSpan totalHours = TimeSpan.FromHours(1);
            TimeSpan regularHours = TimeSpan.FromHours(2);
            TimeSpan otherRegularHours = TimeSpan.FromHours(3);
            TimeSpan overtimeHours = TimeSpan.FromHours(4);
            TimeSpan nightShiftHours = TimeSpan.FromHours(5);
            TimeSpan accumulatedHours = TimeSpan.FromHours(6);
            var sub = Covenant.Common.Entities.Request.TimeSheetTotal.CreateTotalWithOtherRegular(timeSheetId, totalHours,
                regularHours, otherRegularHours, overtimeHours, nightShiftHours, accumulatedHours);

            Assert.Equal(timeSheetId, sub.TimeSheetId);
            Assert.Equal(totalHours, sub.TotalHours);
            Assert.Equal(regularHours, sub.RegularHours);
            Assert.Equal(otherRegularHours, sub.OtherRegularHours);
            Assert.Equal(overtimeHours, sub.OvertimeHours);
            Assert.Equal(nightShiftHours, sub.NightShiftHours);
            Assert.Equal(accumulatedHours, sub.AccumulateWeekHours);
            Assert.Equal(otherRegularHours, sub.OtherRegularHours);
            Assert.Equal(TimeSpan.Zero, sub.HolidayHours);
            Assert.Null(sub.TimeSheet);
            Assert.NotEqual(Guid.Empty, sub.Id);
        }

        [Fact]
        public void CreateTotalForHoliday()
        {
            var timeSheetId = Guid.NewGuid();
            TimeSpan totalHours = TimeSpan.FromHours(1);
            TimeSpan accumulatedHours = TimeSpan.FromHours(6);
            var sub = Covenant.Common.Entities.Request.TimeSheetTotal.CreateTotalForHoliday(timeSheetId, totalHours, accumulatedHours);

            Assert.Equal(timeSheetId, sub.TimeSheetId);
            Assert.Equal(totalHours, sub.TotalHours);
            Assert.Equal(TimeSpan.Zero, sub.RegularHours);
            Assert.Equal(TimeSpan.Zero, sub.OtherRegularHours);
            Assert.Equal(TimeSpan.Zero, sub.OvertimeHours);
            Assert.Equal(TimeSpan.Zero, sub.NightShiftHours);
            Assert.Equal(accumulatedHours, sub.AccumulateWeekHours);
            Assert.Equal(TimeSpan.Zero, sub.OtherRegularHours);
            Assert.Equal(totalHours, sub.HolidayHours);
            Assert.Null(sub.TimeSheet);
            Assert.NotEqual(Guid.Empty, sub.Id);
        }

        [Fact]
        public void CreateFromITimeSheetTotal()
        {
            var tst = new FakeTimeSheetTotal();
            var sut = new Covenant.Common.Entities.Request.TimeSheetTotal(tst);
            Assert.Equal(tst.Id, sut.Id);
            Assert.Equal(tst.TotalHours, sut.TotalHours);
            Assert.Equal(tst.RegularHours, sut.RegularHours);
            Assert.Equal(tst.OtherRegularHours, sut.OtherRegularHours);
            Assert.Equal(tst.NightShiftHours, sut.NightShiftHours);
            Assert.Equal(tst.HolidayHours, sut.HolidayHours);
            Assert.Equal(tst.OvertimeHours, sut.OvertimeHours);
            Assert.Equal(tst.AccumulateWeekHours, sut.AccumulateWeekHours);
            Assert.Equal(tst.TimeSheetId, sut.TimeSheetId);
            Assert.Same(tst.TimeSheet, sut.TimeSheet);
        }

        private class FakeTimeSheetTotal : ITimeSheetTotal
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public TimeSpan TotalHours { get; set; } = TimeSpan.FromHours(1);
            public TimeSpan RegularHours { get; set; } = TimeSpan.FromHours(2);
            public TimeSpan OtherRegularHours { get; set; } = TimeSpan.FromHours(3);
            public TimeSpan NightShiftHours { get; set; } = TimeSpan.FromHours(4);
            public TimeSpan HolidayHours { get; set; } = TimeSpan.FromHours(5);
            public TimeSpan OvertimeHours { get; set; } = TimeSpan.FromHours(6);
            public TimeSpan AccumulateWeekHours { get; set; } = TimeSpan.FromHours(7);
            public Guid TimeSheetId { get; set; } = Guid.NewGuid();

            public TimeSheet TimeSheet { get; set; } = TimeSheet.CreateTimeSheet(
                Guid.NewGuid(), new DateTime(2020, 01, 01), TimeSpan.FromHours(1),
                now: new DateTime(2020, 01, 01)).Value;
        }
    }
}