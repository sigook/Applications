using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Resources;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.CompanyProfiles;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
[ApiController]
public class JobPositionsController(ICompanyRepository repository, IAgencyService agencyService) : ControllerBase
{
    public const string RouteName = "api/agency/companyprofiles/{profileId}/JobPositions";

    /// <summary>Gets all active job positions of the specified company profile.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="filter">Optional filter that matches job positions by name.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CompanyProfileJobPositionRateModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromRoute] Guid profileId, [FromQuery] GetJobPositionsFilter filter) =>
        Ok(await repository.GetJobPositions(profileId, filter));

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
        var entity = await repository.GetJobPosition(id);
        if (entity is null) return BadRequest();
        entity.Delete(User.GetNickname());
        repository.Update(entity);
        await repository.SaveChangesAsync();
        return Ok();
    }

    /// <summary>Sends a petition email requesting the creation of a new job position.</summary>
    /// <param name="emailService">Email service used to send the petition.</param>
    /// <param name="configuration">Application configuration providing petition recipient emails.</param>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="model">Petition data including the requested job position.</param>
    [HttpPost("Petition")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        var profile = await repository.GetCompanyProfileDetail(cp => cp.Id == profileId);
        if (profile is null) return BadRequest(ModelState.AddError("Invalid company"));
        string nickname = User.GetNickname();
        var subject = $"Rol {model.JobPosition} requested by {nickname}";
        foreach (string email in emails)
        {
            await emailService.SendEmail(new EmailParams(email, subject,
                $"Client:{profile.NumberId} {profile.FullName} <br/> {model.Message} <br/> Please notify {nickname} after create the role")
            {
                EmailSettingName = EmailSettingName.Notification
            });
        }
        return Ok();
    }
}
