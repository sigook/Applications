using Covenant.Common.Constants;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Covenant.Api.Authorization;

/// <summary>
/// Add the claim agencyPersonnelId to users that belong to a specific agency
/// </summary>
public class AgencyPersonnelIdFilter(IAgencyRepository repository) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await AddAgencyPersonnelId(context);
        await next();
    }

    private async Task AddAgencyPersonnelId(ActionExecutingContext context)
    {
        if (context.Controller is not ControllerBase controller || !controller.User.IsAgencyStaff()) return;

        if (!controller.User.TryGetUserId(out Guid userId)) return;

        Guid agencyPersonnelId = await repository.GetAgencyPersonnelIdForUser(userId);
        if (agencyPersonnelId == Guid.Empty) return;

        controller.User.AddIdentity(new ClaimsIdentity(
        [
            new Claim(CovenantConstants.AgencyPersonnelId, agencyPersonnelId.ToString())
        ]));
    }
}
