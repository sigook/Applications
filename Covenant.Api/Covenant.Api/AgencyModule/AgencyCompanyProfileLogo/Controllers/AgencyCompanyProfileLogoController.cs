using Covenant.Api.Authorization;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyCompanyProfileLogo.Controllers
{
    [Route(RouteName)]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class AgencyCompanyProfileLogoController : Controller
    {
        public const string RouteName = "api/AgencyCompanyProfile/{profileId}/Logo";

        [HttpPut]
        public async Task<IActionResult> Put(
            [FromServices] ICompanyRepository repository,
            Guid profileId,
            [FromBody] CovenantFileModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var entity = await repository.GetCompanyProfile(cp => cp.Id == profileId);
            if (entity is null) return BadRequest();
            entity.UpdateLogo(model.FileName, model.Description);
            repository.Update(entity);
            await repository.SaveChangesAsync();
            return Ok();
        }
    }
}