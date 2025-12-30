using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.CompanyModule.CompanyAgencyJobPosition.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = PolicyConfiguration.Company)]
    [ServiceFilter(typeof(CompanyIdFilter))]
    public class CompanyJobPositionController : ControllerBase
    {
        private readonly ICompanyService companyService;

        public CompanyJobPositionController(ICompanyService companyService)
        {
            this.companyService = companyService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromServices] ICompanyRepository companyRepository) =>
            Ok(await companyRepository.GetJobPositions(cpjpr => cpjpr.CompanyProfile.CompanyId == User.GetCompanyId() && !cpjpr.IsDeleted));

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromServices] ICompanyRepository companyRepository, [FromRoute] Guid id)
        {
            var model = await companyRepository.GetJobPositionDetail(id);
            if (model is null) return NotFound();
            return Ok(model);
        }

        [HttpPost("request-new-position")]
        public async Task<IActionResult> RequestNewJobPosition([FromBody] ContactDto contact)
        {
            var result = await companyService.RequestNewJobPosition(contact);
            if (result is null) return BadRequest(ModelState.AddErrors(result.Errors));
            return Ok();
        }
    }
}