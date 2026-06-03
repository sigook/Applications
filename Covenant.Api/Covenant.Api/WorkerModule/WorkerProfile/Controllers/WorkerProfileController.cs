using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Utils.Extensions;
using Covenant.Common.Models;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.WorkerModule.WorkerProfile.Controllers
{
    [Route("api/WorkerProfile")]
    [Authorize(Policy = PolicyConfiguration.Worker)]
    [ApiController]
    [Produces("application/json")]
    public class WorkerProfileController : ControllerBase
    {
        private readonly IWorkerService workerService;

        public WorkerProfileController(IWorkerService workerService)
        {
            this.workerService = workerService;
        }

        /// <summary>
        /// Lists the worker profiles of the authenticated user across agencies.
        /// </summary>
        /// <param name="repository">Worker repository service.</param>
        [HttpGet]
        [ProducesResponseType(typeof(List<WorkerProfileAgencyListModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult> Get([FromServices] IWorkerRepository repository) => Ok(await repository.GetProfiles(User.GetUserId()));

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

        /// <summary>
        /// Gets the detail of a worker profile by its identifier.
        /// </summary>
        /// <param name="repository">Worker repository service.</param>
        /// <param name="profileId">Identifier of the worker profile.</param>
        [HttpGet("{profileId}")]
        [ProducesResponseType(typeof(WorkerProfileDetailModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetById([FromServices] IWorkerRepository repository, Guid profileId)
        {
            var model = await repository.GetWorkerProfileDetail(wp => wp.Id == profileId);
            if (model is null) return NotFound();
            return Ok(model);
        }

        /// <summary>
        /// Lists the other documents of a worker profile.
        /// </summary>
        /// <param name="repository">Worker repository service.</param>
        /// <param name="profileId">Identifier of the worker profile.</param>
        [HttpGet("{profileId:guid}/OtherDocument")]
        [ProducesResponseType(typeof(List<CovenantFileModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> OtherDocument([FromServices] IWorkerRepository repository, Guid profileId) =>
            Ok(await repository.GetOtherDocuments(profileId));
    }
}