using Covenant.Api.Authorization;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
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
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Get([FromServices] ICompanyRepository repository) =>
            Ok(await repository.GetCompanyLocations(c => c.CompanyProfile.CompanyId == User.GetCompanyId()));
    }
}