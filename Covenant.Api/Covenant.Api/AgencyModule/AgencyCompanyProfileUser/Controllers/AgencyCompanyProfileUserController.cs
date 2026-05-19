using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyCompanyProfileUser.Controllers
{
    [ApiController]
    [Route("api/agency-company-profile-user")]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    public class AgencyCompanyProfileUserController : ControllerBase
    {
        private readonly ICompanyService companyService;
        private readonly ICompanyRepository companyRepository;

        public AgencyCompanyProfileUserController(ICompanyService companyService, ICompanyRepository companyRepository)
        {
            this.companyService = companyService;
            this.companyRepository = companyRepository;
        }

        /// <summary>Gets all users of a company profile.</summary>
        /// <param name="companyId">Identifier of the company profile.</param>
        [HttpGet("{companyId}")]
        [ProducesResponseType(typeof(IEnumerable<CompanyUserModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers([FromRoute] Guid companyId)
        {
            var users = await companyRepository.GetAllCompanyUsers(companyId);
            return Ok(users);
        }

        /// <summary>Creates a new user for a company profile.</summary>
        /// <param name="companyId">Identifier of the company profile.</param>
        /// <param name="model">User data.</param>
        [HttpPost("{companyId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromRoute] Guid companyId, [FromBody] CompanyUserModel model)
        {
            var result = await companyService.CreateCompanyUser(model, companyId);
            if (result)
            {
                return Ok();
            }
            return BadRequest(ModelState.AddErrors(result.Errors));
        }

        /// <summary>Deletes a user from a company profile.</summary>
        /// <param name="companyId">Identifier of the company profile.</param>
        /// <param name="userId">Identifier of the user to delete.</param>
        [HttpDelete("{companyId}/users/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
