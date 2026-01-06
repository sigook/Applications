using Covenant.Common.Constants;
using Covenant.Common.Enums;

namespace Covenant.Common.Utils.Extensions;

public static class EnumExtensions
{
    public static string ToCovenantRole(this UserType userType)
    {
        return userType switch
        {
            UserType.AgencyPersonnel => CovenantConstants.Role.AgencyPersonnel,
            UserType.Worker => CovenantConstants.Role.Worker,
            UserType.Company => CovenantConstants.Role.Company,
            UserType.CompanyUser => CovenantConstants.Role.CompanyUser,
            UserType.Agency => CovenantConstants.Role.Agency,
            _ => throw new ArgumentException("No role specified")
        };
    }
}
