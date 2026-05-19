using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Constants;
using Covenant.Common.Models;
using Covenant.Common.Models.Candidate;
using Covenant.Common.Repositories.Candidate;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Covenant.Documents.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        /// <summary>Gets a paginated list of candidates for the current agency.</summary>
        /// <param name="filter">Candidate filter and pagination parameters.</param>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<CandidateListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCandidates([FromQuery] GetCandidatesFilter filter)
        {
            return Ok(await candidateRepository.GetCandidates(User.GetAgencyId(), filter));
        }

        /// <summary>Generates and downloads an Excel report of the current agency's candidates.</summary>
        /// <param name="filter">Candidate filter parameters.</param>
        [HttpGet("File")]
        [Produces("application/octet-stream")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCandidatesFile([FromQuery] GetCandidatesFilter filter)
        {
            var data = candidateRepository.GetAllCandidates(User.GetAgencyId(), filter).ToList();
            var file = await Mediator.Send(new GenerateAgencyCandidatesReport(data));
            return File(file.Document.ToArray(), CovenantConstants.ExcelMime, file.DocumentName);
        }

        /// <summary>Gets the detail of a candidate by its identifier.</summary>
        /// <param name="id">Identifier of the candidate.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CandidateDetailModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCandidateById([FromRoute] Guid id)
        {
            var candidate = await candidateRepository.GetCandidateDetail(id);
            if (candidate is null) return NotFound();
            return Ok(candidate);
        }

        /// <summary>Creates a new candidate for the current agency.</summary>
        /// <param name="model">Candidate data.</param>
        [HttpPost]
        [ProducesResponseType(typeof(CandidateDetailModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] CandidateCreateModel model)
        {
            var candidateId = await candidateService.CreateCandidate(model, User.GetAgencyId());
            if (!candidateId) return BadRequest(candidateId.Errors);
            return CreatedAtAction(nameof(GetCandidateById), new { id = candidateId.Value }, new CandidateDetailModel { Id = candidateId.Value });
        }

        /// <summary>Updates the basic information of a candidate.</summary>
        /// <param name="id">Identifier of the candidate to update.</param>
        /// <param name="model">Updated candidate data.</param>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCandidateBasicInformation([FromRoute] Guid id, [FromBody] CandidateCreateModel model)
        {
            var result = await candidateService.UpdateCandidate(id, model);
            if (!result) return BadRequest(result.Errors);
            return Ok();
        }

        /// <summary>Assigns the current user as the recruiter of a candidate.</summary>
        /// <param name="id">Identifier of the candidate.</param>
        [HttpPut("{id}/Recruiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCandidateRecruiter([FromRoute] Guid id)
        {
            var result = await candidateService.UpdateRecruiterCandidate(id);
            if (!result) return BadRequest();
            return Ok();
        }

        /// <summary>Deletes a candidate.</summary>
        /// <param name="id">Identifier of the candidate to delete.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await candidateService.DeleteCandidate(id);
            if (!result) return BadRequest();
            return Ok();
        }

        /// <summary>Converts a candidate into a worker.</summary>
        /// <param name="id">Identifier of the candidate to convert.</param>
        [HttpPost("{id}/convert-to-worker")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConvertToWorker([FromRoute] Guid id)
        {
            var result = await candidateService.ConvertToWorker(id);
            if (result)
            {
                return Ok();
            }
            return BadRequest(ModelState.AddErrors(result.Errors));
        }

        /// <summary>Bulk-imports candidates for an agency from an uploaded file.</summary>
        /// <param name="agencyId">Identifier of the agency to import candidates into.</param>
        /// <param name="file">Spreadsheet file containing the candidates to import.</param>
        [HttpPost("bulk/{agencyId}")]
        [Consumes("multipart/form-data")]
        [Produces("application/octet-stream")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BulkCandidates([FromRoute] Guid agencyId, IFormFile file)
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