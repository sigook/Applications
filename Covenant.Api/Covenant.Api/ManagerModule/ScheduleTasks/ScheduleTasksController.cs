using Covenant.Common.Repositories.Request;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.ManagerModule.ScheduleTasks
{
    [ApiController]
    [Route(RouteName)]
    public class ScheduleTasksController : ControllerBase
    {
        public const string RouteName = "api/ScheduleTasks";

        private readonly IAgencyService agencyService;

        public ScheduleTasksController(IAgencyService agencyService)
        {
            this.agencyService = agencyService;
        }

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
}