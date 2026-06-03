using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Constants;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Location;
using Covenant.Common.Models.Security;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Company.Models;
using Covenant.Core.BL.Interfaces;
using Covenant.Documents.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyCompanyProfile.Controllers;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
[ApiController]
public class V2AgencyCompanyProfileController : ControllerBase
{
    public const string RouteName = "api/v2/AgencyCompanyProfile";
    private readonly ICompanyRepository companyRepository;
    private readonly IRequestRepository requestRepository;
    private readonly IAgencyService agencyService;
    private readonly ICompanyService companyService;

    private IMediator mediator;

    public V2AgencyCompanyProfileController(
        ICompanyRepository companyRepository,
        IRequestRepository requestRepository,
        IAgencyService agencyService,
        ICompanyService companyService)
    {
        this.companyRepository = companyRepository;
        this.requestRepository = requestRepository;
        this.agencyService = agencyService;
        this.companyService = companyService;
    }

    protected IMediator Mediator => mediator ?? (mediator = HttpContext.RequestServices.GetService<IMediator>());

    /// <summary>Gets a paginated list of company profiles for the current agency.</summary>
    /// <param name="filter">Company filter and pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<CompanyProfileListModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCompanyProfiles([FromQuery] GetCompanyForAgencyFilter filter) =>
        Ok(await companyRepository.GetCompaniesProfileForAgency(User.GetAgencyId(), filter));

