using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models.Companies.Agencies;
using Covenant.Common.Models.Companies;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Companies;
using Covenant.Core.BL.Interfaces.Agencies;
using Covenant.Core.BL.Interfaces.Companies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Agencies.Recruiting.Clients;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
[ApiController]
public class JobPositionsController(ICompanyRepository repository, IAgencyService agencyService) : ControllerBase
{
    public const string RouteName = "api/agency/recruiting/clients/{profileId}/jobpositions";

    /// <summary>Gets a paginated, sortable list of the active job positions of the specified company profile.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="filter">Role search, sorting and pagination.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<CompanyProfileJobPositionRateModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromRoute] Guid profileId, [FromQuery] GetJobPositionsFilter filter) =>
        Ok(await repository.GetJobPositions(profileId, filter));

    /// <summary>Gets every active job position of the specified company profile, for dropdowns.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    [HttpGet("Dropdown")]
    [ProducesResponseType(typeof(IEnumerable<CompanyProfileJobPositionRateModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDropdown([FromRoute] Guid profileId) =>
        Ok(await repository.GetJobPositions(profileId));

    /// <summary>Creates a new job position for the specified company profile.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="model">Job position and rate data.</param>
    [HttpPost]
    [ProducesResponseType(typeof(CompanyProfileJobPositionRateModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(Guid profileId, [FromBody] CompanyProfileJobPositionRateModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await agencyService.CreateCompanyJobPosition(profileId, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return CreatedAtAction(nameof(GetById), new { profileId, id = result.Value }, new CompanyProfileJobPositionRateModel { Id = result.Value });
    }

    /// <summary>Gets the detail of a job position by its identifier.</summary>
    /// <param name="id">Identifier of the job position.</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CompanyProfileJobPositionRateModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var model = await repository.GetJobPositionDetail(id);
        if (model is null) return NotFound();
        return Ok(model);
    }

    /// <summary>Updates a job position of the specified company profile.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="id">Identifier of the job position to update.</param>
    /// <param name="model">Updated job position and rate data.</param>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid profileId, [FromRoute] Guid id, [FromBody] CompanyProfileJobPositionRateModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await agencyService.UpdateCompanyJobPosition(id, profileId, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Soft-deletes a job position of the specified company profile.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="id">Identifier of the job position to delete.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] Guid profileId, [FromRoute] Guid id)
    {
        var result = await agencyService.DeleteCompanyJobPosition(id);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Creates a job position without rates; an agency admin sets the rates later.</summary>
    /// <param name="companyService">Company service used to create the job position.</param>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="model">Job position name, description and shift.</param>
    [HttpPost("WithoutRates")]
    [ProducesResponseType(typeof(CompanyProfileJobPositionRateModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostWithoutRates(
        [FromServices] ICompanyService companyService,
        [FromRoute] Guid profileId,
        [FromBody] CreateJobPositionModel model)
    {
        var result = await companyService.CreateJobPositionWithoutRates(profileId, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return CreatedAtAction(nameof(GetById), new { profileId, id = result.Value }, new CompanyProfileJobPositionRateModel { Id = result.Value });
    }
}
