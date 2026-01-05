using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost]
        public async Task<IActionResult> Post(Guid profileId, [FromBody] CompanyProfileContactPersonModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await agencyService.CreateCompanyContactPerson(profileId, model);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            return CreatedAtAction(nameof(GetById), new { id = result.Value }, new CompanyProfileContactPersonModel { Id = result.Value });
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid profileId) => Ok(await _companyRepository.GetContactPersons(c => c.CompanyProfileId == profileId));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid profileId, Guid id)
        {
            var model = await _companyRepository.GetContactPersonDetail(profileId, id);
            if (model is null) return NotFound();
            return Ok(model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid profileId, Guid id, [FromBody] CompanyProfileContactPersonModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await agencyService.UpdateCompanyContactPerson(id, model);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var contact = await _companyRepository.GetContactPerson(id);
            _companyRepository.Delete(contact);
            await _companyRepository.SaveChangesAsync();
            return Ok();
        }
    }
}