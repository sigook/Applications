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
using Microsoft.AspNetCore.Http;
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

        /// <summary>Creates a new document for the specified company profile.</summary>
        /// <param name="profileId">Identifier of the company profile.</param>
        /// <param name="model">Document data.</param>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>Gets a paginated list of documents for the specified company profile.</summary>
        /// <param name="profileId">Identifier of the company profile.</param>
        /// <param name="pagination">Pagination parameters.</param>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<CompanyProfileDocumentModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] Guid profileId, [FromQuery] Pagination pagination)
        {
            return Ok(await agencyService.GetCompanyDocuments(profileId, pagination));
        }


        /// <summary>Deletes a company profile document.</summary>
        /// <param name="id">Identifier of the document to delete.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await agencyService.DeleteCompanyDocument(id);
            return Ok();
        }
    }
}