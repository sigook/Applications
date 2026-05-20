using Covenant.Common.Entities;
using Covenant.Common.Entities.Notification;
using Covenant.Common.Models.Notification;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Shared.EmailPreferences.Controllers;

[ApiController]
[Route(RouteName)]
public class EmailPreferencesController : ControllerBase
{
    public const string RouteName = "api/EmailPreferences";

    /// <summary>Unsubscribes a user from a given notification type.</summary>
    /// <param name="userRepository">User repository service.</param>
    /// <param name="notificationRepository">Notification repository service.</param>
    /// <param name="model">Unsubscribe request data.</param>
    [AllowAnonymous]
    [HttpPost("Unsubscribe")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Unsubscribe(
        [FromServices] IUserRepository userRepository,
        [FromServices] INotificationRepository notificationRepository,
        [FromBody] UnsubscribeModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        User user = await userRepository.GetUserById(model.UserId);
        if (user is null) return BadRequest();

        NotificationType notificationType = NotificationType.GetAll.FirstOrDefault(c => c.Id.ToString() == model.TypeId);
        if (notificationType is null) return BadRequest();

        UserNotificationType entity = await notificationRepository.Get(model.UserId, notificationType.Id);
        if (entity is null)
        {
            entity = new UserNotificationType(user.Id, notificationType.Id);
            await notificationRepository.Create(entity);
        }
        else
        {
            entity.EmailNotification = false;
            entity.PushNotification = false;
            entity.SMSNotification = false;
            await notificationRepository.Update(entity);
        }

        await notificationRepository.SaveChangesAsync();
        return Ok();
    }
}