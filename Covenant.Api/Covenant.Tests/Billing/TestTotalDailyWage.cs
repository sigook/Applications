using System;
using Covenant.Common.Configuration;
using Covenant.TimeSheetTotal.Models;
using Xunit;

namespace Covenant.Tests.Billing
{
	public class TestTotalDailyWage
	{
		[Fact]
		public void Total()
		{
			var rates = new Rates {Holiday = 2, NightShift = 2, OverTime = 2};
			const int workerRate = 1;
			const int missingRateWorker = 2;
			TimeSpan missingHours = TimeSpan.FromHours(1);
			TimeSpan missingOvertimeHours = TimeSpan.FromHours(2);
			TimeSpan regularHours = TimeSpan.FromHours(3);
			TimeSpan otherRegularHours = TimeSpan.FromHours(4);
			TimeSpan nightShiftHours = TimeSpan.FromHours(5);
			TimeSpan holidayHours = TimeSpan.FromHours(6);
			TimeSpan overtimeHours = TimeSpan.FromHours(7);
			var sub = new TotalDailyWage(rates, workerRate, missingRateWorker, missingHours,
				missingOvertimeHours, regularHours, otherRegularHours,
				nightShiftHours, holidayHours, overtimeHours);

			const decimal eRegular = 3;
			const decimal eOtherRegular = 4;
			const decimal eOvertime = 14;
			const decimal eHoliday = 12;
			const decimal eNightShift = 10;
			const decimal eMissing = 2;
			const decimal eMissingOvertime = 8;
			decimal eTotalGross = eRegular + eOtherRegular + eOvertime + eHoliday
			                      + eNightShift + eMissing + eMissingOvertime;

			Assert.Equal(workerRate, sub.WorkerRate);
			Assert.Equal(2, sub.HolidayRate);
			Assert.Equal(2, sub.OvertimeRate);
			Assert.Equal(2, sub.NightShiftRate);
			Assert.Equal(missingRateWorker, sub.MissingRateWorker);
			Assert.Equal(4, sub.MissingOvertimeRate);
			Assert.Equal(missingHours, sub.MissingHours);
			Assert.Equal(missingOvertimeHours, sub.MissingOvertimeHours);
			Assert.Equal(regularHours, sub.RegularHours);
			Assert.Equal(otherRegularHours, sub.OtherRegularHours);
			Assert.Equal(nightShiftHours, sub.NightShiftHours);
			Assert.Equal(holidayHours, sub.HolidayHours);
			Assert.Equal(overtimeHours, sub.OvertimeHours);
			Assert.Equal(eRegular, sub.Regular);
			Assert.Equal(eOtherRegular, sub.OtherRegular);
			Assert.Equal(eOvertime, sub.Overtime);
			Assert.Equal(eHoliday, sub.Holiday);
			Assert.Equal(eNightShift, sub.NightShift);
			Assert.Equal(eMissing, sub.Missing);
			Assert.Equal(eMissingOvertime, sub.MissingOvertime);
			Assert.Equal(eTotalGross, sub.TotalGross);
		}

		[Fact]
		public void When_Missing_Rate_Is_Zero_Use_Regular_Rate()
		{
			const int workerRate = 15;
			var sub = new TotalDailyWage(Rates.DefaultRates, workerRate,
				default, default, default,
				default, default, default,
				default, default);
			Assert.Equal(workerRate, sub.WorkerRate);
			Assert.Equal(workerRate, sub.MissingRateWorker);

			const int missingRateWorker = 20;
			sub = new TotalDailyWage(Rates.DefaultRates, workerRate,
				missingRateWorker, default, default,
				default, default, default,
				default, default);
			Assert.Equal(workerRate, sub.WorkerRate);
			Assert.Equal(missingRateWorker, sub.MissingRateWorker);
		}
	}
}