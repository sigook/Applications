using Covenant.Api.Authorization;
using Covenant.Common.Models;
using Covenant.Common.Models.Requests.Workers;
using Covenant.Common.Repositories.Requests;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Workers.History;

[Route(RouteName)]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Worker)]
public class HistoryController(IRequestRepository requestRepository) : ControllerBase
{
    public const string RouteName = "api/worker/history";

    private readonly IRequestRepository _requestRepository = requestRepository;

    /// <summary>
    /// Lists every assignment of the authenticated worker, newest first, with the worker-facing status Active, Completed or Ended.
    /// </summary>
    /// <param name="filter">Pagination and the optional worker-facing status (Active, Completed, Ended, or Past = Completed and Ended).</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<WorkerRequestListModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult> Get([FromQuery] GetWorkerHistoryFilter filter) =>
        Ok(await _requestRepository.GetRequestsHistoryForWorker(User.GetUserId(), filter));

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
