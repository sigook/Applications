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

        [HttpPost]
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

        [HttpPut("{id:guid}")]
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

        [HttpGet]
        public async Task<IActionResult> Get() => 
            Ok(await companyRepository.GetAllCompanyUsers(User.GetCompanyId()));

        [HttpGet("detail")]
        public async Task<IActionResult> GetById()
        {
            var id = identityServerService.GetUserId();
            var model = await companyRepository.GetCompanyUserDetail(id);
            if (model is null) return NotFound();
            return Ok(model);
        }

        [HttpDelete("{id:guid}")]
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