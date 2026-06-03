using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        /// <summary>Assigns a recruiter to the specified request.</summary>
        /// <param name="agencyRepository">Agency repository used to resolve the recruiter personnel.</param>
        /// <param name="requestId">Identifier of the request.</param>
        /// <param name="model">Recruiter assignment data.</param>
        [HttpPost]
        [ProducesResponseType(typeof(RequestRecruiterDetailModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>Gets a paginated list of recruiters assigned to the specified request.</summary>
        /// <param name="requestId">Identifier of the request.</param>
        /// <param name="pagination">Pagination parameters.</param>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<RequestRecruiterDetailModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(Guid requestId, Pagination pagination) =>
            Ok(await _repository.GetRecruiters(requestId, pagination));

        /// <summary>Removes a recruiter from the specified request.</summary>
        /// <param name="requestId">Identifier of the request.</param>
        /// <param name="id">Identifier of the recruiter to remove.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute] Guid requestId, [FromRoute] Guid id)
        {
            var entity = await _repository.GetRequest(r => r.Id == requestId);
            if (entity is null) return BadRequest("Request not found");
            Result result = entity.RemoveRecruiter(id);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            await _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return Ok();
        }
    }
}