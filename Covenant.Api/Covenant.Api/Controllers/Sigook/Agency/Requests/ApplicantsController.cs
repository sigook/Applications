using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Requests;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class ApplicantsController(IRequestApplicantService requestApplicantService, IUploadedFilesService uploadedFilesService) : Controller
{
    public const string RouteName = "api/agency/requests/{requestId}/Applicants";

    /// <summary>Adds an applicant (candidate or worker) to the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="model">Applicant data identifying a candidate or a worker profile.</param>
    [HttpPost]
    [ProducesResponseType(typeof(RequestApplicantDetailModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid requestId, [FromBody] RequestApplicantModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        var result = await requestApplicantService.Create(requestId, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok(result.Value);
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
        var result = await requestApplicantService.UpdateComments(id, model.Comments);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Gets a paginated list of applicants for the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="filter">Applicant filter and pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<RequestApplicantDetailModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid requestId, GetRequestApplicantFilter filter) => Ok(await requestApplicantService.GetApplicants(requestId, filter));

    /// <summary>Searches for potential applicants for the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="searchTerm">Term used to filter potential applicants.</param>
    [HttpGet("Search")]
    [ProducesResponseType(typeof(List<ApplicantSearchResultModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(Guid requestId, [FromQuery] string searchTerm) => Ok(await requestApplicantService.Search(requestId, searchTerm));

    /// <summary>Removes an applicant from a request.</summary>
    /// <param name="id">Identifier of the request applicant to delete.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await requestApplicantService.Delete(id);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Changes the status of a request applicant.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="id">Identifier of the request applicant.</param>
    /// <param name="model">Target status.</param>
    [HttpPut("{id}/Status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeStatus([FromRoute] Guid requestId, [FromRoute] Guid id, [FromBody] ChangeRequestApplicantStatusModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        var result = await requestApplicantService.ChangeStatus(requestId, id, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Gets the compliance checklist of the request with the completion state of the specified applicant.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="id">Identifier of the request applicant.</param>
    [HttpGet("{id}/ComplianceItems")]
    [ProducesResponseType(typeof(List<ApplicantComplianceItemModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetComplianceItems([FromRoute] Guid requestId, [FromRoute] Guid id)
    {
        var result = await requestApplicantService.GetComplianceItems(requestId, id);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok(result.Value);
    }

    /// <summary>Completes a compliance item for an applicant, optionally uploading a document to the worker profile.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="id">Identifier of the request applicant.</param>
    /// <param name="itemId">Identifier of the compliance item.</param>
    [HttpPost("{id}/ComplianceItems/{itemId}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CompleteComplianceItem([FromRoute] Guid requestId, [FromRoute] Guid id, [FromRoute] Guid itemId)
    {
        var model = uploadedFilesService.GetModel<CompleteApplicantComplianceItemModel>();
        var result = await requestApplicantService.CompleteComplianceItem(requestId, id, itemId, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Unchecks a compliance item for an applicant. Documents already uploaded stay on the worker profile.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="id">Identifier of the request applicant.</param>
    /// <param name="itemId">Identifier of the compliance item.</param>
    [HttpDelete("{id}/ComplianceItems/{itemId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UncompleteComplianceItem([FromRoute] Guid requestId, [FromRoute] Guid id, [FromRoute] Guid itemId)
    {
        var result = await requestApplicantService.UncompleteComplianceItem(requestId, id, itemId);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }
}
