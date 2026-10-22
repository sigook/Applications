using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models.Request.Agency;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Core.BL.Interfaces.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Agency.Recruiting.Requests;

[ApiController]
[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class ShiftController(IRequestService requestService, IRequestDetailsService requestDetailsService) : Controller
{
    public const string RouteName = "api/agency/recruiting/requests/{requestId}/shift";

    /// <summary>Gets the shift of the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    [HttpGet]
    [ProducesResponseType(typeof(ShiftModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid requestId)
    {
        ShiftModel model = await requestService.GetRequestShift(requestId);
        if (model is null) return NotFound();
        return Ok(model);
    }

    /// <summary>Updates the shift of the specified request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="model">New shift data.</param>
    [HttpPut]
    [ProducesResponseType(typeof(AgencyRequestDetailModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid requestId, [FromBody] ShiftModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        var result = await requestDetailsService.UpdateShift(requestId, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok(result.Value);
    }
}
