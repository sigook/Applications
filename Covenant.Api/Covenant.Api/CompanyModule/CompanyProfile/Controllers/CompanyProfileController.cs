using Covenant.Api.Authorization;
using Covenant.Api.Common.Validators;
using Covenant.Api.Utils;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.CompanyModule.CompanyProfile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Policy = PolicyConfiguration.Company)]
    [ServiceFilter(typeof(CompanyIdFilter))]
    public class CompanyProfileController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IDefaultLogoProvider _defaultLogoProvider;
        private readonly ICompanyService companyService;

        public CompanyProfileController(
            ICompanyRepository companyRepository,
            IDefaultLogoProvider defaultLogoProvider,
            ICompanyService companyService)
        {
            _companyRepository = companyRepository;
            _defaultLogoProvider = defaultLogoProvider;
            this.companyService = companyService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] CompanyRegisterByItselfModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var validator = new InlineValidator<CompanyRegisterByItselfModel> { ClassLevelCascadeMode = CascadeMode.Stop, RuleLevelCascadeMode = CascadeMode.Stop };
            validator.RuleFor(r => r.Name).NotEmpty().NotNull().Length(2, 60);
            validator.RuleFor(r => r.Email).NotEmpty().NotNull().EmailAddress();
            validator.RuleFor(r => r.Phone).PhoneNumber();
            validator.RuleFor(r => r.PhoneExt).PhoneExt();
            validator.RuleFor(r => r.Locations).NotNull().ListMustContainAtLeastOneElement();
            validator.RuleForEach(r => r.Locations).SetValidator(new LocationValidator());
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

            if (string.IsNullOrEmpty(model.Logo?.FileName)) model.Logo = await _defaultLogoProvider.GetLogo(model.Name);

            Result<Guid> result = await companyService.CreateCompanyProfile(model);
            if (result) return CreatedAtAction(nameof(GetById), new { profileId = result.Value }, new { });
            return BadRequest(ModelState.AddErrors(result.Errors));
        }

        [HttpGet]
        public async Task<IActionResult> GetById()
        {
            var model = await _companyRepository.GetCompanyProfileDetail(cp => cp.CompanyId == User.GetCompanyId());
            if (model is null) return NotFound();
            return Ok(model);
        }

        [HttpPut("{profileId}")]
        public async Task<IActionResult> Put(Guid profileId, [FromBody] CompanyProfileDetailModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await companyService.UpdateProfile(profileId, model);
            if (result) return Ok();
            return BadRequest(ModelState.AddErrors(result.Errors));
        }
    }
}