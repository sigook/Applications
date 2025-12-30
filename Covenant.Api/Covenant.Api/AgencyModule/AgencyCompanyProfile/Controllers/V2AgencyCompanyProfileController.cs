using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Constants;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Security;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Company.Models;
using Covenant.Core.BL.Interfaces;
using Covenant.Documents.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

    [HttpGet]
    public async Task<IActionResult> GetCompanyProfiles([FromQuery] GetCompanyForAgencyFilter filter) =>
        Ok(await companyRepository.GetCompaniesProfileForAgency(User.GetAgencyId(), filter));

    [HttpGet("File")]
    public async Task<IActionResult> GetFileCompanyProfiles([FromQuery] GetCompanyForAgencyFilter filter)
    {
        var data = companyRepository.GetAllCompaniesProfileForAgency(User.GetAgencyId(), filter).ToList();
        var file = await Mediator.Send(new GenerateAgencyCompanyProfileReport(data));
        return File(file.Document.ToArray(), CovenantConstants.ExcelMime, file.DocumentName);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCompanyProfileById([FromRoute] Guid id)
    {
        var model = await companyRepository.GetCompanyProfileDetail(cp => cp.Id == id);
        if (model is null) return NotFound();
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromBody] CompanyProfileDetailModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await agencyService.CreateCompany(model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return CreatedAtAction(nameof(GetCompanyProfileById), new { id = result.Value }, new CompanyProfileDetailModel { Id = result.Value, Email = model.Email });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCompany([FromRoute] Guid id, [FromBody] CompanyProfileDetailModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await agencyService.UpdateCompany(id, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return CreatedAtAction(nameof(GetCompanyProfileById), new { id }, new CompanyProfileDetailModel { Id = id, Email = model.Email });
    }

    [HttpPut("{companyProfileId:guid}/Email")]
    public async Task<IActionResult> Email([FromRoute] Guid companyProfileId, [FromBody] UpdateEmailModel model)
    {
        var result = await agencyService.UpdateEmailCompanyProfile(companyProfileId, model);
        if (result)
        {
            return Ok();
        }
        return BadRequest(ModelState.AddErrors(result.Errors));
    }

    [HttpPut("{companyProfileId}/VaccinationRequired")]
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

    [HttpPatch("{companyProfileId}/RequiresPermissionToSeeOrders")]
    public async Task<IActionResult> UpdateRequiresPermissionToSeeOrders([FromRoute] Guid companyProfileId, [FromBody] CompanyProfileSettingsUpdateModel settingsUpdateModel)
    {
        var profile = await companyRepository.GetCompanyProfile(cp => cp.Id == companyProfileId);
        if (profile is not null)
        {
            profile.UpdatePermissionToSeeOrders(settingsUpdateModel.RequiresPermissionToSeeOrders);
            companyRepository.Update(profile);
            await companyRepository.SaveChangesAsync();
            return Ok();
        }
        return NotFound();
    }

    [HttpPatch("{companyProfileId}/PaidHolidays")]
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

    [HttpPatch("{companyProfileId}/Overtime")]
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

    [HttpGet("{companyProfileId}/CompanyUsers")]
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

    [HttpGet("company-with-requests")]
    public async Task<IActionResult> GetCompanyWithRequest()
    {
        return Ok(await requestRepository.GetCompaniesWithRequests(User.GetAgencyIds()));
    }

    [HttpGet("{profileId}/company-provinces-taxes")]
    public async Task<IActionResult> GetCompanyProvincesWithTaxes([FromRoute] Guid profileId)
    {
        var result = await companyRepository.GetCompanyProvincesWithTaxes(profileId);
        return Ok(result);
    }

    [HttpPost("bulk/{agencyId}")]
    public async Task<IActionResult> BulkCompanyData([FromRoute] Guid agencyId, [FromForm] IFormFile file)
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