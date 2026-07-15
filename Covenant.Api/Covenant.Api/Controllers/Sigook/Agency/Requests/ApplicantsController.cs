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

namespace Covenant.Api.Controllers.Sigook.Agency.Requests;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class ApplicantsController(IRequestRepository repository, ICandidateRepository candidateRepository) : Controller
{
    public const string RouteName = "api/agency/requests/{requestId}/Applicants";

    /// <summary>Adds an applicant (candidate or worker) to the specified request.</summary>
    /// <param name="workerRequestRepository">Worker request repository used to validate worker applicants.</param>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="model">Applicant data identifying a candidate or a worker profile.</param>
    [HttpPost]
    [ProducesResponseType(typeof(RequestApplicantDetailModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromServices] IWorkerRequestRepository workerRequestRepository, [FromRoute] Guid requestId, [FromBody] RequestApplicantModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        var entity = await repository.GetRequestApplicant(ra => ra.RequestId == requestId && ra.WorkerProfileId == model.WorkerProfileId && ra.CandidateId == model.CandidateId);
        if (entity != null) return BadRequest(ModelState.AddError("The candidate is already in the request as an applicant"));
        var createdBy = User.GetNickname();
        if (model.CandidateId.HasValue)
        {
            var request = await repository.GetRequest(r => r.Id == requestId);
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
            if (workerRequest != null && workerRequest.IsBooked) return BadRequest(ModelState.AddError("The worker is already in the request as a worker"));
            var result = RequestApplicant.CreateWithWorker(requestId, model.WorkerProfileId.Value, createdBy, model.Comments);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            entity = result.Value;
        }
        else return BadRequest();
        await repository.Create(entity);
        await repository.SaveChangesAsync();
        return Ok(new RequestApplicantDetailModel { Id = entity.Id, CreatedBy = entity.CreatedBy, CreatedAt = entity.CreatedAt });
    }

    /// <summary>Updates the comments of a request applicant.</summary>
    /// <param name="id">Identifier of the request applicant.</param>
    /// <param name="model">Updated comments.</param>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put(Guid id, [FromBody] CommentsModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        RequestApplicant entity = await repository.GetRequestApplicant(c => c.Id == id);
        Result result = entity.UpdateComments(model.Comments);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        await repository.Update(entity);
        await repository.SaveChangesAsync();
        return Ok();
    }

    /// <summary>Gets a paginated list of applicants for the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="filter">Applicant filter and pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<RequestApplicantDetailModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid requestId, GetRequestApplicantFilter filter) => Ok(await repository.GetRequestApplicants(requestId, filter));

    /// <summary>Searches for potential applicants for the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="searchTerm">Term used to filter potential applicants.</param>
    [HttpGet("Search")]
    [ProducesResponseType(typeof(List<ApplicantSearchResultModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(Guid requestId, [FromQuery] string searchTerm)
    {
        var agencyId = User.GetAgencyId();
        var results = await repository.SearchApplicants(agencyId, requestId, searchTerm);
        return Ok(results);
    }

    /// <summary>Removes an applicant from a request.</summary>
    /// <param name="id">Identifier of the request applicant to delete.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id)
    {
        RequestApplicant entity = await repository.GetRequestApplicant(c => c.Id == id);
        if (entity is null) return BadRequest();
        repository.Delete(entity);
        await repository.SaveChangesAsync();
        return Ok();
    }
}
