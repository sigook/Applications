using Covenant.Api.Authorization;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using Covenant.Core.BL.Interfaces.Workers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Worker.Comments;
[ApiController]
[Authorize(Policy = PolicyConfiguration.Worker)]
[Route(RouteName)]
[Produces("application/json")]
public class CommentsController(IWorkerService workerService) : ControllerBase
{
    public const string RouteName = "api/worker/comments";
    /// <summary>Gets the paginated comments written about the current worker.</summary>
    /// <param name="pagination">Pagination criteria.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<WorkerCommentModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Pagination pagination) =>
        Ok(await workerService.GetMyComments(pagination ?? new Pagination()));
}
