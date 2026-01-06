using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Constants;
using Covenant.Common.Models.Candidate;
using Covenant.Common.Repositories.Candidate;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Covenant.Documents.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyCandidate.Controllers
{
    [Route(RouteName)]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ApiController]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class AgencyCandidateController : ControllerBase
    {
        public const string RouteName = "api/AgencyCandidate";
        private IMediator mediator;

        private readonly ICandidateRepository candidateRepository;
        private readonly ICandidateService candidateService;

        public AgencyCandidateController(ICandidateRepository candidateRepository, ICandidateService candidateService)
        {
            this.candidateRepository = candidateRepository;
            this.candidateService = candidateService;
        }

        protected IMediator Mediator => mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        [HttpGet]
        public async Task<IActionResult> GetCandidates([FromQuery] GetCandidatesFilter filter)
        {
            return Ok(await candidateRepository.GetCandidates(User.GetAgencyId(), filter));
        }

        [HttpGet("File")]
        public async Task<IActionResult> GetCandidatesFile([FromQuery] GetCandidatesFilter filter)
        {
            var data = candidateRepository.GetAllCandidates(User.GetAgencyId(), filter).ToList();
            var file = await Mediator.Send(new GenerateAgencyCandidatesReport(data));
            return File(file.Document.ToArray(), CovenantConstants.ExcelMime, file.DocumentName);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCandidateById([FromRoute] Guid id)
        {
            var candidate = await candidateRepository.GetCandidateDetail(id);
            if (candidate is null) return NotFound();
            return Ok(candidate);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CandidateCreateModel model)
        {
            var candidateId = await candidateService.CreateCandidate(model, User.GetAgencyId());
            if (!candidateId) return BadRequest(candidateId.Errors);
            return CreatedAtAction(nameof(GetCandidateById), new { id = candidateId.Value }, new CandidateDetailModel { Id = candidateId.Value });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCandidateBasicInformation([FromRoute] Guid id, [FromBody] CandidateCreateModel model)
        {
            var result = await candidateService.UpdateCandidate(id, model);
            if (!result) return BadRequest(result.Errors);
            return Ok();
        }

        [HttpPut("{id}/Recruiter")]
        public async Task<IActionResult> UpdateCandidateRecruiter([FromRoute] Guid id)
        {
            var result = await candidateService.UpdateRecruiterCandidate(id);
            if (!result) return BadRequest();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await candidateService.DeleteCandidate(id);
            if (!result) return BadRequest();
            return Ok();
        }

        [HttpPost("{id}/convert-to-worker")]
        public async Task<IActionResult> ConvertToWorker([FromRoute] Guid id)
        {
            var result = await candidateService.ConvertToWorker(id);
            return Ok();
        }

        [HttpPost("bulk/{agencyId}")]
        public async Task<IActionResult> BulkCandidates([FromRoute] Guid agencyId, [FromForm] IFormFile file)
        {
            var result = await candidateService.BulkCandidates(agencyId, file);
            if (result)
            {
                var value = result.Value;
                if (value.Document.Length > 0)
                {
                    return File(value.Document, CovenantConstants.ExcelMime, value.DocumentName);
                }
                else
                {
                    return Ok();
                }                
            }
            return BadRequest(ModelState.AddErrors(result.Errors));
        }
    }
}