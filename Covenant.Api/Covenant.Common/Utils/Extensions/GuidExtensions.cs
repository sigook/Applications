namespace Covenant.Common.Utils.Extensions;

public static class GuidExtensions
{
    public static bool Empty(this Guid guid) => guid == Guid.Empty;
    public static bool Empty(this Guid? guid) => guid == null || guid == Guid.Empty;
    public static bool NotEmpty(this Guid guid) => guid != Guid.Empty;
}
