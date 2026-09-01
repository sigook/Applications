using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Request;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Requests;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class ShiftController(IRequestRepository requestRepository, IShiftRepository shiftRepository, IRequestService requestService) : Controller
{
    public const string RouteName = "api/agency/requests/{requestId}/Shift";

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
        var entity = await requestRepository.GetRequest(r => r.Id == requestId);
        if (entity is null) return BadRequest();
        var newShift = model.ToShift();
        entity.OnNewShift += async (sender, e) => await shiftRepository.Create(newShift);
        Result result = entity.UpdateShift(newShift);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        await requestRepository.Update(entity);
        await requestRepository.SaveChangesAsync();
        return Ok(new AgencyRequestDetailModel { Id = entity.Id, DisplayShift = newShift.DisplayShift });
    }
}
