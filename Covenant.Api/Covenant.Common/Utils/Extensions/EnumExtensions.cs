using Covenant.Common.Enums;
using System.Text.RegularExpressions;

namespace Covenant.Common.Utils.Extensions;

public static partial class EnumExtensions
{
    public static string ToDisplayName(this Enum value) => PascalCaseBoundary().Replace(value.ToString(), " $1");

    public static bool IsSocialInsuranceIssue(this WorkerRequirement requirement) =>
        requirement is WorkerRequirement.SocialInsuranceNumber or WorkerRequirement.SocialInsuranceExpired;

    public static string ToMessage(this WorkerRequirement requirement) => requirement switch
    {
        WorkerRequirement.ProfileImage => "Worker doesn't have profile image",
        WorkerRequirement.TaxCategory => "Taxes classes have not been configurated",
        WorkerRequirement.SocialInsuranceNumber => "Worker doesn't have social insurance number",
        WorkerRequirement.SocialInsuranceFile => "Worker doesn't have social insurance document",
        WorkerRequirement.SocialInsuranceExpired => "Social insurance has expired",
        WorkerRequirement.Identification => "Worker doesn't have an identification document",
        _ => requirement.ToString()
    };

    [GeneratedRegex("(?<=[a-z])([A-Z])")]
    private static partial Regex PascalCaseBoundary();
}
