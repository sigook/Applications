using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models.Workers.Agencies;
using Covenant.Common.Models.Workers;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Workers;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces.Workers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Agencies.Recruiting.Workers;

[ApiController]
[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class NotesController : Controller
{
    public const string RouteName = "api/agency/recruiting/workers/{workerProfileId}/notes";

    /// <summary>Creates a new note for the specified worker profile.</summary>
    /// <param name="workerService">Worker service.</param>
    /// <param name="workerProfileId">Identifier of the worker profile.</param>
    /// <param name="model">Note content.</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromServices] IWorkerService workerService, [FromRoute] Guid workerProfileId, [FromBody] WorkerProfileNoteCreateModel model)
    {
        var result = await workerService.AddNote(workerProfileId, model?.Note);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Gets a paginated list of notes for the specified worker profile.</summary>
    /// <param name="repository">Worker repository.</param>
    /// <param name="workerProfileId">Identifier of the worker profile.</param>
    /// <param name="pagination">Pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<WorkerProfileNoteListModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromServices] IWorkerRepository repository, [FromRoute] Guid workerProfileId, Pagination pagination) =>
        Ok(await repository.GetWorkerProfileNotes(workerProfileId, pagination));
}
