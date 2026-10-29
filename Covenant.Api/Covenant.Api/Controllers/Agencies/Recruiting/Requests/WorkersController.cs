using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Agencies;
using Covenant.Common.Models.Requests;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Requests;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces.Agencies;
using Covenant.Core.BL.Interfaces.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Agencies.Recruiting.Requests;

[Route(RouteName)]
[ApiController]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class WorkersController(
    IAgencyService agencyService,
    IRequestService requestService,
    IValidator<FinishAssignmentModel> finishValidator) : ControllerBase
{
    public const string RouteName = "api/agency/recruiting/requests/{requestId}/workers";

    /// <summary>Gets the workers assigned to the specified request.</summary>
    /// <param name="repository">Request repository.</param>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="pagination">Worker request filter and pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<AgencyWorkerRequestModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult> Get([FromServices] IRequestRepository repository, [FromRoute] Guid requestId, [FromQuery] GetWorkersRequestFilter pagination) =>
        Ok(await repository.GetWorkersRequestByRequestId(requestId, pagination));

    /// <summary>Gets a worker request of the specified request by its identifier.</summary>
    /// <param name="repository">Request repository.</param>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="id">Identifier of the worker request.</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AgencyWorkerRequestModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetById([FromServices] IRequestRepository repository, Guid requestId, Guid id)
    {
        Guid agencyId = User.GetAgencyId();
        var model = await repository.GetWorkerRequestByAgencyId(agencyId, requestId, id);
        if (model is null) return NotFound();
        return Ok(model);
    }

    /// <summary>Updates the start-working date of a worker request.</summary>
    /// <param name="requestDetailsService">Request details service.</param>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="id">Identifier of the worker request.</param>
    /// <param name="model">Worker booking data containing the new start date.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Put([FromServices] IRequestDetailsService requestDetailsService, Guid requestId, Guid id, [FromBody] AgencyBookWorkerModel model)
    {
        if (!ModelState.IsValid || model?.StartWorking is null) return BadRequest();
        var result = await requestDetailsService.UpdateWorkerStartWorking(requestId, id, model.StartWorking.GetValueOrDefault());
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Books a worker into the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="workerProfileId">Identifier of the worker profile to book.</param>
    /// <param name="model">Worker booking data.</param>
    [HttpPost("{workerProfileId:guid}/Book")]
    [ProducesResponseType(typeof(AgencyWorkerRequestModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Post([FromRoute] Guid requestId, [FromRoute] Guid workerProfileId, [FromBody] AgencyBookWorkerModel model)
    {
        var result = await agencyService.BookWorker(requestId, workerProfileId, model);
        if (result) return Ok(new AgencyWorkerRequestModel { Id = result.Value });
        return BadRequest(ModelState.AddErrors(result.Errors));
    }

    /// <summary>Rejects a worker from the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="workerProfileId">Identifier of the worker profile to reject.</param>
    /// <param name="model">Rejection comments.</param>
    [HttpPut("{workerProfileId:guid}/Reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Reject([FromRoute] Guid requestId, [FromRoute] Guid workerProfileId, [FromBody] CommentsModel model)
    {
        var result = await requestService.RejectWorker(requestId, workerProfileId, model);
        if (result) return Ok();
        return BadRequest(ModelState.AddErrors(result.Errors));
    }

    /// <summary>Ends the assignment of a booked worker in the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="workerProfileId">Identifier of the worker profile whose assignment ends.</param>
    /// <param name="model">Last day worked, reason, comments and whether the worker is notified.</param>
    [HttpPut("{workerProfileId:guid}/Finish")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Finish([FromRoute] Guid requestId, [FromRoute] Guid workerProfileId, [FromBody] FinishAssignmentModel model)
    {
        var validation = await finishValidator.ValidateAsync(model);
        if (!validation.IsValid) return BadRequest(ModelState.AddErrors(validation.ToResultFailure().Errors));
        var result = await requestService.FinishWorker(requestId, workerProfileId, model);
        if (result) return Ok();
        return BadRequest(ModelState.AddErrors(result.Errors));
    }

    /// <summary>Ends the assignment of every booked worker in the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="model">Last day worked, reason, comments and whether the workers are notified.</param>
    [HttpPut("Finish")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> FinishAll([FromRoute] Guid requestId, [FromBody] FinishAssignmentModel model)
    {
        var validation = await finishValidator.ValidateAsync(model);
        if (!validation.IsValid) return BadRequest(ModelState.AddErrors(validation.ToResultFailure().Errors));
        var result = await requestService.FinishAllWorkers(requestId, model);
        if (result) return Ok();
        return BadRequest(ModelState.AddErrors(result.Errors));
    }
}
