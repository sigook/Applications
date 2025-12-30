namespace Covenant.Common.Interfaces;

public interface ITimeService
{
    DateTime GetCurrentDateTime();

    DateTimeOffset GetCurrentDateTimeOffset();

    DateTimeOffset GetCurrentLocalDateTime(double latitude, double longitude);
}