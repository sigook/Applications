using Covenant.Api.Authorization;
using Covenant.Common.Models.Location;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.CompanyModule.CompanyLocation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Policy = "Company")]
    [ServiceFilter(typeof(CompanyIdFilter))]
    public class CompanyLocationController : ControllerBase
    {
        /// <summary>Gets the locations belonging to the current company.</summary>
        /// <param name="repository">Company repository resolved from DI.</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LocationDetailModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromServices] ICompanyRepository repository) =>
            Ok(await repository.GetCompanyLocations(c => c.CompanyProfile.CompanyId == User.GetCompanyId()));
    }
}
