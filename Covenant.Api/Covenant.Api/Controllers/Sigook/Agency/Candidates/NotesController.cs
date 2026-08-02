using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Candidate;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Candidate;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Candidates;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ApiController]
[ServiceFilter(typeof(AgencyIdFilter))]
public class NotesController(ICandidateRepository candidateRepository) : Controller
{
    public const string RouteName = "api/agency/candidates/{candidateId}/Notes";

    /// <summary>Creates a new note for the specified candidate.</summary>
    /// <param name="candidateId">Identifier of the candidate the note belongs to.</param>
    /// <param name="model">Note content and color.</param>
    [HttpPost]
    [ProducesResponseType(typeof(NoteModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Post(Guid candidateId, [FromBody] NoteModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = CovenantNote.Create(model.Note, model.Color, User.GetNickname());
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        var entity = new CandidateNote(candidateId, result.Value);
        await candidateRepository.Create(entity);
        await candidateRepository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.NoteId }, new NoteModel
        {
            Id = entity.NoteId,
            CreatedBy = entity.Note.CreatedBy,
            CreatedAt = entity.Note.CreatedAt,
            Color = entity.Note.Color
        });
    }

    /// <summary>Gets the detail of a candidate note by its identifier.</summary>
    /// <param name="candidateId">Identifier of the candidate the note belongs to.</param>
    /// <param name="id">Identifier of the note.</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(NoteModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetById(Guid candidateId, Guid id)
    {
        var note = await candidateRepository.GetNoteDetail(candidateId, id);
        if (note is null) return NotFound();
        return Ok(note);
    }

    /// <summary>Gets a paginated list of notes for the specified candidate.</summary>
    /// <param name="candidateId">Identifier of the candidate.</param>
    /// <param name="pagination">Pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<NoteModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult> Get(Guid candidateId, Pagination pagination) => Ok(await candidateRepository.GetNotes(candidateId, pagination));

    /// <summary>Updates an existing candidate note.</summary>
    /// <param name="candidateId">Identifier of the candidate the note belongs to.</param>
    /// <param name="id">Identifier of the note to update.</param>
    /// <param name="model">Updated note content and color.</param>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put(Guid candidateId, Guid id, [FromBody] NoteModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        var result = CovenantNote.Create(model.Note, model.Color, User.GetNickname());
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        var entity = await candidateRepository.GetNote(candidateId, id);
        if (entity is null) return BadRequest();
        entity.Update(result.Value, User.GetNickname());
        await candidateRepository.Update(entity);
        await candidateRepository.SaveChangesAsync();
        return Ok();
    }

    /// <summary>Soft-deletes a candidate note.</summary>
    /// <param name="candidateId">Identifier of the candidate the note belongs to.</param>
    /// <param name="id">Identifier of the note to delete.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteNote(Guid candidateId, Guid id)
    {
        var entity = await candidateRepository.GetNote(candidateId, id);
        if (entity is null) return BadRequest();
        entity.Delete(User.GetNickname());
        await candidateRepository.Update(entity);
        await candidateRepository.SaveChangesAsync();
        return Ok();
    }
}
