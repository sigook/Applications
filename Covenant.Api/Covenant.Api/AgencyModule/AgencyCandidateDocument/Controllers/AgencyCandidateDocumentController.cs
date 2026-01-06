using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Candidate;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Candidate;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyCandidateDocument.Controllers
{
    [Route(RouteName)]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ApiController]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class AgencyCandidateDocumentController : Controller
    {
        public const string RouteName = "api/AgencyCandidate/{candidateId}/Document";
        private readonly ICandidateRepository candidateRepository;
        private readonly ICandidateService candidateService;
        private readonly IDocumentService documentService;

        public AgencyCandidateDocumentController(ICandidateRepository repository, ICandidateService candidateService, IDocumentService documentService)
        {
            candidateRepository = repository;
            this.candidateService = candidateService;
            this.documentService = documentService;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromRoute] Guid candidateId, [FromBody] CovenantFileModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await candidateService.CreateCandidateDocument(candidateId, model);
            if (!result)
            {
                return BadRequest(ModelState.AddErrors(result.Errors));
            }
            return Ok(result.Value);
        }

        [HttpGet]
        public async Task<ActionResult> GetDocuments(Guid candidateId, Pagination pagination) =>
            Ok(await candidateRepository.GetDocuments(candidateId, pagination));

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDocument(Guid candidateId, Guid id)
        {
            var entity = await candidateRepository.GetDocument(candidateId, id);
            candidateRepository.Delete(entity);
            await candidateRepository.SaveChangesAsync();
            await documentService.DeleteFile(entity.DocumentId);
            return Ok();
        }
    }
}