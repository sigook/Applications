using Covenant.Common.Constants;
using System.Security.Claims;
using System.Security.Principal;

namespace Covenant.Common.Utils.Extensions;

public static class PrincipalExtensions
{
    public static bool IsCompanyUser(this IPrincipal user) =>
        user.IsInRole(CovenantConstants.Role.CompanyUser);

    public static bool IsAgencyStaff(this IPrincipal user) =>
        CovenantConstants.Role.AgencyStaff.Any(user.IsInRole);

    public static bool IsAdmin(this IPrincipal user) =>
        CovenantConstants.Role.AdminAccess.Any(user.IsInRole);

    public static bool IsSales(this IPrincipal user) =>
        user.IsInRole(CovenantConstants.Role.Sales);

    public static bool IsSuperAdmin(this IPrincipal user) =>
        user.IsInRole(CovenantConstants.Role.SuperAdmin);

    public static string GetSubject(this ClaimsPrincipal user) =>
        user.FindFirst(IdentityClaims.Subject)?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public static Guid GetCompanyId(this ClaimsPrincipal user)
    {
        string sub = user.IsCompanyUser()
            ? user.FindFirst(CovenantConstants.CompanyId)?.Value
            : user.GetSubject();
        if (string.IsNullOrEmpty(sub)) return Guid.Empty;
        Guid.TryParse(sub, out Guid id);
        return id;
    }

    public static Guid GetAgencyId(this ClaimsPrincipal user)
    {
        string sub = user.IsAgencyStaff()
            ? user.FindFirst(CovenantConstants.AgencyId)?.Value
            : user.GetSubject();
        if (string.IsNullOrEmpty(sub)) return Guid.Empty;
        Guid.TryParse(sub, out Guid id);
        return id;
    }

    public static List<Guid> GetAgencyIds(this ClaimsPrincipal user)
    {
        if (!user.IsAgencyStaff()) return [];

        var agencyIdsClaim = user.FindFirst(CovenantConstants.AgencyIds);
        if (agencyIdsClaim is null)
        {
            var agencyId = user.GetAgencyId();
            return agencyId != Guid.Empty ? [agencyId] : [];
        }

        return [.. agencyIdsClaim.Value
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(id => Guid.TryParse(id.Trim(), out Guid parsedId) ? parsedId : Guid.Empty)
            .Where(id => id != Guid.Empty)];
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
        string sub = user.GetSubject();
        if (string.IsNullOrEmpty(sub)) return false;
        return Guid.TryParse(sub, out id);
    }
}
