using System.Globalization;
using Covenant.Common.Enums;
using Covenant.Common.Utils;
using Xunit;

namespace Covenant.Tests.Common;

public class SalesPeriodWindowsTest
{
    private static DateTimeOffset Utc(string instant) =>
        DateTimeOffset.Parse(instant, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

    [Fact]
    public void DayWindowUsesTheUtcCalendarDay()
    {
        var window = SalesPeriodWindows.GetPeriodWindow(SalesPeriod.Day, Utc("2026-09-13T03:30:00Z"));

        Assert.Equal(new DateTime(2026, 9, 13), window.Range.From);
        Assert.Equal(new DateTime(2026, 9, 13), window.Range.To);
        Assert.Equal(new DateTime(2026, 9, 13, 0, 0, 0, DateTimeKind.Utc), window.FromUtc);
        Assert.Equal(new DateTime(2026, 9, 14, 0, 0, 0, DateTimeKind.Utc), window.ToUtcExclusive);
        Assert.Equal("Sep 13, 2026", window.Range.Label);
    }

    [Fact]
    public void WeekWindowRunsSundayToSaturday()
    {
        var window = SalesPeriodWindows.GetPeriodWindow(SalesPeriod.Week, Utc("2026-09-09T15:00:00Z"));

        Assert.Equal(new DateTime(2026, 9, 6), window.Range.From);
        Assert.Equal(new DateTime(2026, 9, 12), window.Range.To);
        Assert.Equal(new DateTime(2026, 9, 6, 0, 0, 0, DateTimeKind.Utc), window.FromUtc);
        Assert.Equal(new DateTime(2026, 9, 13, 0, 0, 0, DateTimeKind.Utc), window.ToUtcExclusive);
        Assert.Equal("Sep 6 - Sep 12, 2026", window.Range.Label);
    }

    [Fact]
    public void WeekWindowStartsOnTheUtcSundayEvenLateAtNight()
    {
        // 2026-09-13T03:30Z is a Sunday in UTC, so it opens a new week.
        var window = SalesPeriodWindows.GetPeriodWindow(SalesPeriod.Week, Utc("2026-09-13T03:30:00Z"));

        Assert.Equal(new DateTime(2026, 9, 13), window.Range.From);
        Assert.Equal(new DateTime(2026, 9, 19), window.Range.To);
        Assert.Equal("Sep 13 - Sep 19, 2026", window.Range.Label);
    }

    [Theory]
    [InlineData("2026-03-10T15:00:00Z", 2026, 3, 8)]
    [InlineData("2026-11-03T15:00:00Z", 2026, 11, 1)]
    public void WeekWindowIsAlwaysSevenTwentyFourHourDays(string instant, int year, int month, int day)
    {
        var window = SalesPeriodWindows.GetPeriodWindow(SalesPeriod.Week, Utc(instant));

        Assert.Equal(new DateTime(year, month, day), window.Range.From);
        Assert.Equal(TimeSpan.FromDays(7), window.ToUtcExclusive - window.FromUtc);
        Assert.Equal(0, window.FromUtc.TimeOfDay.Ticks);
        Assert.Equal(0, window.ToUtcExclusive.TimeOfDay.Ticks);
    }

    [Fact]
    public void MonthWindowCoversTheCalendarMonth()
    {
        var window = SalesPeriodWindows.GetPeriodWindow(SalesPeriod.Month, Utc("2026-09-13T15:00:00Z"));

        Assert.Equal(new DateTime(2026, 9, 1), window.Range.From);
        Assert.Equal(new DateTime(2026, 9, 30), window.Range.To);
        Assert.Equal(new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc), window.FromUtc);
        Assert.Equal(new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc), window.ToUtcExclusive);
        Assert.Equal("September 2026", window.Range.Label);
    }

    [Fact]
    public void QuarterWindowCoversTheCalendarQuarter()
    {
        var window = SalesPeriodWindows.GetPeriodWindow(SalesPeriod.Quarter, Utc("2026-09-13T15:00:00Z"));

        Assert.Equal(new DateTime(2026, 7, 1), window.Range.From);
        Assert.Equal(new DateTime(2026, 9, 30), window.Range.To);
        Assert.Equal(new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc), window.ToUtcExclusive);
        Assert.Equal("Q3 2026", window.Range.Label);
    }

    [Fact]
    public void FirstUtcMinutesOfTheYearOpenTheFirstQuarter()
    {
        var window = SalesPeriodWindows.GetPeriodWindow(SalesPeriod.Quarter, Utc("2027-01-01T04:30:00Z"));

        Assert.Equal("Q1 2027", window.Range.Label);
        Assert.Equal(new DateTime(2027, 1, 1), window.Range.From);
        Assert.Equal(new DateTime(2027, 3, 31), window.Range.To);
    }

    [Fact]
    public void SameInstantInAnyOffsetGivesTheSameWindow()
    {
        // One instant, 2026-09-13T04:30Z, written from three machines in different zones.
        var utc = SalesPeriodWindows.GetPeriodWindow(SalesPeriod.Day, Utc("2026-09-13T04:30:00Z"));
        var eastern = SalesPeriodWindows.GetPeriodWindow(
            SalesPeriod.Day, new DateTimeOffset(2026, 9, 13, 0, 30, 0, TimeSpan.FromHours(-4)));
        var india = SalesPeriodWindows.GetPeriodWindow(
            SalesPeriod.Day, new DateTimeOffset(2026, 9, 13, 10, 0, 0, TimeSpan.FromHours(5.5)));

        Assert.Equal(utc.Range.From, eastern.Range.From);
        Assert.Equal(utc.Range.From, india.Range.From);
        Assert.Equal(utc.FromUtc, eastern.FromUtc);
        Assert.Equal(utc.FromUtc, india.FromUtc);
        Assert.Equal(new DateTime(2026, 9, 13), utc.Range.From);
    }

    [Fact]
    public void LocalMidnightEastOfUtcFallsInThePreviousUtcDay()
    {
        // The machine reads 2026-09-13T00:30+05:30, which is still 2026-09-12 in UTC.
        var window = SalesPeriodWindows.GetPeriodWindow(
            SalesPeriod.Day, new DateTimeOffset(2026, 9, 13, 0, 30, 0, TimeSpan.FromHours(5.5)));

        Assert.Equal(new DateTime(2026, 9, 12), window.Range.From);
    }

    [Fact]
    public void WeekWindowSpanningTwoMonths()
    {
        var window = SalesPeriodWindows.GetPeriodWindow(SalesPeriod.Week, Utc("2026-04-01T09:00:00Z"));

        Assert.Equal(new DateTime(2026, 3, 29), window.Range.From);
        Assert.Equal(new DateTime(2026, 4, 4), window.Range.To);
        Assert.Equal("Mar 29 - Apr 4, 2026", window.Range.Label);
    }

    [Fact]
    public void WeekWindowSpanningTwoYearsKeepsSevenDaysAndLabelsTheEndYear()
    {
        var window = SalesPeriodWindows.GetPeriodWindow(SalesPeriod.Week, Utc("2026-12-31T09:00:00Z"));

        Assert.Equal(new DateTime(2026, 12, 27), window.Range.From);
        Assert.Equal(new DateTime(2027, 1, 2), window.Range.To);
        Assert.Equal(new DateTime(2027, 1, 3, 0, 0, 0, DateTimeKind.Utc), window.ToUtcExclusive);
        // The label carries only the closing year, as it always has.
        Assert.Equal("Dec 27 - Jan 2, 2027", window.Range.Label);
    }

    [Theory]
    [InlineData("2026-02-14T09:00:00Z", 2026, 28)]
    [InlineData("2028-02-14T09:00:00Z", 2028, 29)]
    public void MonthWindowCoversFebruaryIncludingLeapYears(string instant, int year, int lastDay)
    {
        var window = SalesPeriodWindows.GetPeriodWindow(SalesPeriod.Month, Utc(instant));

        Assert.Equal(new DateTime(year, 2, 1), window.Range.From);
        Assert.Equal(new DateTime(year, 2, lastDay), window.Range.To);
        Assert.Equal(new DateTime(year, 3, 1, 0, 0, 0, DateTimeKind.Utc), window.ToUtcExclusive);
        Assert.Equal($"February {year}", window.Range.Label);
    }

    [Fact]
    public void DayWindowOnTheLastDayOfAQuarterStaysInThatQuarter()
    {
        var instant = Utc("2026-09-30T23:59:59Z");

        var day = SalesPeriodWindows.GetPeriodWindow(SalesPeriod.Day, instant);
        var quarter = SalesPeriodWindows.GetPeriodWindow(SalesPeriod.Quarter, instant);

        Assert.Equal(new DateTime(2026, 9, 30), day.Range.From);
        Assert.Equal(new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc), day.ToUtcExclusive);
        Assert.Equal("Q3 2026", quarter.Range.Label);
        Assert.Equal(new DateTime(2026, 9, 30), quarter.Range.To);
    }

    [Fact]
    public void BoundsAreUtcAndRangeDatesAreUnspecified()
    {
        var window = SalesPeriodWindows.GetPeriodWindow(SalesPeriod.Week, Utc("2026-09-09T15:00:00Z"));

        Assert.Equal(DateTimeKind.Utc, window.FromUtc.Kind);
        Assert.Equal(DateTimeKind.Utc, window.ToUtcExclusive.Kind);
        Assert.Equal(DateTimeKind.Unspecified, window.Range.From.Kind);
        Assert.Equal(DateTimeKind.Unspecified, window.Range.To.Kind);
    }

    [Fact]
    public void UnknownPeriodThrows()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => SalesPeriodWindows.GetPeriodWindow((SalesPeriod)99, Utc("2026-09-09T15:00:00Z")));
    }
}
