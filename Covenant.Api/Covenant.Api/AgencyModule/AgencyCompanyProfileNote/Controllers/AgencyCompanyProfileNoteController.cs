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

namespace Covenant.Api.AgencyModule.AgencyCompanyProfileNote.Controllers
{
    [Route(RouteName)]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class AgencyCompanyProfileNoteController : Controller
    {
        private readonly ICompanyRepository _repository;
        public const string RouteName = "api/AgencyCompanyProfile/{profileId}/Note";
        public AgencyCompanyProfileNoteController(ICompanyRepository repository) => _repository = repository;

        [HttpPost]
        public async Task<IActionResult> Post(Guid profileId, [FromBody] NoteModel model)
        {
            if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
            Result<CovenantNote> result = CovenantNote.Create(model.Note, model.Color, User.GetNickname());
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            var entity = new CompanyProfileNote(profileId, result.Value);
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

        [HttpGet]
        public async Task<IActionResult> Get(Guid profileId, Pagination pagination) => Ok(await _repository.GetNotes(profileId, pagination));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid profileId, Guid id)
        {
            var model = await _repository.GetNoteDetail(profileId, id);
            if (model is null) return NotFound();
            return Ok(model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid profileId, Guid id, [FromBody] NoteModel model)
        {
            if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
            Result<CovenantNote> result = CovenantNote.Create(model.Note, model.Color, User.GetNickname());
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            var entity = await _repository.GetNote(profileId, id);
            if (entity is null) return BadRequest();
            entity.Update(result.Value, User.GetNickname());
            _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid profileId, Guid id)
        {
            var entity = await _repository.GetNote(profileId, id);
            if (entity is null) return BadRequest();
            entity.Delete(User.GetNickname());
            _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return Ok();
        }
    }
}