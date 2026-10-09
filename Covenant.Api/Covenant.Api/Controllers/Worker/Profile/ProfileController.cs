using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Workers;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces.Workers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Worker.Profile;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Worker)]
[ApiController]
[Produces("application/json")]
public class ProfileController : ControllerBase
{
    public const string RouteName = "api/worker/profile";

    private readonly IWorkerService workerService;

    public ProfileController(IWorkerService workerService)
    {
        this.workerService = workerService;
    }

    /// <summary>
    /// Registers a new worker profile from a multipart/form-data payload.
    /// </summary>
    /// <param name="requestId">Optional request identifier the worker is registering against.</param>
    [HttpPost]
    [AllowAnonymous]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Post([FromQuery] int? requestId)
    {
        var result = await workerService.CreateWorker(requestId);
        if (result)
        {
            return Ok(result.Value);
        }
        return BadRequest(ModelState.AddErrors(result.Errors));
    }

    /// <summary>
    /// Gets the detail of the authenticated worker's own profile.
    /// </summary>
    /// <param name="repository">Worker repository service.</param>
    [HttpGet("me")]
    [ProducesResponseType(typeof(WorkerProfileDetailModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetMyProfile([FromServices] IWorkerRepository repository)
    {
        var model = await repository.GetWorkerProfileDetail(wp => wp.WorkerId == User.GetUserId());
        if (model is null) return NotFound();
        return Ok(model);
    }
}
