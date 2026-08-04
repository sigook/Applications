using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Shared.WorkerComment.Controllers
{
    [Route("api/worker/{workerId}/comment")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class WorkerCommentController(IWorkerService workerService) : ControllerBase
    {
        /// <summary>Gets the paginated comments for a worker, scoped to the caller.</summary>
        /// <param name="workerId">Worker identifier.</param>
        /// <param name="pagination">Pagination criteria.</param>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<WorkerCommentModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Get(Guid workerId, Pagination pagination)
        {
            var result = await workerService.GetComments(workerId, pagination ?? new Pagination());
            if (!result) return Forbid();
            return Ok(result.Value);
        }
    }
}
