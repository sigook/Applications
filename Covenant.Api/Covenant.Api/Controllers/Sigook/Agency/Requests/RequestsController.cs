using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Requests;

[Route(RouteName)]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
[ServiceFilter(typeof(AgencyPersonnelIdFilter))]
public class RequestsController(IRequestService requestService, IAgencyService agencyService) : ControllerBase
{
    public const string RouteName = "api/agency/requests";

    /// <summary>Gets the detail of a request by its identifier.</summary>
    /// <param name="repository">Request repository.</param>
    /// <param name="id">Identifier of the request.</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AgencyRequestDetailModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromServices] IRequestRepository repository, Guid id) =>
        this.GetByIdResult(await repository.GetRequestDetailForAgency(id));

    /// <summary>Creates a new request for the current agency.</summary>
    /// <param name="model">Request data.</param>
    [HttpPost]
    [ProducesResponseType(typeof(AgencyRequestDetailModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] RequestCreateModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await requestService.CreateRequest(model);
        if (result) return CreatedAtAction(nameof(GetById), new { id = result.Value }, new AgencyRequestDetailModel { Id = result.Value });
        return BadRequest(ModelState.AddErrors(result.Errors));
    }

    /// <summary>Updates an existing request.</summary>
    /// <param name="id">Identifier of the request to update.</param>
    /// <param name="model">Updated request data.</param>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(AgencyRequestDetailModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] RequestCreateModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await requestService.UpdateRequest(id, model);
        if (result) return CreatedAtAction(nameof(GetById), new { id }, new AgencyRequestDetailModel { Id = id });
        return BadRequest(ModelState.AddErrors(result.Errors));
    }

    /// <summary>Toggles the ASAP flag of a request.</summary>
    /// <param name="id">Identifier of the request.</param>
    [HttpPut("{id:guid}/IsAsap")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> IsAsap([FromRoute] Guid id)
    {
        var result = await requestService.UpdateIsAsap(id);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Bulk-updates the ASAP flag of multiple requests.</summary>
    /// <param name="requestsQuickUpdate">Requests and the ASAP value to apply.</param>
    [HttpPut("is-asap")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateIsAsapRequests([FromBody] RequestsQuickUpdate requestsQuickUpdate)
    {
        var result = await requestService.UpdateIsAsapRequests(requestsQuickUpdate);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Increases the requested workers quantity of a request by one.</summary>
    /// <param name="id">Identifier of the request.</param>
    [HttpPut("{id}/IncreaseWorkersQuantityByOne")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> IncreaseWorkersQuantityByOne([FromRoute] Guid id)
    {
        var result = await agencyService.IncreaseWorkersQuantityByOne(id);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Reduces the requested workers quantity of a request by one.</summary>
    /// <param name="id">Identifier of the request.</param>
    [HttpPut("{id}/ReduceWorkersQuantityByOne")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReduceWorkersQuantityByOne([FromRoute] Guid id)
    {
        var result = await requestService.ReduceWorkerQuantityByOne(id);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Toggles the punch card visibility status of a request in the mobile app.</summary>
    /// <param name="id">Identifier of the request.</param>
    [HttpPut("{id}/PunchCardVisibilityStatusInApp")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PunchCardVisibilityStatusInApp([FromRoute] Guid id)
    {
        var result = await requestService.PunchCardUpdateVisibilityStatusInApp(id);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Cancels a request.</summary>
    /// <param name="id">Identifier of the request to cancel.</param>
    /// <param name="model">Cancellation detail.</param>
    [HttpPut("{id}/Cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cancel([FromRoute] Guid id, [FromBody] RequestCancellationDetailModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await requestService.CancelRequest(id, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Cancels multiple requests in bulk.</summary>
    /// <param name="model">Bulk cancellation data.</param>
    [HttpPut("bulk-cancel")]
    [ProducesResponseType(typeof(BulkRequestCancellationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkCancel([FromBody] BulkRequestCancellation model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await requestService.BulkCancelRequests(model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok(result.Value);
    }

    /// <summary>Replaces the recruiters of multiple requests in bulk. An empty recruiter list unassigns every recruiter.</summary>
    /// <param name="model">Bulk recruiter update data.</param>
    [HttpPut("bulk-recruiters")]
    [Authorize(Policy = PolicyConfiguration.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkRecruiters([FromBody] BulkRequestRecruiters model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await requestService.BulkUpdateRecruiters(model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Reopens a previously closed request.</summary>
    /// <param name="id">Identifier of the request to open.</param>
    [HttpPut("{id:guid}/Open")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Open([FromRoute] Guid id)
    {
        var result = await requestService.OpenRequest(id, User.GetNickname());
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Gets the job boards where the specified request is published.</summary>
    /// <param name="id">Identifier of the request.</param>
    [HttpGet("{id:guid}/sources")]
    [ProducesResponseType(typeof(IEnumerable<RequestSourceDetailModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSources([FromRoute] Guid id)
    {
        var result = await requestService.GetRequestSources(id);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok(result.Value);
    }

    /// <summary>Replaces the full set of job boards where the specified request is published.</summary>
    /// <param name="id">Identifier of the request.</param>
    /// <param name="sources">Job boards to publish on. PublishedAt and ExternalUrl are optional per item.</param>
    [HttpPut("{id:guid}/sources")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetSources([FromRoute] Guid id, [FromBody] IEnumerable<CreateRequestSourceModel> sources)
    {
        var result = await requestService.SetRequestSources(id, sources);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Queues a job that sends invitations to potential workers for the specified request and notifies the recruitment team on Teams when it finishes.</summary>
    /// <param name="id">Identifier of the request.</param>
    [HttpPost("{id:guid}/SendInvitation")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendInvitation([FromRoute] Guid id)
    {
        var result = await requestService.SendInvitation(id);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }
}
