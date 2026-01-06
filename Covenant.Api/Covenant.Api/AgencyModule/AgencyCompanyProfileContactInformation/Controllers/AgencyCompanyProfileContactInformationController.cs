using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Entities;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyCompanyProfileContactInformation.Controllers
{
    [Route(RouteName)]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class AgencyCompanyProfileContactInformationController : Controller
    {
        public const string RouteName = "api/AgencyCompanyProfile/{profileId}/ContactInformation";

        [HttpPut]
        public async Task<IActionResult> Put(
            [FromServices] ICompanyRepository repository,
            Guid profileId,
            [FromBody] CompanyProfileDetailModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            Result<CvnWebPage> rWebPage = CvnWebPage.Create(model.Website);
            if (!rWebPage) return BadRequest(ModelState.AddErrors(rWebPage.Errors));
            var entity = await repository.GetCompanyProfile(cp => cp.Id == profileId);
            if (entity is null) return BadRequest();
            entity.Phone = model.Phone;
            entity.PhoneExt = model.PhoneExt;
            entity.Fax = model.Fax;
            entity.FaxExt = model.FaxExt;
            entity.Website = model.Website;
            repository.Update(entity);
            await repository.SaveChangesAsync();
            return Ok();
        }
    }
}