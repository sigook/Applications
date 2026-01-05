using Covenant.Common.Entities;
using Covenant.Common.Functionals;
using Xunit;

namespace Covenant.Tests.Common
{
    public class ShiftTest
    {
        [Theory]
        [InlineData("00:00", "23:59")]
        [InlineData("01:00", "18:00")]
        public void Create(string startS, string finishS)
        {
            var shift = new Shift();
            TimeSpan start = TimeSpan.Parse(startS);
            TimeSpan finish = TimeSpan.Parse(finishS);
            Assert.True(shift.AddSunday(start, finish));
            Assert.True(shift.Sunday);
            Assert.Equal(start, shift.SundayStart);
            Assert.Equal(finish, shift.SundayFinish);

            Assert.True(shift.AddMonday(start, finish));
            Assert.True(shift.Monday);
            Assert.Equal(start, shift.MondayStart);
            Assert.Equal(finish, shift.MondayFinish);

            Assert.True(shift.AddTuesday(start, finish));
            Assert.True(shift.Tuesday);
            Assert.Equal(start, shift.TuesdayStart);
            Assert.Equal(finish, shift.TuesdayFinish);

            Assert.True(shift.AddWednesday(start, finish));
            Assert.True(shift.Wednesday);
            Assert.Equal(start, shift.WednesdayStart);
            Assert.Equal(finish, shift.WednesdayFinish);

            Assert.True(shift.AddThursday(start, finish));
            Assert.True(shift.Thursday);
            Assert.Equal(start, shift.ThursdayStart);
            Assert.Equal(finish, shift.ThursdayFinish);

            Assert.True(shift.AddFriday(start, finish));
            Assert.True(shift.Friday);
            Assert.Equal(start, shift.FridayStart);
            Assert.Equal(finish, shift.FridayFinish);

            Assert.True(shift.AddSaturday(start, finish));
            Assert.True(shift.Saturday);
            Assert.Equal(start, shift.SaturdayStart);
            Assert.Equal(finish, shift.SaturdayFinish);
        }

        [Fact]
        public void StartAndFinishNotLessThanZero()
        {
            var shift = new Shift();
            Result result = shift.AddSunday(TimeSpan.FromHours(-1), new TimeSpan(01, 00, 00));
            Assert.False(result);

            result = shift.AddSunday(new TimeSpan(01, 00, 00), TimeSpan.FromHours(-1));
            Assert.False(result);
        }

        [Fact]
        public void StartAndFinishNotGreaterThan23_59()
        {
            var shift = new Shift();
            Result result = shift.AddSunday(TimeSpan.FromHours(24), new TimeSpan(01, 00, 00));
            Assert.False(result);

            result = shift.AddSunday(new TimeSpan(01, 00, 00), TimeSpan.FromHours(24));
            Assert.False(result);
        }

        [Fact]
        public void UpdateDisplayShift()
        {
            var shift = new Shift();
            shift.AddMonday(TimeSpan.Parse("08:00"), TimeSpan.Parse("16:00"));
            Assert.Equal("Mon 08:00 to 16:00", shift.DisplayShift);
            shift.AddTuesday(TimeSpan.Parse("08:00"), TimeSpan.Parse("16:00"));
            Assert.Equal("Mon-Tue 08:00 to 16:00", shift.DisplayShift);
            shift.AddWednesday(TimeSpan.Parse("08:00"), TimeSpan.Parse("16:00"));
            Assert.Equal("Mon-Tue-Wed 08:00 to 16:00", shift.DisplayShift);
            shift.AddThursday(TimeSpan.Parse("08:00"), TimeSpan.Parse("16:00"));
            Assert.Equal("Mon-Tue-Wed-Thu 08:00 to 16:00", shift.DisplayShift);
            shift.AddFriday(TimeSpan.Parse("08:00"), TimeSpan.Parse("16:00"));
            Assert.Equal("Mon-Fri 08:00 to 16:00", shift.DisplayShift);
            shift.AddSaturday(TimeSpan.Parse("08:00"), TimeSpan.Parse("16:00"));
            Assert.Equal("Mon-Sat 08:00 to 16:00", shift.DisplayShift);
            shift.AddSunday(TimeSpan.Parse("08:00"), TimeSpan.Parse("16:00"));
            Assert.Equal("Sun-Sat 08:00 to 16:00", shift.DisplayShift);

            shift.AddSunday(TimeSpan.Parse("09:00"), TimeSpan.Parse("10:00"));
            Assert.Equal("Sun-Sat", shift.DisplayShift);
        }
    }
}