using Covenant.Common.Repositories.Request;
using Covenant.Core.BL.Interfaces;
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

        [HttpPost("NotificationSinExpiration")]
        public async Task<IActionResult> NotificationSinExpiration()
        {
            await agencyService.NotifySinsExpired();
            return Ok();
        }

        [HttpPost("WarnLicensesExpiration")]
        public async Task<IActionResult> WarnLicensesExpiration()
        {
            await agencyService.NotifyLicensesExpired();
            return Ok();
        }

        [HttpPost("StartRequests")]
        public async Task<IActionResult> StartRequests([FromServices] IRequestRepository service)
        {
            await service.PutRequestInProgress();
            return Ok();
        }
    }
}