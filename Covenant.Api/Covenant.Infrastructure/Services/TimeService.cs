using Covenant.Common.Interfaces;
using GeoTimeZone;

namespace Covenant.Infrastructure.Services;

public class TimeService : ITimeService
{
    public DateTime GetCurrentDateTime()
    {
        return DateTime.Now;
    }

    public DateTimeOffset GetCurrentDateTimeOffset()
    {
        return DateTimeOffset.Now;
    }

    public DateTimeOffset GetCurrentLocalDateTime(double latitude, double longitude)
    {
        var iana = TimeZoneLookup.GetTimeZone(latitude, longitude);
        var zone = TimeZoneInfo.FindSystemTimeZoneById(iana.Result);
        var result = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, zone);
        return result;
    }
}