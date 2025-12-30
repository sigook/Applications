using System;
using Covenant.TimeSheetTotal.Services;
using Xunit;

namespace Covenant.Tests.Billing
{
    public class TestNightShiftService
    {
        [Theory]
        [InlineData("00:00:00", "01:00:00", "00:00:00", "01:00:00", "01:00:00")]
        [InlineData("00:00:00", "02:00:00", "00:00:00", "01:00:00", "01:00:00")]
        [InlineData("00:00:00", "03:00:00", "00:00:00", "01:00:00", "01:00:00")]
        [InlineData("00:00:00", "04:00:00", "00:00:00", "01:00:00", "01:00:00")]
        [InlineData("00:00:00", "01:00:00", "00:00:00", "00:00:00", "00:00:00")]
        [InlineData("00:00:00", "02:00:00", "00:00:00", "00:00:00", "00:00:00")]
        [InlineData("00:00:00", "03:00:00", "00:00:00", "00:00:00", "00:00:00")]
        [InlineData("00:00:00", "04:00:00", "00:00:00", "00:00:00", "00:00:00")]
        [InlineData("20:00:00", "23:59:59", "22:00:00", "06:00:00", "01:59:59")]
        [InlineData("21:00:00", "23:59:59", "22:00:00", "06:00:00", "01:59:59")]
        [InlineData("22:00:00", "23:59:59", "22:00:00", "06:00:00", "01:59:59")]
        [InlineData("23:00:00", "23:59:59", "22:00:00", "06:00:00", "00:59:59")]
        public void CalculateNightShift(string start1, string end1, string start2, string end2, string result)
        {
            TimeSpan entry = TimeSpan.Parse(start1);
            TimeSpan departure = TimeSpan.Parse(end1);
            TimeSpan startNight = TimeSpan.Parse(start2);
            TimeSpan endNight = TimeSpan.Parse(end2);

            TimeSpan nightShift = NightShiftCalculator.Calculate(entry, departure, startNight, endNight);
            Assert.Equal(TimeSpan.Parse(result), nightShift);
        }

        [Fact]
        public void DemoRangeDatePlusTime()
        {
            var start = new DateTime(2018, 01, 01, 08, 00, 00);
            var end = new DateTime(2018, 01, 02, 23, 00, 00);

            var valueTrue = new DateTime(2018, 01, 02, 08, 00, 00);
            var valueFalse = new DateTime(2018, 01, 02, 23, 30, 00);

            bool valueComparison = start <= valueTrue && end > valueTrue;
            Assert.True(valueComparison);
            valueComparison = start <= valueTrue && end > valueFalse;
            Assert.False(valueComparison);


            var start1 = new DateTime(2018, 01, 01, 08, 00, 00);
            var end1 = new DateTime(2018, 01, 02, 23, 00, 00);

            var start2True = new DateTime(2018, 01, 01, 08, 00, 00);
            var end2True = new DateTime(2018, 01, 02, 23, 00, 00);

            var start2False = new DateTime(2018, 01, 01, 02, 00, 00);
            var end2False = new DateTime(2018, 01, 01, 08, 00, 00);

            bool rangeOverlap = start1 < end2True && end1 > start2True;
            Assert.True(rangeOverlap);
            rangeOverlap = start1 < end2False && end1 > start2False;
            Assert.False(rangeOverlap);
        }

        [Fact]
        public void DemoRangeDate()
        {
            var start = new DateTime(2018, 01, 01);
            var end = new DateTime(2018, 01, 02);
            TimeSpan result = end - start;

            var valueTrue = new DateTime(2018, 01, 01);
            var valueFalse = new DateTime(2018, 01, 03);

            bool valueComparison = start <= valueTrue && end >= valueTrue;
            Assert.True(valueComparison);
            valueComparison = start <= valueFalse && end >= valueFalse;
            Assert.False(valueComparison);

            var start1 = new DateTime(2018, 01, 01);
            var end1 = new DateTime(2018, 01, 02);

            var start2True = new DateTime(2018, 01, 01);
            var end2True = new DateTime(2018, 01, 02);

            var start2False = new DateTime(2018, 01, 03);
            var end2False = new DateTime(2018, 01, 04);

            bool rangeOverlap = start1 <= end2True && end1 >= start2True;
            Assert.True(rangeOverlap);
            rangeOverlap = start1 <= end2False && end1 >= start2False;
            Assert.False(rangeOverlap);
        }

        [Fact]
        public void DemoRangeTime()
        {
            var start = new TimeSpan(00, 00, 00);
            var end = new TimeSpan(06, 00, 00);

            TimeSpan duration = start <= end ? end - start : end - start + TimeSpan.FromHours(24);
            Assert.Equal(new TimeSpan(06, 00, 00), duration);

            start = new TimeSpan(22, 00, 00);
            end = new TimeSpan(06, 00, 00);

            duration = start <= end ? end - start : end - start + TimeSpan.FromHours(24);
            Assert.Equal(new TimeSpan(08, 00, 00), duration);


            var valueTrue = new TimeSpan(05, 00, 00);
            var valueFalse = new TimeSpan(10, 00, 00);

            bool valueComparison = start <= end
                ? (start <= valueTrue && end > valueTrue)
                : (start <= valueTrue || end > valueTrue);
            Assert.True(valueComparison);

            valueComparison = start <= end
                ? (start <= valueFalse && end > valueFalse)
                : (start <= valueFalse || end > valueFalse);
            Assert.False(valueComparison);


            var start1 = new TimeSpan(00, 00, 00);
            var end1 = new TimeSpan(06, 00, 00);

            var start2True = new TimeSpan(01, 00, 00);
            var end2True = new TimeSpan(05, 00, 00);

            var start2False = new TimeSpan(06, 00, 00);
            var end2False = new TimeSpan(10, 00, 00);

            bool rangeOverlap = (start1 <= end1 && start2True <= end2True)
                ? (start1 < end2True && end1 > start2True)
                : (start1 < end2True || end1 > start2True || (start1 > end1 && start2True > end2True));
            Assert.True(rangeOverlap);

            rangeOverlap = (start1 <= end1 && start2False <= end2False)
                ? (start1 < end2False && end1 > start2False)
                : (start1 < end2False || end1 > start2False || (start1 > end1 && start2False > end2False));
            Assert.False(rangeOverlap);
        }
    }
}