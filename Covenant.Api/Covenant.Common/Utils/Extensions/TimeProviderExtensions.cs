using GeoTimeZone;

namespace Covenant.Common.Utils.Extensions;

public static class TimeProviderExtensions
{
    public static DateTimeOffset GetLocalNow(this TimeProvider timeProvider, double latitude, double longitude)
    {
        var iana = TimeZoneLookup.GetTimeZone(latitude, longitude);
        var zone = TimeZoneInfo.FindSystemTimeZoneById(iana.Result);
        return TimeZoneInfo.ConvertTime(timeProvider.GetUtcNow(), zone);
    }

    public static DateTimeOffset? GetLocalNow(this TimeProvider timeProvider, string timeZoneId)
    {
        if (string.IsNullOrWhiteSpace(timeZoneId) || !TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out var zone))
            return null;
        return TimeZoneInfo.ConvertTime(timeProvider.GetUtcNow(), zone);
    }
}
