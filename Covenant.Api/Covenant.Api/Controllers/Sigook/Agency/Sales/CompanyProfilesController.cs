using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Constants;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
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
public class CompanyProfilesController(IMediator mediator, ISalesService salesService) : ControllerBase
{
    /// <summary>Gets a paginated list of company profiles for the sales module. Sales users only see the clients where they are the assigned sales representative.</summary>
    /// <param name="filter">Company filter and pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<CompanyProfileListModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] GetCompanyForAgencyFilter filter) =>
        Ok(await salesService.GetCompanies(filter));

    /// <summary>Gets the detail of a company profile. Sales users can only open the clients where they are the assigned sales representative.</summary>
    /// <param name="id">Identifier of the company profile.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CompanyProfileDetailModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id) =>
        this.GetByIdResult(await salesService.GetCompanyDetail(id));

    /// <summary>Creates a company profile. A sales user is always assigned as the sales representative of the client they create.</summary>
    /// <param name="model">Company profile data.</param>
    [HttpPost]
    [ProducesResponseType(typeof(CompanyProfileDetailModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CompanyProfileDetailModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await salesService.CreateCompany(model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return CreatedAtAction(nameof(GetById), new { id = result.Value }, new CompanyProfileDetailModel { Id = result.Value, Email = model.Email });
    }

    /// <summary>Updates a company profile. Sales users can only update the clients assigned to them, and cannot reassign them.</summary>
    /// <param name="id">Identifier of the company profile.</param>
    /// <param name="model">Updated company profile data.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CompanyProfileDetailModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] CompanyProfileDetailModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await salesService.UpdateCompany(id, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok(new CompanyProfileDetailModel { Id = id, Email = model.Email });
    }

    /// <summary>Generates and downloads an Excel report of the sales module company profiles.</summary>
    /// <param name="filter">Company filter parameters.</param>
    [HttpGet("File")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFile([FromQuery] GetCompanyForAgencyFilter filter)
    {
        var data = salesService.GetCompaniesForReport(filter).ToList();
        var file = await mediator.Send(new GenerateAgencyCompanyProfileReport(data));
        return File(file.Document.ToArray(), CovenantConstants.ExcelMime, file.DocumentName);
    }
}