    /// <summary>Generates and downloads an Excel report of the current agency's company profiles.</summary>
    /// <param name="filter">Company filter parameters.</param>
    [HttpGet("File")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFileCompanyProfiles([FromQuery] GetCompanyForAgencyFilter filter)
    {
        var data = companyRepository.GetAllCompaniesProfileForAgency(User.GetAgencyId(), filter).ToList();
        var file = await Mediator.Send(new GenerateAgencyCompanyProfileReport(data));
        return File(file.Document.ToArray(), CovenantConstants.ExcelMime, file.DocumentName);
    }

    /// <summary>Generates and downloads a detailed Excel report of the current agency's company profiles.</summary>
    /// <param name="filter">Company filter parameters.</param>
    [HttpGet("FileWithDetails")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFileCompanyProfilesWithDetails([FromQuery] GetCompanyForAgencyFilter filter)
    {
        var data = await companyRepository.GetCompaniesWithDetailsForAgency(User.GetAgencyId(), filter);
        var file = await Mediator.Send(new GenerateAgencyCompanyProfileWithDetailsReport(data));
        return File(file.Document.ToArray(), CovenantConstants.ExcelMime, file.DocumentName);
    }

    /// <summary>Gets the detail of a company profile by its identifier.</summary>
    /// <param name="id">Identifier of the company profile.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CompanyProfileDetailModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCompanyProfileById([FromRoute] Guid id)
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
    public async Task<IActionResult> CreateCompany([FromBody] CompanyProfileDetailModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await agencyService.CreateCompany(model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return CreatedAtAction(nameof(GetCompanyProfileById), new { id = result.Value }, new CompanyProfileDetailModel { Id = result.Value, Email = model.Email });
    }

    /// <summary>Updates an existing company profile.</summary>
    /// <param name="id">Identifier of the company profile to update.</param>
    /// <param name="model">Updated company profile data.</param>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CompanyProfileDetailModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCompany([FromRoute] Guid id, [FromBody] CompanyProfileDetailModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await agencyService.UpdateCompany(id, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return CreatedAtAction(nameof(GetCompanyProfileById), new { id }, new CompanyProfileDetailModel { Id = id, Email = model.Email });
    }

    /// <summary>Updates the email address of a company profile.</summary>
    /// <param name="companyProfileId">Identifier of the company profile.</param>
    /// <param name="model">New email information.</param>
    [HttpPut("{companyProfileId:guid}/Email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Email([FromRoute] Guid companyProfileId, [FromBody] UpdateEmailModel model)
    {
        var result = await agencyService.UpdateEmailCompanyProfile(companyProfileId, model);
        if (result)
        {
            return Ok();
        }
        return BadRequest(ModelState.AddErrors(result.Errors));
    }

    /// <summary>Updates the vaccination requirement of a company profile.</summary>
    /// <param name="companyProfileId">Identifier of the company profile.</param>
    /// <param name="model">Vaccination requirement data.</param>
    [HttpPut("{companyProfileId}/VaccinationRequired")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VaccinationRequired([FromRoute] Guid companyProfileId, [FromBody] VaccinationRequiredModel model)
    {
        var profile = await companyRepository.GetCompanyProfile(cp => cp.Id == companyProfileId);
        if (profile is null) return BadRequest();
        Result result = profile.UpdateVaccinationInfo(model.Required, model.Comments);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        companyRepository.Update(profile);
        await companyRepository.SaveChangesAsync();
        return Ok();
    }

    /// <summary>Updates whether a company profile requires permission to see requests.</summary>
    /// <param name="companyProfileId">Identifier of the company profile.</param>
    /// <param name="settingsUpdateModel">Company profile settings update data.</param>
    [HttpPatch("{companyProfileId}/RequiresPermissionToSeeRequests")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRequiresPermissionToSeeRequests([FromRoute] Guid companyProfileId, [FromBody] CompanyProfileSettingsUpdateModel settingsUpdateModel)
    {
        var profile = await companyRepository.GetCompanyProfile(cp => cp.Id == companyProfileId);
        if (profile is not null)
        {
            profile.UpdatePermissionToSeeRequests(settingsUpdateModel.RequiresPermissionToSeeRequests);
            companyRepository.Update(profile);
            await companyRepository.SaveChangesAsync();
            return Ok();
        }
        return NotFound();
    }

    /// <summary>Updates the paid holidays setting of a company profile.</summary>
    /// <param name="companyProfileId">Identifier of the company profile.</param>
    /// <param name="settingsUpdateModel">Company profile settings update data.</param>
    [HttpPatch("{companyProfileId}/PaidHolidays")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePaidHolidays([FromRoute] Guid companyProfileId, [FromBody] CompanyProfileSettingsUpdateModel settingsUpdateModel)
    {
        var profile = await companyRepository.GetCompanyProfile(cp => cp.Id == companyProfileId);
        if (profile is not null)
        {
            profile.PaidHolidays = settingsUpdateModel.PaidHolidays;
            companyRepository.Update(profile);
            await companyRepository.SaveChangesAsync();
            return Ok();
        }
        return NotFound();
    }

    /// <summary>Updates the overtime threshold of a company profile.</summary>
    /// <param name="companyProfileId">Identifier of the company profile.</param>
    /// <param name="settingsUpdateModel">Company profile settings update data.</param>
    [HttpPatch("{companyProfileId}/Overtime")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOvertime([FromRoute] Guid companyProfileId, [FromBody] CompanyProfileSettingsUpdateModel settingsUpdateModel)
    {
        var profile = await companyRepository.GetCompanyProfile(cp => cp.Id == companyProfileId);
        if (profile is not null)
        {
            var result = profile.UpdateOvertimeStartsAfter(TimeSpan.FromHours(settingsUpdateModel.OvertimeStartsAfter));
            if (result)
            {
                companyRepository.Update(profile);
                await companyRepository.SaveChangesAsync();
                return Ok();
            }
            return BadRequest(ModelState.AddErrors(result.Errors));
        }
        return NotFound();
    }

    /// <summary>Gets the users of the company associated with a company profile.</summary>
    /// <param name="companyProfileId">Identifier of the company profile.</param>
    [HttpGet("{companyProfileId}/CompanyUsers")]
    [ProducesResponseType(typeof(IEnumerable<CompanyUserModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCompanyUsers([FromRoute] Guid companyProfileId)
    {
        var companyId = await companyRepository.GetCompanyId(companyProfileId);
        if (companyId == default)
        {
            return NotFound();
        }
        var companyUsers = await companyRepository.GetAllCompanyUsers(companyId);
        return Ok(companyUsers);
    }

    /// <summary>Gets the companies that have associated requests for the current agencies.</summary>
    [HttpGet("company-with-requests")]
    [ProducesResponseType(typeof(IEnumerable<CompanyProfileListModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCompanyWithRequest()
    {
        return Ok(await requestRepository.GetCompaniesWithRequests(User.GetAgencyIds()));
    }


    /// <summary>Bulk-imports company data for an agency from an uploaded file.</summary>
    /// <param name="agencyId">Identifier of the agency to import companies into.</param>
    /// <param name="file">Spreadsheet file containing the company data to import.</param>
    [HttpPost("bulk/{agencyId}")]
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
        else
        {
            return Ok();
        }

    }
}