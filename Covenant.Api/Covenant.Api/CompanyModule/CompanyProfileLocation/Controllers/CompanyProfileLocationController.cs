using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Location;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.CompanyModule.CompanyProfileLocation.Controllers
{
    [Route("api/CompanyProfile/Location")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Policy = PolicyConfiguration.Company)]
    [ServiceFilter(typeof(CompanyIdFilter))]
    public class CompanyProfileLocationController : ControllerBase
    {
        private readonly ICompanyService companyService;

        public CompanyProfileLocationController(ICompanyService companyService)
        {
            this.companyService = companyService;
        }

        /// <summary>Gets the locations of the current company profile.</summary>
        /// <param name="repository">Company repository resolved from DI.</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LocationDetailModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromServices] ICompanyRepository repository) =>
            Ok(await repository.GetCompanyLocations(c => c.CompanyProfile.CompanyId == User.GetCompanyId()));

        /// <summary>Creates a new location for the current company profile.</summary>
        /// <param name="model">Location data.</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>Updates an existing company profile location.</summary>
        /// <param name="id">Location identifier.</param>
        /// <param name="model">Updated location data.</param>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>Deletes a company profile location by its identifier.</summary>
        /// <param name="repository">Company repository resolved from DI.</param>
        /// <param name="id">Location identifier.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromServices] ICompanyRepository repository, [FromRoute] Guid id)
        {
            var location = await repository.GetLocation(id);
            repository.Delete(location);
            await repository.SaveChangesAsync();
            return Ok();
        }
    }
}
