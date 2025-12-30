using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        /// List all request that worker can apply
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns>PaginatedList</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult> Get(Pagination pagination) =>
            Ok(await _requestRepository.GetRequestsForWorker(User.GetUserId(), pagination));

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById([FromRoute] Guid id) =>
            this.GetByIdResult(await _requestRepository.GetRequestDetailForWorker(User.GetUserId(), id));

        [HttpPost("{requestId:guid}/Apply")]
        public async Task<IActionResult> Apply([FromRoute] Guid requestId, [FromBody] WorkerRequestApplyModel model)
        {
            var result = await workerService.Apply(requestId, model);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            return Ok(result.Value);
        }

        [HttpPost("{workerId:guid}/{requestId:guid}/Apply")]
        [AllowAnonymous]
        public async Task<IActionResult> Apply([FromRoute] Guid workerId, [FromRoute] Guid requestId, [FromBody] WorkerRequestApplyModel model)
        {
            var result = await workerService.Apply(requestId, model, workerId);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            return Ok(result.Value);
        }
    }
}