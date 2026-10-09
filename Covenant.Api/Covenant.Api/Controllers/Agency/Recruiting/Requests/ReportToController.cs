using Covenant.Api.Authorization;
using Covenant.Common.Entities.Request;
using Covenant.Common.Models.Request.Agency;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Agency.Recruiting.Requests;

[Route(RouteName)]
[ApiController]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class ReportToController(IRequestRepository repository) : ControllerBase
{
    public const string RouteName = "api/agency/recruiting/requests/{requestId}/reportto";

    /// <summary>Adds a report-to contact person to the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="contactPersonId">Identifier of the contact person.</param>
    [HttpPost("{contactPersonId}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(Guid requestId, Guid contactPersonId)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var entity = await repository.GetReportTo(requestId, contactPersonId);
        if (entity != null) return BadRequest();
        entity = new RequestReportTo(requestId, contactPersonId);
        await repository.Create<RequestReportTo>([entity]);
        await repository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { requestId, contactPersonId }, new { });
    }

    /// <summary>Gets the detail of a report-to contact person for the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="contactPersonId">Identifier of the contact person.</param>
    [HttpGet("{contactPersonId}")]
    [ProducesResponseType(typeof(RequestContactPersonDetailModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid requestId, Guid contactPersonId)
    {
        var model = await repository.GetReportToDetail(requestId, contactPersonId);
        if (model is null) return NotFound();
        return Ok(model);
    }

    /// <summary>Removes a report-to contact person from the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="contactPersonId">Identifier of the contact person to remove.</param>
    [HttpDelete("{contactPersonId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid requestId, Guid contactPersonId)
    {
        RequestReportTo entity = await repository.GetReportTo(requestId, contactPersonId);
        if (entity is null) return BadRequest();
        repository.Delete<RequestReportTo>([entity]);
        await repository.SaveChangesAsync();
        return Ok();
    }
}
