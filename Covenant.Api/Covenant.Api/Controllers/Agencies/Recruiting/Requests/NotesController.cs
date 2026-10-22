using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Requests;
using Covenant.Core.BL.Interfaces.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Agency.Recruiting.Requests;

[ApiController]
[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class NotesController(IRequestRepository repository, IRequestDetailsService requestDetailsService) : Controller
{
    public const string RouteName = "api/agency/recruiting/requests/{requestId}/notes";

    /// <summary>Creates a new note for the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="model">Note content and color.</param>
    [HttpPost]
    [ProducesResponseType(typeof(NoteModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid requestId, [FromBody] NoteModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        var result = await requestDetailsService.CreateNote(requestId, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    /// <summary>Gets a paginated list of notes for the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="pagination">Pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<NoteModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromRoute] Guid requestId, Pagination pagination) => Ok(await repository.GetNotes(requestId, pagination));

    /// <summary>Gets the detail of a request note by its identifier.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="id">Identifier of the note.</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(NoteModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid requestId, [FromRoute] Guid id)
    {
        var model = await repository.GetNoteDetail(requestId, id);
        if (model is null) return NotFound();
        return Ok(model);
    }

    /// <summary>Updates an existing request note.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="id">Identifier of the note to update.</param>
    /// <param name="model">Updated note content and color.</param>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid requestId, [FromRoute] Guid id, [FromBody] NoteModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        var result = await requestDetailsService.UpdateNote(requestId, id, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Soft-deletes a request note.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="id">Identifier of the note to delete.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] Guid requestId, [FromRoute] Guid id)
    {
        var result = await requestDetailsService.DeleteNote(requestId, id);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }
}
