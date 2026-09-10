using Covenant.Common.Utils.Extensions;
using Xunit;

namespace Covenant.Tests.Common;

public class DateExtensionsTest
{
    [Theory]
    [InlineData("2020-12-31", "2021-12-31", 53)]
    [InlineData("2020-01-01", "2021-01-01", 53)]
    [InlineData("2020-12-31", "2021-01-01", 1)]
    [InlineData("2020-12-27", "2021-01-02", 1)]
    [InlineData("2020-12-20", "2021-01-02", 2)]
    [InlineData("2020-12-13", "2021-01-02", 3)]
    [InlineData("2020-12-06", "2021-01-02", 4)]
    [InlineData("2020-12-06", "2021-01-09", 5)]
    [InlineData("2020-12-06", "2021-01-15", 6)]
    [InlineData("2020-01-03", "2021-01-09", 54)]
    [InlineData("2021-01-09", "2021-01-03", 0)]
    [InlineData("2021-01-18", "2021-01-30", 2)]
    [InlineData("2021-01-18", "2021-02-06", 3)]
    [InlineData("2021-01-18", "2021-02-13", 4)]
    [InlineData("2020-01-01", "2020-12-31", 53)]
    [InlineData("2021-01-01", "2021-12-31", 53)]
    public void GetNumberOfWeeksIn(string start, string end, int expectedWeeks)
    {
        int weeks = DateTime.Parse(start).GetNumberOfWeeksIn(DateTime.Parse(end));
        Assert.Equal(expectedWeeks, weeks);
    }

    [Theory]
    [InlineData("2020-12-27", "2021-01-01")]
    [InlineData("2020-12-28", "2021-01-01")]
    [InlineData("2020-12-29", "2021-01-01")]
    [InlineData("2020-12-30", "2021-01-01")]
    [InlineData("2020-12-31", "2021-01-01")]
    [InlineData("2021-01-01", "2021-01-01")]
    [InlineData("2021-01-02", "2021-01-01")]
    [InlineData("2021-01-03", "2021-01-8")]
    [InlineData("2021-01-04", "2021-01-8")]
    [InlineData("2021-01-05", "2021-01-8")]
    [InlineData("2021-01-06", "2021-01-8")]
    [InlineData("2021-01-07", "2021-01-8")]
    [InlineData("2021-01-08", "2021-01-8")]
    [InlineData("2021-01-09", "2021-01-8")]
    [InlineData("2021-01-10", "2021-01-15")]
    [InlineData("2021-01-11", "2021-01-15")]
    public void GetPaymentDateForInternalWorkers(string lastDateWorked, string expectedPaymentDate)
    {
        DateTime paymentDate = DateTime.Parse(lastDateWorked).GetPaymentDateForInternalWorkers();
        Assert.Equal(DateTime.Parse(expectedPaymentDate), paymentDate);
    }

    [Theory]
    [InlineData("2020-12-27", "2021-01-08")]
    [InlineData("2020-12-28", "2021-01-08")]
    [InlineData("2020-12-29", "2021-01-08")]
    [InlineData("2020-12-30", "2021-01-08")]
    [InlineData("2020-12-31", "2021-01-08")]
    [InlineData("2021-01-01", "2021-01-08")]
    [InlineData("2021-01-02", "2021-01-08")]
    [InlineData("2021-01-03", "2021-01-15")]
    [InlineData("2021-01-04", "2021-01-15")]
    [InlineData("2021-01-05", "2021-01-15")]
    [InlineData("2021-01-06", "2021-01-15")]
    [InlineData("2021-01-07", "2021-01-15")]
    [InlineData("2021-01-08", "2021-01-15")]
    [InlineData("2021-01-09", "2021-01-15")]
    [InlineData("2021-01-10", "2021-01-22")]
    [InlineData("2021-01-11", "2021-01-22")]
    public void GetPaymentDateForExternalWorkers(string lastDateWorked, string expectedPaymentDate)
    {
        DateTime paymentDate = DateTime.Parse(lastDateWorked).GetPaymentDateForExternalWorkers();
        Assert.Equal(DateTime.Parse(expectedPaymentDate), paymentDate);
    }

    [Theory]
    [InlineData("2026-09-06", "2026-09-06")]
    [InlineData("2026-09-09", "2026-09-06")]
    [InlineData("2026-09-12", "2026-09-06")]
    [InlineData("2026-01-03", "2025-12-28")]
    public void StartOfWeekSunday(string date, string expected) =>
        Assert.Equal(DateTime.Parse(expected), DateTime.Parse(date).StartOfWeekSunday());

    [Theory]
    [InlineData("2026-09-06", "2026-09-01")]
    [InlineData("2026-01-31", "2026-01-01")]
    public void StartOfMonth(string date, string expected) =>
        Assert.Equal(DateTime.Parse(expected), DateTime.Parse(date).StartOfMonth());

    [Theory]
    [InlineData("2026-09-06", "2026-07-01", 3)]
    [InlineData("2026-01-01", "2026-01-01", 1)]
    [InlineData("2026-06-30", "2026-04-01", 2)]
    [InlineData("2026-12-31", "2026-10-01", 4)]
    public void StartOfQuarter(string date, string expected, int expectedQuarter)
    {
        Assert.Equal(DateTime.Parse(expected), DateTime.Parse(date).StartOfQuarter());
        Assert.Equal(expectedQuarter, DateTime.Parse(date).Quarter());
    }
}