using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.CompanyProfiles;

[Route(RouteName)]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Sales)]
[ServiceFilter(typeof(AgencyIdFilter))]
[ServiceFilter(typeof(AgencyPersonnelIdFilter))]
public class InteractionsController(ISalesService salesService) : ControllerBase
{
    public const string RouteName = "api/agency/companyprofiles/{profileId}/Interactions";

    /// <summary>Gets a paginated list of the interactions of a company profile. Sales users only see the interactions they own.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="filter">Interaction filter and pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<CompanyInteractionListModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromRoute] Guid profileId, [FromQuery] GetCompanyInteractionsFilter filter) =>
        Ok(await salesService.GetInteractions(profileId, filter));

    /// <summary>Logs a new interaction for a company profile, owned by the current sales user.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="model">Interaction data: description, purpose, type and status.</param>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid profileId, [FromBody] CreateCompanyInteractionModel model)
    {
        var result = await salesService.CreateInteraction(profileId, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok(result.Value);
    }

    /// <summary>Updates an interaction of a company profile. Sales users can only update the interactions they own.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="id">Identifier of the interaction.</param>
    /// <param name="model">Updated description, purpose, type and status.</param>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid profileId, [FromRoute] Guid id, [FromBody] UpdateCompanyInteractionModel model)
    {
        var result = await salesService.UpdateInteraction(profileId, id, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Deletes an interaction of a company profile. Sales users can only delete the interactions they own.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="id">Identifier of the interaction.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] Guid profileId, [FromRoute] Guid id)
    {
        var result = await salesService.DeleteInteraction(profileId, id);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }
}
