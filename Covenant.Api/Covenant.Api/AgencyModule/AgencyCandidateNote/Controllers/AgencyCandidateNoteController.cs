using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Candidate;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Candidate;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyCandidateNote.Controllers
{
    [Route(RouteName)]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ApiController]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class AgencyCandidateNoteController : Controller
    {
        public const string RouteName = "api/AgencyCandidate/{candidateId}/Note";
        private readonly ICandidateRepository candidateRepository;

        public AgencyCandidateNoteController(ICandidateRepository repository) => candidateRepository = repository;

        [HttpPost]
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

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(Guid candidateId, Guid id)
        {
            var note = await candidateRepository.GetNoteDetail(candidateId, id);
            if (note is null) return NotFound();
            return Ok(note);
        }

        [HttpGet]
        public async Task<ActionResult> Get(Guid candidateId, Pagination pagination) => Ok(await candidateRepository.GetNotes(candidateId, pagination));

        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
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
}