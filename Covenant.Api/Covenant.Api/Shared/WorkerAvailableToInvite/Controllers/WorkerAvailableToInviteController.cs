using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Worker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Shared.WorkerAvailableToInvite.Controllers
{
    [Route(RouteName)]
    [Authorize]
    public class WorkerAvailableToInviteController : Controller
    {
        public const string RouteName = "api/Worker/AvailableToInvite";

        [HttpGet]
        public async Task<IActionResult> AvailableToInvite([FromServices] IWorkerRepository repository, [FromQuery] AvailableToInvitePagination pagination) =>
            Ok(await repository.GetWorkersAvailableToInvite(pagination));
    }
}