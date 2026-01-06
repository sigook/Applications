using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Resources;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyCompanyProfileJobPosition.Controllers;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
[ApiController]
public class AgencyCompanyProfileJobPositionController : ControllerBase
{
    public const string RouteName = "api/AgencyCompanyProfile/{profileId}/JobPosition";
    private readonly ICompanyRepository _repository;
    private readonly IShiftRepository shiftRepository;
    private readonly IAgencyService agencyService;

    public AgencyCompanyProfileJobPositionController(ICompanyRepository companyRepository, IShiftRepository shiftRepository, IAgencyService agencyService)
    {
        _repository = companyRepository;
        this.shiftRepository = shiftRepository;
        this.agencyService = agencyService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromRoute] Guid profileId)
    {
        return Ok(await _repository.GetJobPositions(cpjpr => cpjpr.CompanyProfileId == profileId && !cpjpr.IsDeleted));
    }

    [HttpPost]
    public async Task<IActionResult> Post(Guid profileId, [FromBody] CompanyProfileJobPositionRateModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await agencyService.CreateCompanyJobPosition(profileId, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return CreatedAtAction(nameof(GetById), new { id = result.Value }, new CompanyProfileJobPositionRateModel { Id = result.Value });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var model = await _repository.GetJobPositionDetail(id);
        if (model is null) return NotFound();
        return Ok(model);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put([FromRoute] Guid profileId, [FromRoute] Guid id, [FromBody] CompanyProfileJobPositionRateModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await agencyService.UpdateCompanyJobPosition(id, profileId, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid profileId, [FromRoute] Guid id)
    {
        var entity = await _repository.GetJobPosition(id);
        if (entity is null) return BadRequest();
        entity.Delete(User.GetNickname());
        _repository.Update(entity);
        await _repository.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("Petition")]
    public async Task<IActionResult> Petition(
        [FromServices] IEmailService emailService,
        [FromServices] IConfiguration configuration,
        [FromRoute] Guid profileId,
        [FromBody] JobPositionPetitionModel model)
    {
        var emailsString = configuration.GetValue<string>("EmailsPetitionNewJobPosition");
        if (string.IsNullOrEmpty(emailsString)) return BadRequest(ModelState.AddError("Petition not available"));
        string[] emails = emailsString.Split(",");
        if (string.IsNullOrEmpty(model?.JobPosition))
            return BadRequest(ModelState.AddError(ValidationMessages.RequiredMsg(ApiResources.JobPosition)));
        var profile = await _repository.GetCompanyProfileDetail(cp => cp.Id == profileId);
        if (profile is null) return BadRequest(ModelState.AddError("Invalid company"));
        string nickname = User.GetNickname();
        var subject = $"Rol {model.JobPosition} requested by {nickname}";
        foreach (string email in emails)
        {
            await emailService.SendEmail(new EmailParams(email, subject,
                $"Client:{profile.NumberId} {profile.BusinessName} <br/> {model.Message} <br/> Please notify {nickname} after create the role")
            {
                EmailSettingName = EmailSettingName.Notification
            });
        }
        return Ok();
    }
}