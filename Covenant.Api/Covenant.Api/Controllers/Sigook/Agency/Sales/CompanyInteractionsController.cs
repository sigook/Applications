using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Sales;

[Route("api/agency/sales/[controller]")]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Sales)]
[ServiceFilter(typeof(AgencyIdFilter))]
[ServiceFilter(typeof(AgencyPersonnelIdFilter))]
public class CompanyInteractionsController(ICompanyInteractionService interactionService) : ControllerBase
{
    /// <summary>Gets a paginated list of company interactions for the sales module. Sales users only see the interactions they own.</summary>
    /// <param name="filter">Interaction filter and pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<CompanyInteractionListModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] GetCompanyInteractionsFilter filter) =>
        Ok(await interactionService.GetInteractions(filter));

    /// <summary>Logs a new interaction for a company profile, owned by the current sales user.</summary>
    /// <param name="model">Interaction data: company profile, description, purpose, type and status.</param>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateCompanyInteractionModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        var result = await interactionService.Create(model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok(result.Value);
    }

    /// <summary>Updates an existing interaction. Sales users can only update the interactions they own.</summary>
    /// <param name="id">Identifier of the interaction.</param>
    /// <param name="model">Updated description, purpose, type and status.</param>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateCompanyInteractionModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        var result = await interactionService.Update(id, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Deletes an interaction. Sales users can only delete the interactions they own.</summary>
    /// <param name="id">Identifier of the interaction.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await interactionService.Delete(id);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }
}
