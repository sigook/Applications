using Covenant.Api.Authorization;
using Covenant.Common.Models.Notifications;
using Covenant.Core.BL.Interfaces.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Agencies;

[Route(RouteName)]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class NotificationsController(INotificationService notificationService) : ControllerBase
{
    public const string RouteName = "api/agency/notifications";

    /// <summary>Gets all notifications for the current user in a single payload, grouped by type. Currently surfaces the requests created by companies that are pending review by the agency.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(NotificationsModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get() =>
        Ok(await notificationService.GetNotifications());
}
