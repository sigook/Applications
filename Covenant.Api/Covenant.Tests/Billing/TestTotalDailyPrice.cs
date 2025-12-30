using System;
using Covenant.Common.Configuration;
using Covenant.TimeSheetTotal.Models;
using Xunit;

namespace Covenant.Tests.Billing
{
	public class TestTotalDailyPrice
	{
		[Fact]
		public void Total()
		{
			var rates = new Rates {OverTime = 2, Holiday = 2, NightShift = 2};
			const int agencyRate = 1;
			const int missingRateAgency = 2;

			TimeSpan regularHours = TimeSpan.FromHours(1);
			TimeSpan otherRegularHours = TimeSpan.FromHours(2);
			TimeSpan missingHours = TimeSpan.FromHours(3);
			TimeSpan missingHoursOvertime = TimeSpan.FromHours(4);
			TimeSpan nightShiftHours = TimeSpan.FromHours(5);
			TimeSpan holidayHours = TimeSpan.FromHours(6);
			TimeSpan overtimeHours = TimeSpan.FromHours(7);

			const int expectedRegular = 1;
			const int expectedOtherRegular = 2;
			const int expectedMissing = 6;
			const int expectedMissingOvertime = 16;
			const int expectedNightShift = 10;
			const int expectedHoliday = 12;
			const int expectedOvertime = 14;
			const int expectedTotalGross = expectedRegular + expectedOtherRegular + expectedMissing + expectedMissingOvertime
			                               + expectedNightShift + expectedHoliday + expectedOvertime;
			const int expectedTotalNet = expectedRegular + expectedOtherRegular + expectedMissing + expectedMissingOvertime
			                             + expectedNightShift + expectedHoliday + expectedOvertime;

			var sub = new TotalDailyPrice(rates, agencyRate, missingRateAgency,
				missingHours,
				missingHoursOvertime,
				regularHours,
				otherRegularHours,
				nightShiftHours,
				holidayHours,
				overtimeHours);

			Assert.Equal(agencyRate, sub.AgencyRate);
			Assert.Equal(missingRateAgency, sub.MissingRateAgency);
			Assert.Equal(missingHours, sub.MissingHours);
			Assert.Equal(missingHoursOvertime, sub.MissingHoursOvertime);
			Assert.Equal(regularHours, sub.RegularHours);
			Assert.Equal(otherRegularHours, sub.OtherRegularHours);
			Assert.Equal(nightShiftHours, sub.NightShiftHours);
			Assert.Equal(holidayHours, sub.HolidayHours);
			Assert.Equal(overtimeHours, sub.OvertimeHours);

			Assert.Equal(expectedRegular, sub.Regular);
			Assert.Equal(expectedOtherRegular, sub.OtherRegular);
			Assert.Equal(expectedMissing, sub.Missing);
			Assert.Equal(expectedMissingOvertime, sub.MissingOvertime);
			Assert.Equal(expectedNightShift, sub.NightShift);
			Assert.Equal(expectedHoliday, sub.Holiday);
			Assert.Equal(expectedOvertime, sub.Overtime);
			Assert.Equal(expectedTotalGross, sub.TotalGross);
			Assert.Equal(expectedTotalNet, sub.TotalNet);
		}

		[Fact]
		public void When_Missing_Rate_Is_Zero_Use_Regular_Rate()
		{
			const int agencyRate = 15;
			var sub = new TotalDailyPrice(Rates.DefaultRates, agencyRate,
				default, default, default,
				default, default, default,
				default, default);
			Assert.Equal(agencyRate, sub.AgencyRate);
			Assert.Equal(agencyRate, sub.MissingRateAgency);

			const int missingRateAgency = 20;
			sub = new TotalDailyPrice(Rates.DefaultRates, agencyRate,
				missingRateAgency, default, default,
				default, default, default,
				default, default);
			Assert.Equal(agencyRate, sub.AgencyRate);
			Assert.Equal(missingRateAgency, sub.MissingRateAgency);
		}
	}
}