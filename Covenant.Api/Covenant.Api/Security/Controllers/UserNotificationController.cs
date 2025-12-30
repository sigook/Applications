using Covenant.Common.Constants;
using Covenant.Common.Enums;
using Covenant.Common.Models.Notification;
using Covenant.Common.Repositories.Notification;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Security.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserNotificationController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;
        public UserNotificationController(INotificationRepository notificationRepository) => _notificationRepository = notificationRepository;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var targetAllow = new List<NotificationTarget>();
            if (User.IsInRole(CovenantConstants.Role.Worker))
                targetAllow.Add(NotificationTarget.Worker);
            if (User.IsInRole(CovenantConstants.Role.AgencyPersonnel))
                targetAllow.Add(NotificationTarget.Agency);
            if (User.IsInRole(CovenantConstants.Role.Company))
                targetAllow.Add(NotificationTarget.Company);
            List<UserNotificationListModel> list = await _notificationRepository.Get(User.GetUserId(), targetAllow);
            return Ok(list);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UserNotificationUpdateModel model)
        {
            if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
            await _notificationRepository.CreateUpdate(User.GetUserId(), model);
            await _notificationRepository.SaveChangesAsync();
            return Ok();
        }
    }
}