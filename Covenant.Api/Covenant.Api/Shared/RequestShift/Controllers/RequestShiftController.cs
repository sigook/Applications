using Covenant.Api.Authorization;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Shared.RequestShift.Controllers
{
    [ApiController]
    [Route(RouteName)]
    [Authorize(Policy = PolicyConfiguration.Request)]
    public class RequestShiftController : ControllerBase
    {
        public const string RouteName = "api/Request/{requestId}/Shift";

        [HttpGet]
        public async Task<IActionResult> Get([FromServices] IRequestRepository repository, Guid requestId)
        {
            ShiftModel model = await repository.GetRequestShift(requestId);
            if (model is null) return NotFound();
            return Ok(model);
        }
    }
}