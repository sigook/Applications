using Covenant.Api.Authorization;
using Covenant.Api.Utils;
using Covenant.Api.Utils.Extensions;
using Covenant.Api.Validators.Location;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Company;

[Route(RouteName)]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Company)]
[ServiceFilter(typeof(CompanyIdFilter))]
public class CompanyController(
    ICompanyRepository companyRepository,
    IDefaultLogoProvider defaultLogoProvider,
    ICompanyService companyService) : ControllerBase
{
    public const string RouteName = "api/Company";

    /// <summary>Registers a new company profile created by the company itself.</summary>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CompanyRegisterByItselfModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var validator = new InlineValidator<CompanyRegisterByItselfModel> { ClassLevelCascadeMode = CascadeMode.Stop, RuleLevelCascadeMode = CascadeMode.Stop };
        validator.RuleFor(r => r.Name).NotEmpty().NotNull().Length(2, 60);
        validator.RuleFor(r => r.Email).NotEmpty().NotNull().EmailAddress();
        validator.RuleFor(r => r.Phone).PhoneNumber();
        validator.RuleFor(r => r.PhoneExt).PhoneExt();
        validator.RuleFor(r => r.Locations).NotNull().ListMustContainAtLeastOneElement();
        validator.RuleForEach(r => r.Locations).SetValidator(new LocationModelValidator());
        validator.RuleFor(r => r.Password).NotEmpty().NotNull().Length(6, 100);
        validator.RuleFor(r => r.ConfirmPassword).Equal(p => p.Password);
        var validationResult = await validator.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return BadRequest(ModelState);
        }

        if (string.IsNullOrEmpty(model.Logo?.FileName)) model.Logo = await defaultLogoProvider.GetLogo(model.Name);

        Result<Guid> result = await companyService.CreateCompanyProfile(model);
        if (result) return CreatedAtAction(nameof(GetById), new { profileId = result.Value }, new { });
        return BadRequest(ModelState.AddErrors(result.Errors));
    }

    /// <summary>Gets the profile detail of the current company.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(CompanyProfileDetailModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById()
    {
        var model = await companyRepository.GetCompanyProfileDetail(cp => cp.CompanyId == User.GetCompanyId());
        if (model is null) return NotFound();
        return Ok(model);
    }

    /// <summary>Updates an existing company profile.</summary>
    /// <param name="profileId">Company profile identifier.</param>
    /// <param name="model">Updated company profile data.</param>
    [HttpPut("{profileId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put(Guid profileId, [FromBody] CompanyProfileDetailModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await companyService.UpdateProfile(profileId, model);
        if (result) return Ok();
        return BadRequest(ModelState.AddErrors(result.Errors));
    }
}
