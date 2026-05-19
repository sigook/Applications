using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Request;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyRequestNote.Controllers
{
    [Route(RouteName)]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class AgencyRequestNoteController : Controller
    {
        private readonly IRequestRepository _repository;
        public const string RouteName = "api/AgencyRequest/{requestId}/Note";

        public AgencyRequestNoteController(IRequestRepository repository) => _repository = repository;

        /// <summary>Creates a new note for the specified request.</summary>
        /// <param name="requestId">Identifier of the request.</param>
        /// <param name="model">Note content and color.</param>
        [HttpPost]
        [ProducesResponseType(typeof(NoteModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromRoute] Guid requestId, [FromBody] NoteModel model)
        {
            if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
            Result<CovenantNote> result = CovenantNote.Create(model.Note, model.Color, User.GetNickname());
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            var entity = new RequestNote(requestId, result.Value);
            await _repository.Create(entity);
            await _repository.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = entity.NoteId }, new NoteModel
            {
                Id = entity.NoteId,
                CreatedBy = entity.Note.CreatedBy,
                CreatedAt = entity.Note.CreatedAt,
                Color = entity.Note.Color
            });
        }

        /// <summary>Gets a paginated list of notes for the specified request.</summary>
        /// <param name="requestId">Identifier of the request.</param>
        /// <param name="pagination">Pagination parameters.</param>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<NoteModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] Guid requestId, Pagination pagination) => Ok(await _repository.GetNotes(requestId, pagination));

        /// <summary>Gets the detail of a request note by its identifier.</summary>
        /// <param name="requestId">Identifier of the request.</param>
        /// <param name="id">Identifier of the note.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(NoteModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid requestId, [FromRoute] Guid id)
        {
            var model = await _repository.GetNoteDetail(requestId, id);
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
            Result<CovenantNote> result = CovenantNote.Create(model.Note, model.Color, User.GetNickname());
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            RequestNote entity = await _repository.GetNote(requestId, id);
            if (entity is null) return BadRequest();
            entity.Update(result.Value, User.GetNickname());
            await _repository.Update(entity);
            await _repository.SaveChangesAsync();
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
            var entity = await _repository.GetNote(requestId, id);
            if (entity is null) return BadRequest();
            entity.Delete(User.GetNickname());
            await _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return Ok();
        }
    }
}