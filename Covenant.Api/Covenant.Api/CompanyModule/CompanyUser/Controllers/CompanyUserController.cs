using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Security;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Resources;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.CompanyModule.CompanyUser.Controllers
{
    [Route(RouteName)]
    [ApiController]
    [Authorize(Policy = PolicyConfiguration.Company)]
    [ServiceFilter(typeof(CompanyIdFilter))]
    public class CompanyUserController : ControllerBase
    {
        public const string RouteName = "api/CompanyUser";
        private readonly ICompanyService companyService;
        private readonly ICompanyRepository companyRepository;
        private readonly IIdentityServerService identityServerService;

        public CompanyUserController(
            ICompanyService companyService, 
            ICompanyRepository companyRepository, 
            IIdentityServerService identityServerService)
        {
            this.companyService = companyService;
            this.companyRepository = companyRepository;
            this.identityServerService = identityServerService;
        }

        /// <summary>Creates a new user for the current company.</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] CompanyUserModel model)
        {
            if (model is not null || ModelState.IsValid)
            {
                var result = await companyService.CreateCompanyUser(model);
                if (result)
                {
                    return Ok();
                }
                return BadRequest(ModelState.AddErrors(result.Errors));
            }
            return BadRequest(ModelState);
        }

        /// <summary>Updates an existing company user.</summary>
        /// <param name="id">Company user identifier.</param>
        /// <param name="model">Updated company user data.</param>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] CompanyUserModel model)
        {
            if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
            var entity = await companyRepository.GetCompanyUser(id);
            if (entity is null) return BadRequest();
            entity.Name = model.Name;
            entity.Lastname = model.Lastname;
            entity.Position = model.Position;
            entity.MobileNumber = model.MobileNumber;
            companyRepository.Update(entity);
            await companyRepository.SaveChangesAsync();
            return Ok();
        }

        /// <summary>Gets all users belonging to the current company.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CompanyUserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get()
        {
            var profile = await companyRepository.GetCompanyProfileId(p => p.CompanyId == User.GetCompanyId());
            if (profile is null) return NotFound();
            return Ok(await companyRepository.GetAllCompanyUsers(profile.Id));
        }

        /// <summary>Gets the detail of the currently authenticated company user.</summary>
        [HttpGet("detail")]
        [ProducesResponseType(typeof(CompanyUserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById()
        {
            var id = identityServerService.GetUserId();
            var model = await companyRepository.GetCompanyUserDetail(id);
            if (model is null) return NotFound();
            return Ok(model);
        }

        /// <summary>Deletes a company user by its identifier.</summary>
        /// <param name="id">Company user identifier.</param>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await companyService.DeleteCompanyUser(id);
            if (result)
            {
                return Ok();
            }
            return BadRequest(ModelState.AddErrors(result.Errors));
        }
    }
}