using System.Text.Json.Serialization;

namespace Covenant.Common.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WorkerRequirement
{
    ProfileImage = 1,
    TaxCategory = 2,
    SocialInsuranceNumber = 3,
    SocialInsuranceFile = 4,
    SocialInsuranceExpired = 5,
    Identification = 6
}
