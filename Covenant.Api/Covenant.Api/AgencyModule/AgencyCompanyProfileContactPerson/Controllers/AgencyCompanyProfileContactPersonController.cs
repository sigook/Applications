using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyCompanyProfileContactPerson.Controllers
{
    [Route(RouteName)]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class AgencyCompanyProfileContactPersonController : Controller
    {
        public const string RouteName = "api/AgencyCompanyProfile/{profileId}/ContactPerson";
        private readonly ICompanyRepository _companyRepository;
        private readonly IAgencyService agencyService;

        public AgencyCompanyProfileContactPersonController(ICompanyRepository companyRepository, IAgencyService agencyService)
        {
            _companyRepository = companyRepository;
            this.agencyService = agencyService;
        }

        /// <summary>Creates a new contact person for the specified company profile.</summary>
        /// <param name="profileId">Identifier of the company profile.</param>
        /// <param name="model">Contact person data.</param>
        [HttpPost]
        [ProducesResponseType(typeof(CompanyProfileContactPersonModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(Guid profileId, [FromBody] CompanyProfileContactPersonModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await agencyService.CreateCompanyContactPerson(profileId, model);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            return CreatedAtAction(nameof(GetById), new { id = result.Value }, new CompanyProfileContactPersonModel { Id = result.Value });
        }

        /// <summary>Gets the contact persons of the specified company profile.</summary>
        /// <param name="profileId">Identifier of the company profile.</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CompanyProfileContactPersonModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(Guid profileId) => Ok(await _companyRepository.GetContactPersons(c => c.CompanyProfileId == profileId));

        /// <summary>Gets the detail of a company profile contact person by its identifier.</summary>
        /// <param name="profileId">Identifier of the company profile.</param>
        /// <param name="id">Identifier of the contact person.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CompanyProfileContactPersonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid profileId, Guid id)
        {
            var model = await _companyRepository.GetContactPersonDetail(profileId, id);
            if (model is null) return NotFound();
            return Ok(model);
        }

        /// <summary>Updates a company profile contact person.</summary>
        /// <param name="profileId">Identifier of the company profile.</param>
        /// <param name="id">Identifier of the contact person to update.</param>
        /// <param name="model">Updated contact person data.</param>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(Guid profileId, Guid id, [FromBody] CompanyProfileContactPersonModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await agencyService.UpdateCompanyContactPerson(id, model);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            return Ok();
        }

        /// <summary>Deletes a company profile contact person.</summary>
        /// <param name="id">Identifier of the contact person to delete.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var contact = await _companyRepository.GetContactPerson(id);
            _companyRepository.Delete(contact);
            await _companyRepository.SaveChangesAsync();
            return Ok();
        }
    }
}