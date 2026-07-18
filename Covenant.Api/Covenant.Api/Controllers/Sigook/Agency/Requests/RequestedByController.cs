using Covenant.Api.Authorization;
using Covenant.Common.Entities.Request;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Requests;

[Route(RouteName)]
[ApiController]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class RequestedByController(IRequestRepository repository) : ControllerBase
{
    public const string RouteName = "api/agency/requests/{requestId}/RequestedBy";

    /// <summary>Adds a requested-by contact person to the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="contactPersonId">Identifier of the contact person.</param>
    [HttpPost("{contactPersonId}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(Guid requestId, Guid contactPersonId)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var entity = await repository.GetRequestedBy(requestId, contactPersonId);
        if (entity != null) return BadRequest();
        entity = new RequestRequestedBy(requestId, contactPersonId);
        await repository.Create(entity);
        await repository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { requestId, contactPersonId }, new { });
    }

    /// <summary>Gets a paginated list of requested-by contact persons for the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="pagination">Pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<RequestContactPersonModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid requestId, Pagination pagination) => Ok(await repository.GetRequestedByList(requestId, pagination));

    /// <summary>Gets the detail of a requested-by contact person for the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="contactPersonId">Identifier of the contact person.</param>
    [HttpGet("{contactPersonId}")]
    [ProducesResponseType(typeof(RequestContactPersonDetailModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid requestId, Guid contactPersonId)
    {
        var model = await repository.GetRequestedByDetail(requestId, contactPersonId);
        if (model is null) return NotFound();
        return Ok(model);
    }

    /// <summary>Removes a requested-by contact person from the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="contactPersonId">Identifier of the contact person to remove.</param>
    [HttpDelete("{contactPersonId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid requestId, Guid contactPersonId)
    {
        var entity = await repository.GetRequestedBy(requestId, contactPersonId);
        if (entity is null) return BadRequest();
        repository.Delete(entity);
        await repository.SaveChangesAsync();
        return Ok();
    }
}
