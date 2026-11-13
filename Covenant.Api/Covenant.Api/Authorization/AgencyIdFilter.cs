using Covenant.Common.Constants;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Covenant.Api.Authorization;

public class AgencyIdFilter : IActionFilter
{
    private static readonly string[] AgencyClaimTypes =
    [
        CovenantConstants.AgencyId,
        CovenantConstants.AgencyIds
    ];

    public void OnActionExecuting(ActionExecutingContext context) => AddAgencyClaims(context);

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }

    private static void AddAgencyClaims(ActionExecutingContext context)
    {
        if (context.Controller is not ControllerBase controller || !controller.User.IsAgencyStaff()) return;

        List<Guid> accessible = [.. ParseIds(controller.User.FindAll(CovenantConstants.AgencyId).Select(c => c.Value)).Distinct()];

        var query = context.HttpContext.Request.Query;
        var requested = ParseIds(query[CovenantConstants.AgencyIds].SelectMany(v => (v ?? string.Empty).Split(','))).ToHashSet();
        List<Guid> selected = query.ContainsKey(CovenantConstants.AgencyIds)
            ? [.. accessible.Where(requested.Contains)]
            : accessible;

        var requestedAgency = ParseIds(query[CovenantConstants.AgencyId]).FirstOrDefault();
        var agencyId = accessible.Contains(requestedAgency)
            ? requestedAgency
            : accessible.FirstOrDefault();

        foreach (var identity in controller.User.Identities)
        {
            foreach (var claim in identity.Claims.Where(c => AgencyClaimTypes.Contains(c.Type)).ToList())
            {
                identity.TryRemoveClaim(claim);
            }
        }

        List<Claim> claims =
        [
            new(CovenantConstants.AgencyId, agencyId.ToString()),
            new(CovenantConstants.AgencyIds, string.Join(",", selected))
        ];
        controller.User.AddIdentity(new ClaimsIdentity(claims));
    }

    private static IEnumerable<Guid> ParseIds(IEnumerable<string> values) =>
        values.Select(v => Guid.TryParse(v?.Trim(), out var id) ? id : Guid.Empty).Where(id => id != Guid.Empty);
}
