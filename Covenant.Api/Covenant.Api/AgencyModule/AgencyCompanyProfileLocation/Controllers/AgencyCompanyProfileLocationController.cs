using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyCompanyProfileLocation.Controllers
{
    [Route(RouteName)]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ServiceFilter(typeof(AgencyIdFilter))]
    [ApiController]
    public class AgencyCompanyProfileLocationController : ControllerBase
    {
        private readonly ICompanyRepository _repository;
        private readonly ICompanyService companyService;
        public const string RouteName = "api/AgencyCompanyProfile/{profileId}/Location";
        public AgencyCompanyProfileLocationController(ICompanyRepository repository, ICompanyService companyService)
        {
            _repository = repository;
            this.companyService = companyService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromRoute] Guid profileId, [FromBody] CompanyProfileLocationDetailModel model)
        {
            if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
            var result = await companyService.CreateCompanyLocation(model, profileId);
            if (!result)
            {
                return BadRequest(ModelState.AddErrors(result.Errors));
            }
            return CreatedAtAction(nameof(GetById), new { id = result.Value }, new CompanyProfileLocationDetailModel { Id = result.Value });
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] Guid profileId)
        {
            var data = await _repository.GetCompanyLocations(c => c.CompanyProfileId == profileId);
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid profileId, [FromRoute] Guid id)
        {
            CompanyProfileLocationDetailModel model = await _repository.GetLocationDetail(id);
            if (model is null) return NotFound();
            return Ok(model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] Guid profileId, [FromRoute] Guid id, [FromBody] CompanyProfileLocationDetailModel model)
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
        public async Task<IActionResult> Delete([FromRoute] Guid profileId, [FromRoute] Guid id)
        {
            var location = await _repository.GetLocation(id);
            _repository.Delete(location);
            await _repository.SaveChangesAsync();
            return Ok();
        }
    }
}