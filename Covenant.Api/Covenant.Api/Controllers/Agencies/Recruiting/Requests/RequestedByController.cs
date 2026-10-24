using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models.Requests.Agencies;
using Covenant.Common.Models.Requests;
using Covenant.Common.Repositories.Requests;
using Covenant.Core.BL.Interfaces.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Agencies.Recruiting.Requests;

[Route(RouteName)]
[ApiController]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class RequestedByController(IRequestRepository repository, IRequestDetailsService requestDetailsService) : ControllerBase
{
    public const string RouteName = "api/agency/recruiting/requests/{requestId}/requestedby";

    /// <summary>Adds a requested-by contact person to the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="contactPersonId">Identifier of the contact person.</param>
    [HttpPost("{contactPersonId}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(Guid requestId, Guid contactPersonId)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await requestDetailsService.AddRequestedBy(requestId, contactPersonId);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return CreatedAtAction(nameof(GetById), new { requestId, contactPersonId }, new { });
    }

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
        var result = await requestDetailsService.DeleteRequestedBy(requestId, contactPersonId);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }
}
