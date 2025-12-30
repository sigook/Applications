using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyRequestRecruiter.Controllers
{
    [Route(RouteName)]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class AgencyRequestRecruiterController : Controller
    {
        private readonly IRequestRepository _repository;
        public const string RouteName = "api/AgencyRequest/{requestId}/Recruiter";
        public AgencyRequestRecruiterController(IRequestRepository repository) => _repository = repository;

        [HttpPost]
        public async Task<IActionResult> Post([FromServices] IAgencyRepository agencyRepository, [FromRoute] Guid requestId, [FromBody] RequestRecruiterModel model)
        {
            if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
            var entity = await _repository.GetRequest(r => r.Id == requestId);
            if (entity is null) return BadRequest();
            var personnel = await agencyRepository.GetPersonnel(model.RecruiterId);
            if (personnel is null) return BadRequest();
            Result result = entity.AddRecruiter(personnel);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            await _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return Ok(new RequestRecruiterDetailModel { RecruiterId = personnel.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid requestId, Pagination pagination) =>
            Ok(await _repository.GetRecruiters(requestId, pagination));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid requestId, [FromRoute] Guid id)
        {
            var entity = await _repository.GetRequest(r => r.Id == requestId);
            if (entity is null) return BadRequest("Order not found");
            Result result = entity.RemoveRecruiter(id);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            await _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return Ok();
        }
    }
}