using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models.Companies;
using Covenant.Common.Models.Identity;
using Covenant.Common.Utils.Extensions;
using Covenant.Common.Repositories.Companies;
using Covenant.Core.BL.Interfaces.Companies;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Agencies.Recruiting.Clients;

[Route(RouteName)]
[ApiController]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class UsersController(
    ICompanyService companyService,
    ICompanyRepository companyRepository,
    IValidator<UpdateEmailModel> updateEmailValidator) : ControllerBase
{
    public const string RouteName = "api/agency/recruiting/clients/{profileId}/users";

    /// <summary>Gets the users of the company that owns the specified company profile.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CompanyUserModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromRoute] Guid profileId) =>
        Ok(await companyRepository.GetAllCompanyUsers(profileId));

    /// <summary>Creates a new user for the company that owns the specified company profile.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="model">User data.</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid profileId, [FromBody] CompanyUserModel model)
    {
        var result = await companyService.CreateCompanyUser(model, profileId);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Updates the details of a user of the company that owns the specified company profile.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="userId">Identifier of the user.</param>
    /// <param name="model">Updated user data.</param>
    [HttpPut("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid profileId, [FromRoute] Guid userId, [FromBody] CompanyUserModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await companyService.UpdateCompanyUser(userId, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Deletes a user from the company that owns the specified company profile.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="userId">Identifier of the user to delete.</param>
    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] Guid profileId, [FromRoute] Guid userId)
    {
        var result = await companyService.DeleteCompanyUser(userId, profileId);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Updates the login email of a user of the company that owns the specified company profile.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="userId">Identifier of the user.</param>
    /// <param name="model">New email information.</param>
    [HttpPut("{userId:guid}/Email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Email([FromRoute] Guid profileId, [FromRoute] Guid userId, [FromBody] UpdateEmailModel model)
    {
        var validation = await updateEmailValidator.ValidateAsync(model);
        if (!validation.IsValid) return BadRequest(ModelState.AddErrors(validation.ToResultFailure().Errors));

        var result = await companyService.UpdateCompanyUserEmail(userId, model.NewEmail);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }
}
