using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Constants;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Security;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Company.Models;
using Covenant.Core.BL.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.CompanyProfiles;

[Route(RouteName)]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
[ServiceFilter(typeof(AgencyPersonnelIdFilter))]
public class CompanyProfilesController(
    ICompanyRepository companyRepository,
    IRequestRepository requestRepository,
    IAgencyService agencyService,
    ICompanyService companyService,
    IValidator<UpdateEmailModel> updateEmailValidator) : ControllerBase
{
    public const string RouteName = "api/agency/companyprofiles";

    /// <summary>Gets the detail of a company profile by its identifier.</summary>
    /// <param name="id">Identifier of the company profile.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CompanyProfileDetailModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var model = await companyRepository.GetCompanyProfileDetail(cp => cp.Id == id);
        if (model is null) return NotFound();
        return Ok(model);
    }

    /// <summary>Creates a new company profile.</summary>
    /// <param name="model">Company profile data.</param>
    [HttpPost]
    [ProducesResponseType(typeof(CompanyProfileDetailModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CompanyProfileDetailModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await agencyService.CreateCompany(model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return CreatedAtAction(nameof(GetById), new { id = result.Value }, new CompanyProfileDetailModel { Id = result.Value, Email = model.Email });
    }

    /// <summary>Updates an existing company profile.</summary>
    /// <param name="id">Identifier of the company profile to update.</param>
    /// <param name="model">Updated company profile data.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CompanyProfileDetailModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] CompanyProfileDetailModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await agencyService.UpdateCompany(id, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return CreatedAtAction(nameof(GetById), new { id }, new CompanyProfileDetailModel { Id = id, Email = model.Email });
    }

    /// <summary>Updates the email address of a company profile.</summary>
    /// <param name="id">Identifier of the company profile.</param>
    /// <param name="model">New email information.</param>
    [HttpPut("{id:guid}/Email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Email([FromRoute] Guid id, [FromBody] UpdateEmailModel model)
    {
        var validation = await updateEmailValidator.ValidateAsync(model);
        if (!validation.IsValid) return BadRequest(ModelState.AddErrors(validation.ToResultFailure().Errors));

        var result = await agencyService.UpdateEmailCompanyProfile(id, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Updates the vaccination requirement of a company profile.</summary>
    /// <param name="id">Identifier of the company profile.</param>
    /// <param name="model">Vaccination requirement data.</param>
    [HttpPut("{id:guid}/VaccinationRequired")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VaccinationRequired([FromRoute] Guid id, [FromBody] VaccinationRequiredModel model)
    {
        var profile = await companyRepository.GetCompanyProfile(cp => cp.Id == id);
        if (profile is null) return BadRequest();
        Result result = profile.UpdateVaccinationInfo(model.Required, model.Comments);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        companyRepository.Update(profile);
        await companyRepository.SaveChangesAsync();
        return Ok();
    }

    /// <summary>Updates whether a company profile requires permission to see requests.</summary>
    /// <param name="id">Identifier of the company profile.</param>
    /// <param name="settingsUpdateModel">Company profile settings update data.</param>
    [HttpPatch("{id:guid}/RequiresPermissionToSeeRequests")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRequiresPermissionToSeeRequests([FromRoute] Guid id, [FromBody] CompanyProfileSettingsUpdateModel settingsUpdateModel)
    {
        var profile = await companyRepository.GetCompanyProfile(cp => cp.Id == id);
        if (profile is null) return NotFound();
        profile.UpdatePermissionToSeeRequests(settingsUpdateModel.RequiresPermissionToSeeRequests);
        companyRepository.Update(profile);
        await companyRepository.SaveChangesAsync();
        return Ok();
    }

    /// <summary>Updates the paid holidays setting of a company profile.</summary>
    /// <param name="id">Identifier of the company profile.</param>
    /// <param name="settingsUpdateModel">Company profile settings update data.</param>
    [HttpPatch("{id:guid}/PaidHolidays")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePaidHolidays([FromRoute] Guid id, [FromBody] CompanyProfileSettingsUpdateModel settingsUpdateModel)
    {
        var profile = await companyRepository.GetCompanyProfile(cp => cp.Id == id);
        if (profile is null) return NotFound();
        profile.PaidHolidays = settingsUpdateModel.PaidHolidays;
        companyRepository.Update(profile);
        await companyRepository.SaveChangesAsync();
        return Ok();
    }

    /// <summary>Updates the overtime threshold of a company profile.</summary>
    /// <param name="id">Identifier of the company profile.</param>
    /// <param name="settingsUpdateModel">Company profile settings update data.</param>
    [HttpPatch("{id:guid}/Overtime")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOvertime([FromRoute] Guid id, [FromBody] CompanyProfileSettingsUpdateModel settingsUpdateModel)
    {
        var profile = await companyRepository.GetCompanyProfile(cp => cp.Id == id);
        if (profile is null) return NotFound();
        var result = profile.UpdateOvertimeStartsAfter(TimeSpan.FromHours(settingsUpdateModel.OvertimeStartsAfter));
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        companyRepository.Update(profile);
        await companyRepository.SaveChangesAsync();
        return Ok();
    }

    /// <summary>Gets the companies that have associated requests for the current agencies.</summary>
    [HttpGet("company-with-requests")]
    [ProducesResponseType(typeof(IEnumerable<CompanyProfileListModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCompanyWithRequest() =>
        Ok(await requestRepository.GetCompaniesWithRequests(User.GetAgencyIds()));

    /// <summary>Gets the agency companies for a dropdown filtered by a search term.</summary>
    /// <param name="searchTerm">Term used to filter companies by name.</param>
    [HttpGet("companies-list")]
    [ProducesResponseType(typeof(List<BaseModel<Guid>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCompaniesList([FromQuery] string searchTerm) =>
        Ok(await companyRepository.GetCompaniesList(User.GetAgencyId(), searchTerm));

    /// <summary>Bulk-imports company data for an agency from an uploaded file.</summary>
    /// <param name="agencyId">Identifier of the agency to import companies into.</param>
    /// <param name="file">Spreadsheet file containing the company data to import.</param>
    [HttpPost("bulk/{agencyId:guid}")]
    [Consumes("multipart/form-data")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkCompanyData([FromRoute] Guid agencyId, IFormFile file)
    {
        var result = await companyService.BulkCompany(agencyId, file);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        var value = result.Value;
        if (value.Document.Length > 0)
        {
            return File(value.Document, CovenantConstants.ExcelMime, value.DocumentName);
        }
        return Ok();
    }
}
