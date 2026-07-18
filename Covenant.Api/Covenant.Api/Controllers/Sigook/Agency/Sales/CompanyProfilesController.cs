using Covenant.Api.Authorization;
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
[ServiceFilter(typeof(AgencyPersonnelIdFilter))]
public class CompanyProfilesController(IMediator mediator, ISalesService salesService) : ControllerBase
{
    /// <summary>Gets a paginated list of company profiles for the sales module. Sales users only see the clients where they are the assigned sales representative.</summary>
    /// <param name="filter">Company filter and pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<CompanyProfileListModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] GetCompanyForAgencyFilter filter) =>
        Ok(await salesService.GetCompanies(filter));

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
