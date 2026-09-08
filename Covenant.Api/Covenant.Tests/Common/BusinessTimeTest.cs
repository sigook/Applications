using Covenant.Common.Enums;
using Covenant.Common.Utils;
using Xunit;

namespace Covenant.Tests.Common;

public class BusinessTimeTest
{
    private static DateTimeOffset Utc(string instant) =>
        DateTimeOffset.Parse(instant, null, System.Globalization.DateTimeStyles.AdjustToUniversal);

    [Fact]
    public void DayWindowUsesEasternCalendarDay()
    {
        // 2026-09-13T03:30Z is still Saturday 2026-09-12 at 23:30 in Eastern.
        var window = BusinessTime.GetPeriodWindow(SalesPeriod.Day, Utc("2026-09-13T03:30:00Z"));

        Assert.Equal(new DateTime(2026, 9, 12), window.Range.From);
        Assert.Equal(new DateTime(2026, 9, 12), window.Range.To);
        Assert.Equal(new DateTime(2026, 9, 12, 4, 0, 0, DateTimeKind.Utc), window.FromUtc);
        Assert.Equal(new DateTime(2026, 9, 13, 4, 0, 0, DateTimeKind.Utc), window.ToUtcExclusive);
        Assert.Equal("Sep 12, 2026", window.Range.Label);
    }

    [Fact]
    public void WeekWindowRunsSundayToSaturday()
    {
        var window = BusinessTime.GetPeriodWindow(SalesPeriod.Week, Utc("2026-09-13T03:30:00Z"));

        Assert.Equal(new DateTime(2026, 9, 6), window.Range.From);
        Assert.Equal(new DateTime(2026, 9, 12), window.Range.To);
        Assert.Equal(new DateTime(2026, 9, 6, 4, 0, 0, DateTimeKind.Utc), window.FromUtc);
        Assert.Equal(new DateTime(2026, 9, 13, 4, 0, 0, DateTimeKind.Utc), window.ToUtcExclusive);
        Assert.Equal("Sep 6 - Sep 12, 2026", window.Range.Label);
    }

    [Fact]
    public void WeekWindowCrossingSpringForwardKeepsSevenLocalDays()
    {
        // DST starts Sunday 2026-03-08: the week opens at 05:00Z (EST) and closes at 04:00Z (EDT).
        var window = BusinessTime.GetPeriodWindow(SalesPeriod.Week, Utc("2026-03-10T15:00:00Z"));

        Assert.Equal(new DateTime(2026, 3, 8, 5, 0, 0, DateTimeKind.Utc), window.FromUtc);
        Assert.Equal(new DateTime(2026, 3, 15, 4, 0, 0, DateTimeKind.Utc), window.ToUtcExclusive);
    }

    [Fact]
    public void WeekWindowCrossingFallBackKeepsSevenLocalDays()
    {
        // DST ends Sunday 2026-11-01: the week opens at 04:00Z (EDT) and closes at 05:00Z (EST).
        var window = BusinessTime.GetPeriodWindow(SalesPeriod.Week, Utc("2026-11-03T15:00:00Z"));

        Assert.Equal(new DateTime(2026, 11, 1, 4, 0, 0, DateTimeKind.Utc), window.FromUtc);
        Assert.Equal(new DateTime(2026, 11, 8, 5, 0, 0, DateTimeKind.Utc), window.ToUtcExclusive);
    }

    [Fact]
    public void MonthWindowCoversTheCalendarMonth()
    {
        var window = BusinessTime.GetPeriodWindow(SalesPeriod.Month, Utc("2026-09-13T15:00:00Z"));

        Assert.Equal(new DateTime(2026, 9, 1), window.Range.From);
        Assert.Equal(new DateTime(2026, 9, 30), window.Range.To);
        Assert.Equal(new DateTime(2026, 10, 1, 4, 0, 0, DateTimeKind.Utc), window.ToUtcExclusive);
        Assert.Equal("September 2026", window.Range.Label);
    }

    [Fact]
    public void QuarterWindowCoversTheCalendarQuarter()
    {
        var window = BusinessTime.GetPeriodWindow(SalesPeriod.Quarter, Utc("2026-09-13T15:00:00Z"));

        Assert.Equal(new DateTime(2026, 7, 1), window.Range.From);
        Assert.Equal(new DateTime(2026, 9, 30), window.Range.To);
        Assert.Equal("Q3 2026", window.Range.Label);
    }

    [Fact]
    public void LastEasternHalfHourOfTheYearStaysInTheFourthQuarter()
    {
        // 2027-01-01T04:30Z is 2026-12-31 at 23:30 in Eastern.
        var window = BusinessTime.GetPeriodWindow(SalesPeriod.Quarter, Utc("2027-01-01T04:30:00Z"));

        Assert.Equal("Q4 2026", window.Range.Label);
        Assert.Equal(new DateTime(2026, 10, 1), window.Range.From);
    }

    [Fact]
    public void BoundsAreUtcAndRangeDatesAreUnspecified()
    {
        var window = BusinessTime.GetPeriodWindow(SalesPeriod.Week, Utc("2026-09-13T03:30:00Z"));

        Assert.Equal(DateTimeKind.Utc, window.FromUtc.Kind);
        Assert.Equal(DateTimeKind.Utc, window.ToUtcExclusive.Kind);
        Assert.Equal(DateTimeKind.Unspecified, window.Range.From.Kind);
        Assert.Equal(DateTimeKind.Unspecified, window.Range.To.Kind);
        Assert.Equal("America/New_York", window.Range.TimeZone);
    }
}
