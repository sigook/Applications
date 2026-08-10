using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Covenant.Api.Controllers.Sigook.Agency.Sales;

[Route("api/agency/sales/[controller]")]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Sales)]
[ServiceFilter(typeof(AgencyIdFilter))]
[ServiceFilter(typeof(AgencyPersonnelIdFilter))]
public class DealsController(ISalesService salesService) : ControllerBase
{
    /// <summary>Gets a paginated list of deals for the sales module. Sales users only see the deals they own.</summary>
    /// <param name="filter">Deal filter and pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<DealListModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] GetDealsFilter filter) =>
        Ok(await salesService.GetDeals(filter));

    /// <summary>Creates a deal for a company profile, owned by the current sales user.</summary>
    /// <param name="model">Deal data: company profile, title, date, value, type, status and optional document.</param>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post()
    {
        var result = await salesService.CreateDeal();
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok(result.Value);
    }

    /// <summary>Updates an existing deal. Sales users can only update the deals they own.</summary>
    /// <param name="id">Identifier of the deal.</param>
    /// <param name="model">Updated deal data: title, date, value, type, status and optional document.</param>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateDealModel model)
    {
        var result = await salesService.UpdateDeal(id, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Downloads the document attached to a deal. Sales users can only access their own deals.</summary>
    /// <param name="id">Identifier of the deal.</param>
    [HttpGet("{id}/document")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDocument([FromRoute] Guid id)
    {
        var result = await salesService.GetDealDocument(id);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        if (!new FileExtensionContentTypeProvider().TryGetContentType(result.Value.FileName, out var contentType))
            contentType = "application/octet-stream";
        return File(result.Value.Content, contentType);
    }

    /// <summary>Deletes a deal. Sales users can only delete the deals they own.</summary>
    /// <param name="id">Identifier of the deal.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await salesService.DeleteDeal(id);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }
}
