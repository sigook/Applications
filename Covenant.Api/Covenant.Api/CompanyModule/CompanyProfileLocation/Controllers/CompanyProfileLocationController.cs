using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.CompanyModule.CompanyProfileLocation.Controllers
{
    [Route("api/CompanyProfile/Location")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Policy = "Company")]
    [ServiceFilter(typeof(CompanyIdFilter))]
    public class CompanyProfileLocationController : ControllerBase
    {
        private readonly ICompanyService companyService;

        public CompanyProfileLocationController(ICompanyService companyService)
        {
            this.companyService = companyService;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Get([FromServices] ICompanyRepository repository) =>
            Ok(await repository.GetCompanyLocations(c => c.CompanyProfile.CompanyId == User.GetCompanyId()));

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CompanyProfileLocationDetailModel model)
        {
            if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
            var result = await companyService.CreateCompanyLocation(model);
            if (!result)
            {
                return BadRequest(ModelState.AddErrors(result.Errors));
            }
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] CompanyProfileLocationDetailModel model)
        {
            if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
            var result = await companyService.UpdateCompanyLocation(id, model);
            if (!result)
            {
                return BadRequest(ModelState.AddErrors(result.Errors));
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromServices] ICompanyRepository repository, [FromRoute] Guid id)
        {
            var location = await repository.GetLocation(id);
            repository.Delete(location);
            await repository.SaveChangesAsync();
            return Ok();
        }
    }
}