using Covenant.Api.Authorization;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Shared.RequestShift.Controllers
{
    [ApiController]
    [Route(RouteName)]
    [Authorize(Policy = PolicyConfiguration.Request)]
    public class RequestShiftController : ControllerBase
    {
        public const string RouteName = "api/Request/{requestId}/Shift";

        /// <summary>Gets the shift information for the given request.</summary>
        /// <param name="repository">Request repository service.</param>
        /// <param name="requestId">Request identifier.</param>
        [HttpGet]
        [ProducesResponseType(typeof(ShiftModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromServices] IRequestRepository repository, Guid requestId)
        {
            ShiftModel model = await repository.GetRequestShift(requestId);
            if (model is null) return NotFound();
            return Ok(model);
        }
    }
}