using Covenant.Common.Configuration;
using Covenant.Common.Entities.Request;
using Covenant.Subcontractor.Utils;
using Covenant.TimeSheetTotal.Models;
using Xunit;

namespace Covenant.Tests.Billing;

public class TestSubcontractorMappers
{
    [Fact]
    public void ToSubcontractorWageDetail()
    {
        var total = new TotalDailyWage(Rates.DefaultRates, 1, 2, TimeSpan.FromHours(11),
            TimeSpan.FromHours(12),
            TimeSpan.FromHours(13),
            TimeSpan.FromHours(14),
            TimeSpan.FromHours(15),
            TimeSpan.FromHours(16),
            TimeSpan.FromHours(17));
        var tst = TimeSheetTotalPayroll.CreateTotal(Guid.NewGuid(),
            TimeSpan.FromHours(1),
            TimeSpan.FromHours(2),
            TimeSpan.FromHours(3),
            TimeSpan.FromHours(4),
            TimeSpan.FromHours(5));
        var result = total.ToSubcontractorWageDetail(tst);
        Assert.Equal(total.WorkerRate, result.WorkerRate);
        Assert.Equal(total.Regular, result.Regular);
        Assert.Equal(total.OtherRegular, result.OtherRegular);
        Assert.Equal(total.Missing, result.Missing);
        Assert.Equal(total.MissingOvertime, result.MissingOvertime);
        Assert.Equal(total.NightShift, result.NightShift);
        Assert.Equal(total.Holiday, result.Holiday);
        Assert.Equal(total.Overtime, result.Overtime);
        Assert.Equal(tst, result.TimeSheetTotal);
    }

    [Fact]
    public async Task ForEachHoliday()
    {
        var holidays = new List<DateTime> { new DateTime(2019, 01, 01), new DateTime(2019, 01, 02) };
        IReadOnlyCollection<string> result = await holidays.ForEachHoliday(async h =>
        {
            await Task.CompletedTask;
            return h.Day == 1 ? null : h.Day.ToString();
        });
        Assert.Equal(new[] { "2" }, result);
    }
}