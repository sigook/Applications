using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Candidate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyCandidatePhoneNumber.Controllers
{
    [Route(RouteName)]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ApiController]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class AgencyCandidatePhoneNumberController : Controller
    {
        public const string RouteName = "api/AgencyCandidate/{candidateId}/PhoneNumber";
        private readonly ICandidateRepository candidateRepository;

        public AgencyCandidatePhoneNumberController(ICandidateRepository repository) => candidateRepository = repository;

        [HttpPost]
        public async Task<ActionResult> Post(Guid candidateId, [FromBody] PhoneNumberModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var entity = await candidateRepository.GetCandidate(c => c.Id == candidateId);
            if (entity is null) return BadRequest();
            var result = entity.AddPhone(model.PhoneNumber);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            await candidateRepository.Update(entity);
            await candidateRepository.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, new PhoneNumberModel { Id = result.Value.Id });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(Guid candidateId, Guid id)
        {
            var phoneNumber = await candidateRepository.GetPhoneNumberDetail(candidateId, id);
            if (phoneNumber == null) return NotFound();
            return Ok(phoneNumber);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid candidateId, Guid id)
        {
            var entity = await candidateRepository.GetCandidate(c => c.Id == candidateId);
            if (entity is null) return BadRequest();
            var result = entity.DeletePhone(id);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            await candidateRepository.Update(entity);
            await candidateRepository.SaveChangesAsync();
            return Ok();
        }
    }
}