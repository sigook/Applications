using Xunit;

namespace Covenant.Integration.Tests.Utils;

public static class DateAssert
{
    public static void Equal(DateTime expected, DateTime actual) =>
        Assert.Equal(ToMicroseconds(expected), ToMicroseconds(actual));

    public static void Equal(DateTime? expected, DateTime? actual) =>
        Assert.Equal(expected.HasValue ? ToMicroseconds(expected.Value) : (DateTime?)null,
            actual.HasValue ? ToMicroseconds(actual.Value) : (DateTime?)null);

    private static DateTime ToMicroseconds(DateTime value) =>
        new(value.Ticks - value.Ticks % TimeSpan.TicksPerMicrosecond, value.Kind);
}
