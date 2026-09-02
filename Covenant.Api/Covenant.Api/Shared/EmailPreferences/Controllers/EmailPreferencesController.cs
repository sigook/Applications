using Covenant.Common.Entities;
using Covenant.Common.Entities.Notification;
using Covenant.Common.Models.Notification;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Candidate;
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
    /// <param name="candidateRepository">Candidate repository service.</param>
    /// <param name="model">Unsubscribe request data.</param>
    [AllowAnonymous]
    [HttpPost("Unsubscribe")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Unsubscribe(
        [FromServices] IUserRepository userRepository,
        [FromServices] INotificationRepository notificationRepository,
        [FromServices] ICandidateRepository candidateRepository,
        [FromBody] UnsubscribeModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // Candidates are prospects: they have no User row and no notification types,
        // so their opt-out is a flag on the candidate itself.
        const string candidateUserType = "c";
        if (string.Equals(model.UserType, candidateUserType, StringComparison.OrdinalIgnoreCase))
        {
            var candidate = await candidateRepository.GetCandidate(c => c.Id == model.UserId);
            if (candidate is null) return BadRequest();
            candidate.UnsubscribeFromEmails();
            await candidateRepository.Update(candidate);
            await candidateRepository.SaveChangesAsync();
            return Ok();
        }

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