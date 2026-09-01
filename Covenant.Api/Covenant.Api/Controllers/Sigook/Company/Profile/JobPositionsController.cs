using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Company.Profile;

[Route(RouteName)]
[ApiController]
[Authorize(Policy = PolicyConfiguration.Company)]
[ServiceFilter(typeof(CompanyIdFilter))]
public class JobPositionsController(ICompanyService companyService) : ControllerBase
{
    public const string RouteName = "api/company/profile/JobPositions";

    /// <summary>Gets the active job positions of the current company.</summary>
    /// <param name="companyRepository">Company repository resolved from DI.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CompanyProfileJobPositionRateModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromServices] ICompanyRepository companyRepository) =>
        Ok(await companyRepository.GetJobPositions(cpjpr => cpjpr.CompanyProfile.CompanyId == User.GetCompanyId() && !cpjpr.IsDeleted));

    /// <summary>Gets the detail of a specific job position by its identifier.</summary>
    /// <param name="companyRepository">Company repository resolved from DI.</param>
    /// <param name="id">Job position identifier.</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CompanyProfileJobPositionRateModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] ICompanyRepository companyRepository, [FromRoute] Guid id)
    {
        var model = await companyRepository.GetJobPositionDetail(id);
        if (model is null) return NotFound();
        return Ok(model);
    }

    /// <summary>Requests the creation of a new job position via a contact submission.</summary>
    /// <param name="contact">Contact details for the job position request.</param>
    [HttpPost("request-new-position")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RequestNewJobPosition([FromBody] ContactDto contact)
    {
        var result = await companyService.RequestNewJobPosition(contact);
        if (result is null) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }
}
