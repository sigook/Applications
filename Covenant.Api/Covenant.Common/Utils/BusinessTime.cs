using System.Globalization;
using Covenant.Common.Constants;
using Covenant.Common.Enums;
using Covenant.Common.Models.Company.SalesDashboard;
using Covenant.Common.Utils.Extensions;

namespace Covenant.Common.Utils;

public static class BusinessTime
{
    public static readonly TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById(CovenantConstants.BusinessTimeZoneId);

    public static DateTime ToBusinessTime(DateTimeOffset instant) =>
        DateTime.SpecifyKind(TimeZoneInfo.ConvertTime(instant, Zone).DateTime, DateTimeKind.Unspecified);

    public static DateTime ToUtc(DateTime businessLocal) =>
        TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(businessLocal, DateTimeKind.Unspecified), Zone);

    public static SalesPeriodWindow GetPeriodWindow(SalesPeriod period, DateTimeOffset now)
    {
        DateTime today = ToBusinessTime(now).Date;
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
                From = from,
                To = to,
                Label = GetLabel(period, from, to),
                TimeZone = CovenantConstants.BusinessTimeZoneId
            },
            FromUtc = ToUtc(from),
            ToUtcExclusive = ToUtc(toExclusive)
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
