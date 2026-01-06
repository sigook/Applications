using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyCompanyProfileDocument.Controllers
{
    [Route(RouteName)]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class AgencyCompanyProfileDocumentController : Controller
    {
        public const string RouteName = "api/AgencyCompanyProfile/{profileId}/Document";

        private readonly IAgencyService agencyService;

        public AgencyCompanyProfileDocumentController(IAgencyService agencyService)
        {
            this.agencyService = agencyService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromRoute] Guid profileId, [FromBody] CompanyProfileDocumentModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await agencyService.CreateCompanyDocument(profileId, model);
            if (!entity)
            {
                return BadRequest(ModelState.AddErrors(entity.Errors));
            }
            return Ok(entity.Value.DocumentId);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] Guid profileId, [FromQuery] Pagination pagination)
        {
            return Ok(await agencyService.GetCompanyDocuments(profileId, pagination));
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await agencyService.DeleteCompanyDocument(id);
            return Ok();
        }
    }
}