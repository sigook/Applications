using Covenant.Common.Constants;
using Covenant.Common.Repositories.Agency;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Covenant.Api.Authorization;

/// <summary>
/// Add the claim agencyId to users that belong to a specific agency
/// </summary>
public class AgencyIdFilter : IAsyncActionFilter
{
    private readonly IAgencyRepository _repository;

    public AgencyIdFilter(IAgencyRepository repository) => _repository = repository;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await AddAgencyId(context);
        await next();
    }

    private async Task AddAgencyId(ActionExecutingContext context)
    {
        if (context.Controller is ControllerBase controller)
        {
            if (controller.User.IsInRole(CovenantConstants.Role.AgencyPersonnel))
            {
                string sub = controller.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(sub, out Guid userId))
                {
                    Guid agencyId = await _repository.GetAgencyIdForUser(userId);
                    List<Guid> agencyIds = await _repository.GetAgencyIdsForUser(userId);

                    var claims = new List<Claim>
                    {
                        new Claim(CovenantConstants.AgencyId, agencyId.ToString())
                    };

                    // Add all agency IDs as a comma-separated claim
                    if (agencyIds.Any())
                    {
                        string agencyIdsString = string.Join(",", agencyIds);
                        claims.Add(new Claim(CovenantConstants.AgencyIds, agencyIdsString));
                    }

                    controller.User.AddIdentity(new ClaimsIdentity(claims));
                }
            }
        }
    }
}