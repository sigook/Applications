using Covenant.Common.Configuration;
using Covenant.Common.Entities.Request;
using Covenant.TimeSheetTotal.Models;
using Covenant.TimeSheetTotal.Services;
using Xunit;

namespace Covenant.Tests.Billing
{
    public class TestTimeSheetTotalCalculator
    {
        private static readonly TimeLimits TimeLimits = TimeLimits.DefaultTimeLimits;

        [Fact]
        public void TotalHours()
        {
            var timeIn = new DateTime(2019, 01, 01, 00, 00, 00);
            var timeOut = new DateTime(2019, 01, 01, 23, 00, 00);
            TimeSpan totalHours = TimeSheetTotalCalculator.TotalHours(timeIn.TimeOfDay, timeOut.TimeOfDay, true, TimeSpan.FromHours(1));
            Assert.Equal(TimeSpan.FromHours(22), totalHours);

            totalHours = TimeSheetTotalCalculator.TotalHours(timeIn.TimeOfDay, timeOut.TimeOfDay, true, TimeSpan.FromHours(1));
            Assert.Equal(TimeSpan.FromHours(22), totalHours);

            totalHours = TimeSheetTotalCalculator.TotalHours(timeIn.TimeOfDay, timeOut.TimeOfDay, false, TimeSpan.Zero);
            Assert.Equal(TimeSpan.FromHours(23), totalHours);
        }

        [Theory]
        [InlineData(0, 44, 8, 0)] //No overtime
        [InlineData(44, 44, 8, 0)] //No overtime
        [InlineData(45, 44, 8, 1)] //Overtime
        [InlineData(46, 44, 8, 2)] //Overtime
        [InlineData(47, 44, 8, 3)] //Overtime
        [InlineData(48, 44, 8, 4)] //Half overtime
        [InlineData(49, 44, 8, 5)] //Overtime
        [InlineData(50, 44, 8, 6)] //Overtime
        [InlineData(51, 44, 8, 7)] //Overtime
        [InlineData(52, 44, 8, 8)] //All overtime
        [InlineData(53, 44, 8, 8)] //All overtime maximum overtime totalHours
        [InlineData(54, 44, 8, 8)] //All overtime maximum overtime totalHours
        public void OvertimeHours(int accumulateWeekHours, int maxHoursWeek, int totalHours, int expected)
        {
            TimeSpan overtimeHours = TimeSheetTotalCalculator.OvertimeHours(
                TimeSpan.FromHours(accumulateWeekHours),
                TimeSpan.FromHours(maxHoursWeek),
                TimeSpan.FromHours(totalHours));
            Assert.Equal(TimeSpan.FromHours(expected), overtimeHours);
        }

        [Theory]
        [InlineData(8, 0, 0, 8)] //Regular
        [InlineData(8, 8, 0, 0)] //Is overtime there is not regular
        [InlineData(8, 0, 8, 0)] //Is night shift there is not regular
        [InlineData(8, 4, 4, 0)] //Is Overtime and night shift there is not regular
        [InlineData(0, 0, 0, 0)]
        public void RegularHours(int totalHours, int overtimeHours, int nightShiftHours, int expected)
        {
            TimeSpan regularHours = TimeSheetTotalCalculator.RegularHours(
                TimeSpan.FromHours(totalHours),
                TimeSpan.FromHours(overtimeHours),
                TimeSpan.FromHours(nightShiftHours));
            Assert.Equal(TimeSpan.FromHours(expected), regularHours);
        }

        [Theory]
        [InlineData(false, null, null, false)]
        [InlineData(false, "0", "0", false)]
        [InlineData(true, null, "0", false)]
        [InlineData(true, "0", null, false)]
        [InlineData(true, null, null, false)]
        [InlineData(true, "0", "0", true)]
        public void ShouldCalculateNightShift(bool payNightShift, string start, string end, bool expected)
        {
            TimeSpan? timeStart = null;
            TimeSpan? timeEnd = null;
            if (!string.IsNullOrEmpty(start)) timeStart = TimeSpan.Parse(start);
            if (!string.IsNullOrEmpty(end)) timeEnd = TimeSpan.Parse(end);
            bool shouldCalculateNightShift = TimeSheetTotalCalculator.ShouldCalculateNightShift(payNightShift, timeStart, timeEnd);
            Assert.Equal(expected, shouldCalculateNightShift);
        }

        [Theory]
        [InlineData(false, 0)]
        [InlineData(true, 8)]
        public void NightShiftHours(bool payNightShift, int expected)
        {
            TimeSpan timeIn = new DateTime(2019, 01, 01, 00, 00, 00).TimeOfDay;
            TimeSpan timeOut = new DateTime(2019, 01, 01, 08, 00, 00).TimeOfDay;
            TimeSpan timeStart = timeIn;
            TimeSpan timeEnd = timeOut;
            TimeSpan nightShiftHours = TimeSheetTotalCalculator.NightShiftHours(payNightShift, timeStart, timeEnd, timeIn, timeOut);
            Assert.Equal(TimeSpan.FromHours(expected), nightShiftHours);
        }

