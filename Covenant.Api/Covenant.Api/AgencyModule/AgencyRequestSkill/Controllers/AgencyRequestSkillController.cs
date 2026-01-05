using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Entities.Request;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyRequestSkill.Controllers
{
    [Route(RouteName)]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ApiController]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class AgencyRequestSkillController : Controller
    {
        public const string RouteName = "api/AgencyRequest/{requestId}/Skill";
        private readonly IRequestRepository _repository;

        public AgencyRequestSkillController(IRequestRepository repository) => _repository = repository;

        [HttpPost]
        public async Task<ActionResult> Post(Guid requestId, [FromBody] SkillModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(model?.Skill)) return BadRequest(ModelState);
            var result = RequestSkill.Create(requestId, model.Skill);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            await _repository.Create(result.Value);
            await _repository.SaveChangesAsync();
            model.Id = result.Value.Id;
            return Ok(model);
        }

        [HttpGet]
        public async Task<ActionResult> Get(Guid requestId) => Ok(await _repository.GetSkills(requestId));

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid requestId, Guid id)
        {
            var entity = await _repository.GetSkill(requestId, id);
            if (entity is null) return BadRequest();
            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
            return Ok();
        }
    }
}