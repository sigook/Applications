using System.Text.RegularExpressions;

namespace Covenant.Common.Utils.Extensions;

public static partial class EnumExtensions
{
    public static string ToDisplayName(this Enum value) => PascalCaseBoundary().Replace(value.ToString(), " $1");

    [GeneratedRegex("(?<=[a-z])([A-Z])")]
    private static partial Regex PascalCaseBoundary();
}