        [Fact]
        public void CalculateTimeSheetTotal_NormalCase()
        {
            Guid timeSheetId = Guid.NewGuid();
            var timeInApproved = new DateTime(2018, 01, 01, 00, 00, 00);
            var timeOutApproved = new DateTime(2018, 01, 01, 08, 00, 00);
            TimeSpan accumulateWeekHours = TimeSpan.FromHours(8);
            ITimeSheetTotal timeSheetTotal =
                TimeSheetTotalCalculator.Calculate(new CalculateTimeSheetTotalParams(false, TimeSpan.FromHours(0),
                    true, true, null,
                    null,
                    timeSheetId, timeInApproved, timeOutApproved, false, TimeLimits.MaxHoursWeek, TimeLimits.MaxHoursWeek), ref accumulateWeekHours);
            Assert.Equal(TimeSpan.FromHours(8), timeSheetTotal.TotalHours);
            Assert.Equal(TimeSpan.FromHours(8), timeSheetTotal.RegularHours);
            Assert.Equal(TimeSpan.FromHours(0), timeSheetTotal.NightShiftHours);
            Assert.Equal(TimeSpan.FromHours(0), timeSheetTotal.HolidayHours);
            Assert.Equal(TimeSpan.FromHours(0), timeSheetTotal.OvertimeHours);
            Assert.Equal(accumulateWeekHours, timeSheetTotal.AccumulateWeekHours);
            Assert.Equal(timeSheetId, timeSheetTotal.TimeSheetId);
        }

        [Theory]
        [InlineData(true, "06:00:00", "08:00:00", "06:00:00", "02:00:00")]
        [InlineData(false, "06:00:00", "08:00:00", "08:00:00", "00:00:00")]
        [InlineData(true, "22:00:00", "00:00:00", "08:00:00", "00:00:00")]
        [InlineData(true, "08:00:00", "10:00:00", "08:00:00", "00:00:00")]
        public void CalculateTimeSheetTotal_NightShiftHours(bool payNightShift, string start, string end, string regular, string nightShift)
        {
            Guid timeSheetId = Guid.NewGuid();
            var timeInApproved = new DateTime(2018, 01, 01, 00, 00, 00);
            var timeOutApproved = new DateTime(2018, 01, 01, 08, 00, 00);
            TimeSpan accumulateWeekHours = TimeSpan.FromHours(8);

            TimeSpan timeStartNightShift = TimeSpan.Parse(start);
            TimeSpan timeEndNightShift = TimeSpan.Parse(end);
            TimeSpan durationBreak = TimeSpan.Zero;
            ITimeSheetTotal timeSheetTotal =
                TimeSheetTotalCalculator.Calculate(new CalculateTimeSheetTotalParams(false, durationBreak,
                    false, payNightShift, timeStartNightShift, timeEndNightShift,
                    timeSheetId, timeInApproved, timeOutApproved, false, TimeLimits.MaxHoursWeek, TimeLimits.MaxHoursWeek), ref accumulateWeekHours);
            Assert.Equal(TimeSpan.FromHours(8), timeSheetTotal.TotalHours);
            Assert.Equal(TimeSpan.Parse(regular), timeSheetTotal.RegularHours);
            Assert.Equal(TimeSpan.Parse(nightShift), timeSheetTotal.NightShiftHours);
            Assert.Equal(TimeSpan.FromHours(0), timeSheetTotal.HolidayHours);
            Assert.Equal(TimeSpan.FromHours(0), timeSheetTotal.OvertimeHours);
            Assert.Equal(accumulateWeekHours, timeSheetTotal.AccumulateWeekHours);
            Assert.Equal(timeSheetId, timeSheetTotal.TimeSheetId);
        }

