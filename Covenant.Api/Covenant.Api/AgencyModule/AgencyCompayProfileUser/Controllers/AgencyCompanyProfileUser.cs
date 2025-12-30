using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace Covenant.Api.AgencyModule.AgencyCompayProfileUser.Controllers
{
    [ApiController]
    [Route("api/agency-company-profile-user")]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    public class AgencyCompanyProfileUser : ControllerBase
    {
        private readonly ICompanyService companyService;
        private readonly ICompanyRepository companyRepository;

        public AgencyCompanyProfileUser(ICompanyService companyService, ICompanyRepository companyRepository)
        {
            this.companyService = companyService;
            this.companyRepository = companyRepository;
        }

        [HttpGet("{companyId}")]
        public async Task<IActionResult> GetUsers([FromRoute] Guid companyId)
        {
            var users = await companyRepository.GetAllCompanyUsers(companyId);
            return Ok(users);
        }

        [HttpPost("{companyId}")]
        public async Task<IActionResult> CreateUser([FromRoute] Guid companyId, [FromBody] CompanyUserModel model)
        {
            var result = await companyService.CreateCompanyUser(model, companyId);
            if (result)
            {
                return Ok();
            }
            return BadRequest(ModelState.AddErrors(result.Errors));
        }

        [HttpDelete("{companyId}/users/{userId}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid companyId, [FromRoute] Guid userId)
        {
            var result = await companyService.DeleteCompanyUser(userId, companyId);
            if (result)
            {
                return Ok();
            }
            return BadRequest(ModelState.AddErrors(result.Errors));
        }
    }
}
