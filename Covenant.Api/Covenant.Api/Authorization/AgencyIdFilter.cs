using Covenant.Common.Constants;
using Covenant.Common.Repositories.Agencies;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Covenant.Api.Authorization;

/// <summary>
/// Adds the agencyId, agencyIds and agencyPersonnelId claims to agency staff, replacing whatever the token carried.
/// </summary>
public class AgencyIdFilter(IAgencyRepository repository) : IAsyncActionFilter
{
    private static readonly string[] AgencyClaimTypes =
    [
        CovenantConstants.AgencyId,
        CovenantConstants.AgencyIds,
        CovenantConstants.AgencyPersonnelId
    ];

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await AddAgencyClaims(context);
        await next();
    }

    private async Task AddAgencyClaims(ActionExecutingContext context)
    {
        if (context.Controller is not ControllerBase controller || !controller.User.IsAgencyStaff()) return;
        if (!controller.User.TryGetUserId(out Guid userId)) return;

        var agencyContext = await repository.GetAgencyContextForUser(userId);

        foreach (var identity in controller.User.Identities)
        {
            foreach (var claim in identity.Claims.Where(c => AgencyClaimTypes.Contains(c.Type)).ToList())
            {
                identity.TryRemoveClaim(claim);
            }
        }

        var claims = new List<Claim> { new(CovenantConstants.AgencyId, (agencyContext?.AgencyId ?? Guid.Empty).ToString()) };
        if (agencyContext is not null)
        {
            claims.Add(new Claim(CovenantConstants.AgencyIds, string.Join(",", agencyContext.AgencyIds)));
            claims.Add(new Claim(CovenantConstants.AgencyPersonnelId, agencyContext.AgencyPersonnelId.ToString()));
        }
        controller.User.AddIdentity(new ClaimsIdentity(claims));
    }
}
