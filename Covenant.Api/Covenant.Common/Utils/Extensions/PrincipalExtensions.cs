using Covenant.Common.Constants;
using System.Security.Claims;
using System.Security.Principal;

namespace Covenant.Common.Utils.Extensions;

public static class PrincipalExtensions
{
    public static bool IsCompanyUser(this IPrincipal user) =>
        user.IsInRole(CovenantConstants.Role.CompanyUser);

    private static bool IsAgencyPersonnel(this IPrincipal user) =>
        user.IsInRole(CovenantConstants.Role.AgencyPersonnel);

    public static bool IsPayrollManager(this IPrincipal user)
    {
        return user.IsInRole("payroll") || user.IsInRole("admin");
    }

    public static Guid GetCompanyId(this ClaimsPrincipal user)
    {
        string sub = user.IsCompanyUser()
            ? user.FindFirst(CovenantConstants.CompanyId)?.Value
            : user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(sub)) return Guid.Empty;
        Guid.TryParse(sub, out Guid id);
        return id;
    }

    public static Guid GetAgencyId(this ClaimsPrincipal user)
    {
        string sub = user.IsAgencyPersonnel()
            ? user.FindFirst(CovenantConstants.AgencyId)?.Value
            : user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(sub)) return Guid.Empty;
        Guid.TryParse(sub, out Guid id);
        return id;
    }

    public static List<Guid> GetAgencyIds(this ClaimsPrincipal user)
    {
        if (!user.IsAgencyPersonnel())
        {
            return new List<Guid>();
        }

        string agencyIdsString = user.FindFirst(CovenantConstants.AgencyIds)?.Value;

        if (string.IsNullOrEmpty(agencyIdsString))
        {
            // Fallback to single agency ID if AgencyIds claim is not present
            var agencyId = user.GetAgencyId();
            return agencyId != Guid.Empty ? new List<Guid> { agencyId } : new List<Guid>();
        }

        var agencyIds = agencyIdsString
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(id => Guid.TryParse(id.Trim(), out Guid parsedId) ? parsedId : Guid.Empty)
            .Where(id => id != Guid.Empty)
            .ToList();

        return agencyIds;
    }

    public static string GetNickname(this ClaimsPrincipal user) =>
        user.FindFirst("nickname")?.Value ?? user.Identity.Name ?? user.FindFirst("name")?.Value;

    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        user.TryGetUserId(out Guid id);
        return id;
    }

    public static bool TryGetUserId(this ClaimsPrincipal user, out Guid id)
    {
        id = Guid.Empty;
        string sub = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(sub)) return false;
        return Guid.TryParse(sub, out id);
    }
}
