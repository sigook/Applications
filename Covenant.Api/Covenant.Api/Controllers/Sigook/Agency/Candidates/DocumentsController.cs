using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Candidate;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Candidates;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ApiController]
[ServiceFilter(typeof(AgencyIdFilter))]
public class DocumentsController(
    ICandidateRepository candidateRepository,
    ICandidateService candidateService,
    IDocumentService documentService) : Controller
{
    public const string RouteName = "api/agency/candidates/{candidateId}/Documents";

    /// <summary>Creates a new document for the specified candidate.</summary>
    /// <param name="candidateId">Identifier of the candidate.</param>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Post([FromRoute] Guid candidateId)
    {
        var result = await candidateService.CreateCandidateDocument(candidateId);
        if (!result)
        {
            return BadRequest(ModelState.AddErrors(result.Errors));
        }
        return Ok(result.Value);
    }

    /// <summary>Gets a paginated list of documents for the specified candidate.</summary>
    /// <param name="candidateId">Identifier of the candidate.</param>
    /// <param name="pagination">Pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<CovenantFileModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetDocuments(Guid candidateId, Pagination pagination) =>
        Ok(await candidateRepository.GetDocuments(candidateId, pagination));

    /// <summary>Deletes a candidate document.</summary>
    /// <param name="candidateId">Identifier of the candidate.</param>
    /// <param name="id">Identifier of the document to delete.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteDocument(Guid candidateId, Guid id)
    {
        var entity = await candidateRepository.GetDocument(candidateId, id);
        candidateRepository.Delete(entity);
        await candidateRepository.SaveChangesAsync();
        await documentService.DeleteFile(entity.DocumentId);
        return Ok();
    }
}
