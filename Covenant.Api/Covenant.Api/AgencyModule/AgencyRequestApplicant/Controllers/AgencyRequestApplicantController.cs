using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Entities.Request;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Candidate;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyRequestApplicant.Controllers
{
    [Route(RouteName)]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class AgencyRequestApplicantController : Controller
    {
        private readonly IRequestRepository _repository;
        private readonly ICandidateRepository candidateRepository;
        public const string RouteName = "api/AgencyRequest/{requestId}/Applicant";
        public AgencyRequestApplicantController(IRequestRepository repository, ICandidateRepository candidateRepository)
        {
            _repository = repository;
            this.candidateRepository = candidateRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromServices] IWorkerRequestRepository workerRequestRepository, [FromRoute] Guid requestId, [FromBody] RequestApplicantModel model)
        {
            if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
            var entity = await _repository.GetRequestApplicant(ra => ra.RequestId == requestId && ra.WorkerProfileId == model.WorkerProfileId && ra.CandidateId == model.CandidateId);
            if (entity != null) return BadRequest(ModelState.AddError("The candidate is already in the order as an applicant"));
            var createdBy = User.GetNickname();
            if (model.CandidateId.HasValue)
            {
                var request = await _repository.GetRequest(r => r.Id == requestId);
                var candidate = await candidateRepository.GetCandidate(c => c.Id == model.CandidateId.Value);
                if (!candidate.Skills.Any(s => s.Equals(request.JobTitle)))
                {
                    candidate.AddSkill(request.JobTitle);
                }
                var result = RequestApplicant.CreateWithCandidate(requestId, model.CandidateId.Value, createdBy, model.Comments);
                if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
                entity = result.Value;
            }
            else if (model.WorkerProfileId.HasValue)
            {
                var workerRequest = await workerRequestRepository.GetWorkerRequestByWorkerProfileId(model.WorkerProfileId.Value, requestId);
                if (workerRequest != null && workerRequest.IsBooked) return BadRequest(ModelState.AddError("The worker is already in the order as a worker"));
                var result = RequestApplicant.CreateWithWorker(requestId, model.WorkerProfileId.Value, createdBy, model.Comments);
                if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
                entity = result.Value;
            }
            else return BadRequest();
            await _repository.Create(entity);
            await _repository.SaveChangesAsync();
            return Ok(new RequestApplicantDetailModel { Id = entity.Id, CreatedBy = entity.CreatedBy, CreatedAt = entity.CreatedAt });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] CommentsModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            RequestApplicant entity = await _repository.GetRequestApplicant(c => c.Id == id);
            Result result = entity.UpdateComments(model.Comments);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            await _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid requestId, GetRequestApplicantFilter filter) => Ok(await _repository.GetRequestApplicants(requestId, filter));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            RequestApplicant entity = await _repository.GetRequestApplicant(c => c.Id == id);
            if (entity is null) return BadRequest();
            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
            return Ok();
        }
    }
}