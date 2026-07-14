using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Agency;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Requests;

[Route(RouteName)]
[ApiController]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class WorkersController(IAgencyService agencyService, IRequestService requestService) : ControllerBase
{
    public const string RouteName = "api/agency/requests/{requestId}/Workers";

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
    /// <param name="repository">Worker request repository.</param>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="id">Identifier of the worker request.</param>
    /// <param name="model">Worker booking data containing the new start date.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Put([FromServices] IWorkerRequestRepository repository, Guid requestId, Guid id, [FromBody] AgencyBookWorkerModel model)
    {
        if (!ModelState.IsValid || model?.StartWorking is null) return BadRequest();
        var entity = await repository.GetWorkerRequest(id);
        if (entity is null || entity.RequestId != requestId) return BadRequest();
        Result result = entity.UpdateStartWorking(model.StartWorking.GetValueOrDefault());
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        await repository.UpdateWorkerRequest(entity);
        await repository.SaveChangesAsync();
        return Ok();
    }

    /// <summary>Books a worker into the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="workerId">Identifier of the worker to book.</param>
    /// <param name="model">Worker booking data.</param>
    [HttpPost("{workerId:guid}/Book")]
    [ProducesResponseType(typeof(AgencyWorkerRequestModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Post([FromRoute] Guid requestId, [FromRoute] Guid workerId, [FromBody] AgencyBookWorkerModel model)
    {
        var result = await agencyService.BookWorker(requestId, workerId, model);
        if (result) return Ok(new AgencyWorkerRequestModel { Id = result.Value });
        return BadRequest(ModelState.AddErrors(result.Errors));
    }

    /// <summary>Rejects a worker from the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="workerId">Identifier of the worker to reject.</param>
    /// <param name="model">Rejection comments.</param>
    [HttpPut("{workerId:guid}/Reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Reject([FromRoute] Guid requestId, [FromRoute] Guid workerId, [FromBody] CommentsModel model)
    {
        var result = await requestService.RejectWorker(requestId, workerId, model);
        if (result) return Ok();
        return BadRequest(ModelState.AddErrors(result.Errors));
    }
}
