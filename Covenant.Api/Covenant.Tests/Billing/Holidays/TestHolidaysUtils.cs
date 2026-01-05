using Covenant.Common.Utils.Extensions;
using Xunit;

namespace Covenant.Tests.Billing.Holidays
{
    public class TestHolidaysUtils
    {
        [Theory]
        [InlineData("2020-07-20", "2020-07-18")]//Monday
        [InlineData("2020-07-21", "2020-07-18")]//Tuesday
        [InlineData("2020-07-22", "2020-07-18")]//Wednesday
        [InlineData("2020-07-23", "2020-07-18")]//Thursday
        [InlineData("2020-07-24", "2020-07-18")]//Friday
        [InlineData("2020-07-25", "2020-07-18")]//Saturday
        [InlineData("2020-07-26", "2020-07-18")]//Sunday
        public void GetEnd(string holidayS, string endS)
        {
            DateTime holiday = DateTime.Parse(holidayS);
            DateTime end = holiday.GetEnd();
            Assert.Equal(DateTime.Parse(endS), end);
        }

        [Fact]
        public void GetStart()
        {
            DateTime start = new DateTime(2020, 08, 29).GetStart();
            Assert.Equal(new DateTime(2020, 08, 02), start);
        }

        [Fact]
        public void GetRangeOfDaysWorkerMustWorkToReceiveHolidayPay()
        {
            var holiday = new DateTime(2019, 01, 01);
            IEnumerable<DateTime> range = holiday.GetRangeOfDaysWorkerMustWorkToReceiveHolidayPay();
            Assert.Equal(new[]
            {
                holiday,
                new DateTime(2019,01,02),
                new DateTime(2018,12,31)
            }, range);
        }
    }
}