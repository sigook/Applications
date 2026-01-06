using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Candidate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyCandidateSkill.Controllers
{
    [Route(RouteName)]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ApiController]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class AgencyCandidateSkillController : Controller
    {
        public const string RouteName = "api/AgencyCandidate/{candidateId}/Skill";
        private readonly ICandidateRepository candidateRepository;

        public AgencyCandidateSkillController(ICandidateRepository repository) => candidateRepository = repository;

        [HttpPost]
        public async Task<ActionResult> Post(Guid candidateId, [FromBody] SkillModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (string.IsNullOrEmpty(model?.Skill)) return BadRequest();
            var candidate = await candidateRepository.GetCandidate(c => c.Id == candidateId);
            if (candidate is null) return BadRequest();
            var result = candidate.AddSkill(model.Skill);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            await candidateRepository.Update(candidate);
            await candidateRepository.SaveChangesAsync();
            model.Id = result.Value.Id;
            return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(Guid candidateId, Guid id)
        {
            var skill = await candidateRepository.GetSkillDetail(candidateId, id);
            if (skill == null) return NotFound();
            return Ok(skill);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid candidateId, Guid id)
        {
            var candidate = await candidateRepository.GetCandidate(c => c.Id == candidateId);
            if (candidate is null) return BadRequest();
            var result = candidate.DeleteSkill(id);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            await candidateRepository.Update(candidate);
            await candidateRepository.SaveChangesAsync();
            return Ok();
        }
    }
}