using Covenant.Api.Authorization;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Models.Worker;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.WorkerModule.WorkerRequestHistory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Policy = PolicyConfiguration.Worker)]
    public class WorkerRequestHistoryController : ControllerBase
    {
        private readonly IRequestRepository _requestRepository;

        public WorkerRequestHistoryController(IRequestRepository requestRepository) =>
            _requestRepository = requestRepository;

        /// <summary>
        /// Lists the request history for the authenticated worker.
        /// </summary>
        /// <param name="pagination">Pagination parameters.</param>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<WorkerRequestListModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult> Get(Pagination pagination) =>
            Ok(await _requestRepository.GetRequestsHistoryForWorker(User.GetUserId(), pagination));

        /// <summary>
        /// Gets the detail of a historic request for the authenticated worker.
        /// </summary>
        /// <param name="id">Identifier of the request.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WorkerRequestDetailModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var model = await _requestRepository.GetRequestDetailForWorker(User.GetUserId(), id);
            if (model is null) return NotFound();
            return Ok(model);
        }
    }
}