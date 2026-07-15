using Covenant.Api.Authorization;
using Covenant.Common.Constants;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.Documents.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Recruiting;

[Route(RouteName)]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Recruiting)]
[ServiceFilter(typeof(AgencyIdFilter))]
[ServiceFilter(typeof(AgencyPersonnelIdFilter))]
public class CompanyProfilesController(IMediator mediator, ICompanyRepository companyRepository) : ControllerBase
{
    public const string RouteName = "api/agency/recruiting/companyprofiles";

    /// <summary>Gets a paginated list of company profiles for the current agency.</summary>
    /// <param name="filter">Company filter and pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<CompanyProfileListModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] GetCompanyForAgencyFilter filter)
    {
        filter.SalesPersonnelId = null;
        return Ok(await companyRepository.GetCompaniesProfileForAgency(User.GetAgencyId(), filter));
    }

    /// <summary>Generates and downloads an Excel report of the current agency's company profiles.</summary>
    /// <param name="filter">Company filter parameters.</param>
    [HttpGet("File")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFile([FromQuery] GetCompanyForAgencyFilter filter)
    {
        filter.SalesPersonnelId = null;
        var data = companyRepository.GetAllCompaniesProfileForAgency(User.GetAgencyId(), filter).ToList();
        var file = await mediator.Send(new GenerateAgencyCompanyProfileReport(data));
        return File(file.Document.ToArray(), CovenantConstants.ExcelMime, file.DocumentName);
    }

    /// <summary>Generates and downloads a detailed Excel report of the current agency's company profiles.</summary>
    /// <param name="filter">Company filter parameters.</param>
    [HttpGet("FileWithDetails")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFileWithDetails([FromQuery] GetCompanyForAgencyFilter filter)
    {
        var data = await companyRepository.GetCompaniesWithDetailsForAgency(User.GetAgencyId(), filter);
        var file = await mediator.Send(new GenerateAgencyCompanyProfileWithDetailsReport(data));
        return File(file.Document.ToArray(), CovenantConstants.ExcelMime, file.DocumentName);
    }
}
