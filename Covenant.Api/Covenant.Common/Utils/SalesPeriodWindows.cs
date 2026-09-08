using System.Globalization;
using Covenant.Common.Enums;
using Covenant.Common.Models.Company.SalesDashboard;
using Covenant.Common.Utils.Extensions;

namespace Covenant.Common.Utils;

public static class SalesPeriodWindows
{
    public static SalesPeriodWindow GetPeriodWindow(SalesPeriod period, DateTimeOffset now)
    {
        DateTime today = now.UtcDateTime.Date;
        (DateTime from, DateTime toExclusive) = period switch
        {
            SalesPeriod.Day => (today, today.AddDays(1)),
            SalesPeriod.Week => (today.StartOfWeekSunday(), today.StartOfWeekSunday().AddDays(7)),
            SalesPeriod.Month => (today.StartOfMonth(), today.StartOfMonth().AddMonths(1)),
            SalesPeriod.Quarter => (today.StartOfQuarter(), today.StartOfQuarter().AddMonths(3)),
            _ => throw new ArgumentOutOfRangeException(nameof(period))
        };
        DateTime to = toExclusive.AddDays(-1);
        return new SalesPeriodWindow
        {
            Range = new SalesPeriodRangeModel
            {
                Period = period,
                From = DateTime.SpecifyKind(from, DateTimeKind.Unspecified),
                To = DateTime.SpecifyKind(to, DateTimeKind.Unspecified),
                Label = GetLabel(period, from, to)
            },
            FromUtc = DateTime.SpecifyKind(from, DateTimeKind.Utc),
            ToUtcExclusive = DateTime.SpecifyKind(toExclusive, DateTimeKind.Utc)
        };
    }

    private static string GetLabel(SalesPeriod period, DateTime from, DateTime to) => period switch
    {
        SalesPeriod.Day => from.ToString("MMM d, yyyy", CultureInfo.InvariantCulture),
        SalesPeriod.Week => $"{from.ToString("MMM d", CultureInfo.InvariantCulture)} - {to.ToString("MMM d, yyyy", CultureInfo.InvariantCulture)}",
        SalesPeriod.Month => from.ToString("MMMM yyyy", CultureInfo.InvariantCulture),
        SalesPeriod.Quarter => $"Q{from.Quarter()} {from.Year}",
        _ => throw new ArgumentOutOfRangeException(nameof(period))
    };
}
