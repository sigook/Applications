using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Request;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyRequestWorkerNote.Controllers
{
    [Route(RouteName)]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class AgencyRequestWorkerNoteController : Controller
    {
        private readonly IWorkerRequestRepository _repository;
        public const string RouteName = "api/AgencyRequest/{requestId}/Worker/{workerRequestId}/Note";
        public AgencyRequestWorkerNoteController(IWorkerRequestRepository repository) => _repository = repository;

        [HttpPost]
        public async Task<IActionResult> Post(Guid requestId, Guid workerRequestId, [FromBody] NoteModel model)
        {
            if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
            var result = CovenantNote.Create(model.Note, model.Color, User.GetNickname());
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            var entity = new WorkerRequestNote(workerRequestId, result.Value);
            await _repository.CreateNote(entity);
            await _repository.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = entity.NoteId }, new NoteModel
            {
                Id = entity.NoteId,
                CreatedBy = entity.Note.CreatedBy,
                CreatedAt = entity.Note.CreatedAt,
                Color = entity.Note.Color
            });
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid workerRequestId, Pagination pagination) => Ok(await _repository.GetNotes(workerRequestId, pagination));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var model = await _repository.GetNoteDetail(id);
            if (model is null) return NotFound();
            return Ok(model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] NoteModel model)
        {
            if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
            var result = CovenantNote.Create(model.Note, model.Color, User.GetNickname());
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            var entity = await _repository.GetNote(id);
            if (entity is null) return BadRequest();
            entity.Update(result.Value, User.GetNickname());
            await _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var entity = await _repository.GetNote(id);
            if (entity is null) return BadRequest();
            entity.Delete(User.GetNickname());
            await _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return Ok();
        }
    }
}