        [Theory]
        [InlineData(true, true, "00:00:00", "08:00:00")]
        [InlineData(false, false, "08:00:00", "00:00:00")]
        [InlineData(true, false, "08:00:00", "00:00:00")]
        [InlineData(false, true, "08:00:00", "00:00:00")]
        public void CalculateTimeSheetTotal_Holiday(bool isHoliday, bool isPaid, string regular, string holiday)
        {
            Guid timeSheetId = Guid.NewGuid();
            var timeInApproved = new DateTime(2018, 01, 01, 00, 00, 00);
            var timeOutApproved = new DateTime(2018, 01, 01, 08, 00, 00);

            bool holidayIsPaid = isPaid;
            TimeSpan accumulateWeekHours = TimeSpan.FromHours(8);
            const bool breakIsPaid = false;
            TimeSpan durationBreak = TimeSpan.Zero;
            const bool payNightShift = false;
            ITimeSheetTotal timeSheetTotal =
                TimeSheetTotalCalculator.Calculate(new CalculateTimeSheetTotalParams(breakIsPaid,
                    durationBreak,
                    holidayIsPaid,
                    payNightShift,
                    default,
                    default,
                    timeSheetId,
                    timeInApproved,
                    timeOutApproved,
                    isHoliday, TimeLimits.MaxHoursWeek, TimeLimits.MaxHoursWeek), ref accumulateWeekHours);
            Assert.Equal(TimeSpan.FromHours(8), timeSheetTotal.TotalHours);
            Assert.Equal(TimeSpan.Parse(regular), timeSheetTotal.RegularHours);
            Assert.Equal(TimeSpan.FromHours(0), timeSheetTotal.NightShiftHours);
            Assert.Equal(TimeSpan.Parse(holiday), timeSheetTotal.HolidayHours);
            Assert.Equal(TimeSpan.FromHours(0), timeSheetTotal.OvertimeHours);
            Assert.Equal(accumulateWeekHours, timeSheetTotal.AccumulateWeekHours);
            Assert.Equal(timeSheetId, timeSheetTotal.TimeSheetId);
        }

        [Theory]
        [InlineData(44, "00:00:00", "08:00:00")]
        [InlineData(40, "04:00:00", "04:00:00")]
        [InlineData(36, "08:00:00", "00:00:00")]
        public void CalculateTimeSheetTotal_Overtime(int accumulate, string regular, string overtime)
        {
            const int workedHours = 8;
            Guid timeSheetId = Guid.NewGuid();
            var timeInApproved = new DateTime(2018, 01, 01);
            DateTime timeOutApproved = timeInApproved.AddHours(workedHours);
            TimeSpan accumulateWeekHours = TimeSpan.FromHours(accumulate);

            ITimeSheetTotal timeSheetTotal = TimeSheetTotalCalculator.Calculate(
                new CalculateTimeSheetTotalParams(default, default, default,
                    default, default, default, timeSheetId,
                    timeInApproved, timeOutApproved, false, TimeLimits.MaxHoursWeek, TimeLimits.MaxHoursWeek),
                ref accumulateWeekHours);
            Assert.Equal(TimeSpan.FromHours(8), timeSheetTotal.TotalHours);
            Assert.Equal(TimeSpan.Parse(regular), timeSheetTotal.RegularHours);
            Assert.Equal(TimeSpan.FromHours(0), timeSheetTotal.NightShiftHours);
            Assert.Equal(TimeSpan.FromHours(0), timeSheetTotal.HolidayHours);
            Assert.Equal(TimeSpan.Parse(overtime), timeSheetTotal.OvertimeHours);
            Assert.Equal(accumulateWeekHours, timeSheetTotal.AccumulateWeekHours);
            Assert.Equal(timeSheetId, timeSheetTotal.TimeSheetId);
        }

        [Theory]
        [InlineData(40, 44, 50, 12, 4, 6, 2)]//Regular, OtherRegular, Overtime
        [InlineData(40, 44, 44, 12, 4, 0, 8)]//Regular,Overtime
        [InlineData(40, 44, 60, 12, 4, 8, 0)]//Regular,OtherRegular
        [InlineData(44, 44, 44, 12, 0, 0, 12)]//Overtime
        [InlineData(40, 44, 40, 12, 0, 0, 12)]//Overtime
        [InlineData(44, 44, 60, 12, 0, 12, 0)]//OtherRegular
        [InlineData(30, 44, 50, 12, 12, 0, 0)]//Regular
        public void TimeSheetTotal_Other_Regular_Hours(
            int accumulated, int maxHours, int overtimeStarts, int dailyHours, int expectedRegular, int expectedOtherRegular, int expectedOvertime)
        {
            TimeSpan accumulateHours = TimeSpan.FromHours(accumulated);
            TimeSpan maxHoursWeek = TimeSpan.FromHours(maxHours);
            TimeSpan overtimeStartsFrom = TimeSpan.FromHours(overtimeStarts);
            var timeIn = new DateTime(2019, 01, 01, 00, 00, 00);
            DateTime timeOut = timeIn.AddHours(dailyHours);
            ITimeSheetTotal total =
                TimeSheetTotalCalculator.Calculate(new CalculateTimeSheetTotalParams(
                        default, default,
                        default, default,
                        default, default, default,
                        timeIn, timeOut, default,
                        maxHoursWeek, overtimeStartsFrom),
                    ref accumulateHours);

            Assert.Equal(TimeSpan.FromHours(expectedRegular), total.RegularHours);
            Assert.Equal(TimeSpan.FromHours(expectedOtherRegular), total.OtherRegularHours);
            Assert.Equal(TimeSpan.FromHours(expectedOvertime), total.OvertimeHours);
            Assert.Equal(TimeSpan.Zero, total.NightShiftHours);
            Assert.Equal(TimeSpan.Zero, total.HolidayHours);
        }
    }
}