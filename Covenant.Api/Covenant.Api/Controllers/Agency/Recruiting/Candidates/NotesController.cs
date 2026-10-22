using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Candidates;
using Covenant.Core.BL.Interfaces.Candidates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Agency.Recruiting.Candidates;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ApiController]
[ServiceFilter(typeof(AgencyIdFilter))]
public class NotesController(ICandidateRepository candidateRepository, ICandidateService candidateService) : Controller
{
    public const string RouteName = "api/agency/recruiting/candidates/{candidateId}/notes";

    /// <summary>Creates a new note for the specified candidate.</summary>
    /// <param name="candidateId">Identifier of the candidate the note belongs to.</param>
    /// <param name="model">Note content and color.</param>
    [HttpPost]
    [ProducesResponseType(typeof(NoteModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Post(Guid candidateId, [FromBody] NoteModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await candidateService.CreateNote(candidateId, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
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
        var result = await candidateService.UpdateNote(candidateId, id, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
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
        var result = await candidateService.DeleteNote(candidateId, id);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }
}
