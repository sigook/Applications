using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Covenant.Common.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.WorkerModule.WorkerRequest.Controllers
{
    [Route(RouteName)]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Policy = PolicyConfiguration.Worker)]
    public class WorkerRequestController : ControllerBase
    {
        public const string RouteName = "api/WorkerRequest";
        private readonly IRequestRepository _requestRepository;
        private readonly IWorkerService workerService;

        public WorkerRequestController(IRequestRepository requestRepository, IWorkerService workerService)
        {
            _requestRepository = requestRepository;
            this.workerService = workerService;
        }

        /// <summary>
        /// Lists all requests the authenticated worker can apply to.
        /// </summary>
        /// <param name="pagination">Pagination parameters.</param>
        /// <returns>PaginatedList</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<WorkerRequestListModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult> Get(Pagination pagination) =>
            Ok(await _requestRepository.GetRequestsForWorker(User.GetUserId(), pagination));

        /// <summary>
        /// Gets the detail of a specific request for the authenticated worker.
        /// </summary>
        /// <param name="id">Identifier of the request.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WorkerRequestDetailModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid id) =>
            this.GetByIdResult(await _requestRepository.GetRequestDetailForWorker(User.GetUserId(), id));

        /// <summary>
        /// Applies the authenticated worker to a request.
        /// </summary>
        /// <param name="requestId">Identifier of the request.</param>
        /// <param name="model">Application data.</param>
        [HttpPost("{requestId:guid}/Apply")]
        [ProducesResponseType(typeof(RequestApplicantDetailModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Apply([FromRoute] Guid requestId, [FromBody] WorkerRequestApplyModel model)
        {
            var result = await workerService.Apply(requestId, model);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            return Ok(result.Value);
        }

        /// <summary>
        /// Applies a specific worker to a request (anonymous).
        /// </summary>
        /// <param name="workerId">Identifier of the worker.</param>
        /// <param name="requestId">Identifier of the request.</param>
        /// <param name="model">Application data.</param>
        [HttpPost("{workerId:guid}/{requestId:guid}/Apply")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(RequestApplicantDetailModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Apply([FromRoute] Guid workerId, [FromRoute] Guid requestId, [FromBody] WorkerRequestApplyModel model)
        {
            var result = await workerService.Apply(requestId, model, workerId);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            return Ok(result.Value);
        }
    }
}