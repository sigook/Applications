using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.CompanyProfiles;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class NotesController(ICompanyRepository repository) : Controller
{
    public const string RouteName = "api/agency/companyprofiles/{profileId}/Notes";

    /// <summary>Creates a new note for the specified company profile.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="model">Note content and color.</param>
    [HttpPost]
    [ProducesResponseType(typeof(NoteModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(Guid profileId, [FromBody] NoteModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        Result<CovenantNote> result = CovenantNote.Create(model.Note, model.Color, User.GetNickname());
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        var entity = new CompanyProfileNote(profileId, result.Value);
        await repository.Create(entity);
        await repository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { profileId, id = entity.NoteId }, new NoteModel
        {
            Id = entity.NoteId,
            CreatedBy = entity.Note.CreatedBy,
            CreatedAt = entity.Note.CreatedAt,
            Color = entity.Note.Color
        });
    }

    /// <summary>Gets a paginated list of notes for the specified company profile.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="pagination">Pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<NoteModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid profileId, Pagination pagination) => Ok(await repository.GetNotes(profileId, pagination));

    /// <summary>Gets the detail of a company profile note by its identifier.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="id">Identifier of the note.</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(NoteModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid profileId, Guid id)
    {
        var model = await repository.GetNoteDetail(profileId, id);
        if (model is null) return NotFound();
        return Ok(model);
    }

    /// <summary>Updates an existing company profile note.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="id">Identifier of the note to update.</param>
    /// <param name="model">Updated note content and color.</param>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put(Guid profileId, Guid id, [FromBody] NoteModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        Result<CovenantNote> result = CovenantNote.Create(model.Note, model.Color, User.GetNickname());
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        var entity = await repository.GetNote(profileId, id);
        if (entity is null) return BadRequest();
        entity.Update(result.Value, User.GetNickname());
        repository.Update(entity);
        await repository.SaveChangesAsync();
        return Ok();
    }

    /// <summary>Soft-deletes a company profile note.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="id">Identifier of the note to delete.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid profileId, Guid id)
    {
        var entity = await repository.GetNote(profileId, id);
        if (entity is null) return BadRequest();
        entity.Delete(User.GetNickname());
        repository.Update(entity);
        await repository.SaveChangesAsync();
        return Ok();
    }
}
