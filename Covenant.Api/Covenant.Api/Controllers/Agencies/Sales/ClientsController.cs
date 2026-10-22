using Covenant.Api.Authorization;
using Covenant.Common.Constants;
using Covenant.Common.Models.Companies.Agencies;
using Covenant.Common.Models.Companies;
using Covenant.Common.Models;
using Covenant.Core.BL.Interfaces.Sales;
using Covenant.Documents.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Agencies.Sales;

[Route(RouteName)]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Sales)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class ClientsController(IMediator mediator, ISalesService salesService) : ControllerBase
{
    public const string RouteName = "api/agency/sales/clients";

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
