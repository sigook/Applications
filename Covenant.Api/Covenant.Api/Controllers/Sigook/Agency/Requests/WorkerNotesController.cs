using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Request;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Requests;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class WorkerNotesController(IWorkerRequestRepository repository) : Controller
{
    public const string RouteName = "api/agency/requests/{requestId}/Workers/{workerRequestId}/Notes";

    /// <summary>Creates a new note for the specified worker request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="workerRequestId">Identifier of the worker request.</param>
    /// <param name="model">Note content and color.</param>
    [HttpPost]
    [ProducesResponseType(typeof(NoteModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(Guid requestId, Guid workerRequestId, [FromBody] NoteModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        var result = CovenantNote.Create(model.Note, model.Color, User.GetNickname());
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        var entity = new WorkerRequestNote(workerRequestId, result.Value);
        await repository.CreateNote(entity);
        await repository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.NoteId }, new NoteModel
        {
            Id = entity.NoteId,
            CreatedBy = entity.Note.CreatedBy,
            CreatedAt = entity.Note.CreatedAt,
            Color = entity.Note.Color
        });
    }

    /// <summary>Gets a paginated list of notes for the specified worker request.</summary>
    /// <param name="workerRequestId">Identifier of the worker request.</param>
    /// <param name="pagination">Pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<NoteModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid workerRequestId, Pagination pagination) => Ok(await repository.GetNotes(workerRequestId, pagination));

    /// <summary>Gets the detail of a worker request note by its identifier.</summary>
    /// <param name="id">Identifier of the note.</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(NoteModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var model = await repository.GetNoteDetail(id);
        if (model is null) return NotFound();
        return Ok(model);
    }

    /// <summary>Updates an existing worker request note.</summary>
    /// <param name="id">Identifier of the note to update.</param>
    /// <param name="model">Updated note content and color.</param>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] NoteModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        var result = CovenantNote.Create(model.Note, model.Color, User.GetNickname());
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        var entity = await repository.GetNote(id);
        if (entity is null) return BadRequest();
        entity.Update(result.Value, User.GetNickname());
        await repository.Update(entity);
        await repository.SaveChangesAsync();
        return Ok();
    }

    /// <summary>Soft-deletes a worker request note.</summary>
    /// <param name="id">Identifier of the note to delete.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var entity = await repository.GetNote(id);
        if (entity is null) return BadRequest();
        entity.Delete(User.GetNickname());
        await repository.Update(entity);
        await repository.SaveChangesAsync();
        return Ok();
    }
}
