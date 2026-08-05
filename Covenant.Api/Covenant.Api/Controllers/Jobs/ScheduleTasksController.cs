using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Jobs;

[ApiController]
[Authorize]
[Route(RouteName)]
public class ScheduleTasksController(IAgencyService agencyService) : ControllerBase
{
    public const string RouteName = "api/ScheduleTasks";

    /// <summary>
    /// Triggers notifications for workers whose social insurance documents have expired.
    /// </summary>
    [HttpPost("NotificationSinExpiration")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> NotificationSinExpiration()
    {
        await agencyService.NotifySinsExpired();
        return Ok();
    }

    /// <summary>
    /// Triggers notifications for workers whose licenses are about to expire.
    /// </summary>
    [HttpPost("WarnLicensesExpiration")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> WarnLicensesExpiration()
    {
        await agencyService.NotifyLicensesExpired();
        return Ok();
    }
}
