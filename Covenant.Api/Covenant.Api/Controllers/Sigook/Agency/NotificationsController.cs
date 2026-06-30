using Covenant.Api.Authorization;
using Covenant.Common.Models.Notifications;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency;

[Route("api/agency/[controller]")]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class NotificationsController(INotificationService notificationService) : ControllerBase
{
    /// <summary>Gets all notifications for the current user in a single payload, grouped by type. Currently surfaces hired workers (non direct-hiring) within their first days so the recruiter can confirm attendance on the punch card.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(NotificationsModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get() =>
        Ok(await notificationService.GetNotifications());
}
