using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Constants;
using Covenant.Common.Models.Request;
using Covenant.Core.BL.Interfaces;
using Covenant.Documents.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Sales;

[Route("api/agency/sales/[controller]")]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Sales)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class RequestsController(IMediator mediator, ISalesService salesService) : ControllerBase
{
    /// <summary>Gets a paginated list of requests for the sales module. Sales users only see requests where they are the assigned sales representative.</summary>
    /// <param name="pagination">Request filter and pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(AgencyRequestsPagedResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(GetRequestForAgencyFilter pagination) =>
        Ok(await salesService.GetRequests(pagination));

    /// <summary>Gets the detail of a request. Sales users can only open requests where they are the assigned sales representative.</summary>
    /// <param name="id">Identifier of the request.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AgencyRequestDetailModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id) =>
        this.GetByIdResult(await salesService.GetRequestDetail(id));

    /// <summary>Creates a request. A sales user is always assigned as the sales representative of the request they create.</summary>
    /// <param name="model">Request data.</param>
    [HttpPost]
    [ProducesResponseType(typeof(AgencyRequestDetailModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] RequestCreateModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await salesService.CreateRequest(model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return CreatedAtAction(nameof(GetById), new { id = result.Value }, new AgencyRequestDetailModel { Id = result.Value });
    }

    /// <summary>Updates a request. Sales users can only update requests assigned to them, and cannot reassign them.</summary>
    /// <param name="id">Identifier of the request.</param>
    /// <param name="model">Updated request data.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AgencyRequestDetailModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] RequestCreateModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await salesService.UpdateRequest(id, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok(new AgencyRequestDetailModel { Id = id });
    }

    /// <summary>Generates and downloads an Excel report of the sales module requests.</summary>
    /// <param name="pagination">Request filter parameters.</param>
    [HttpGet("File")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFile(GetRequestForAgencyFilter pagination)
    {
        var data = salesService.GetRequestsForReport(pagination).ToList();
        var file = await mediator.Send(new GenerateAgencyRequestsReport(data));
        return File(file.Document.ToArray(), CovenantConstants.ExcelMime, file.DocumentName);
    }
}
