using Covenant.Common.Constants;
using Covenant.Common.Enums;
using Covenant.Common.Models.Notification;
using Covenant.Common.Repositories.Notification;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Account;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserNotificationController(INotificationRepository notificationRepository) : ControllerBase
{
    /// <summary>Gets the notification preferences for the current user.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<UserNotificationListModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
        var targetAllow = new List<NotificationTarget>();
        if (User.IsInRole(CovenantConstants.Role.Worker))
            targetAllow.Add(NotificationTarget.Worker);
        if (User.IsAgencyStaff())
            targetAllow.Add(NotificationTarget.Agency);
        if (User.IsInRole(CovenantConstants.Role.Company))
            targetAllow.Add(NotificationTarget.Company);
        List<UserNotificationListModel> list = await notificationRepository.Get(User.GetUserId(), targetAllow);
        return Ok(list);
    }

    /// <summary>Updates the notification preferences for the current user.</summary>
    /// <param name="model">Updated notification preferences.</param>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromBody] UserNotificationUpdateModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        await notificationRepository.CreateUpdate(User.GetUserId(), model);
        await notificationRepository.SaveChangesAsync();
        return Ok();
    }
}
