using Covenant.Api.Authorization;
using Covenant.Common.Models;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Company.Requests;

[Route(RouteName)]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Company)]
[ServiceFilter(typeof(CompanyIdFilter))]
public class ShiftController(IRequestService requestService) : ControllerBase
{
    public const string RouteName = "api/company/requests/{requestId}/Shift";

    /// <summary>Gets the shift of a request belonging to the current company.</summary>
    /// <param name="requestId">Request identifier.</param>
    [HttpGet]
    [ProducesResponseType(typeof(ShiftModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid requestId)
    {
        ShiftModel model = await requestService.GetRequestShift(requestId);
        if (model is null) return NotFound();
        return Ok(model);
    }
}
