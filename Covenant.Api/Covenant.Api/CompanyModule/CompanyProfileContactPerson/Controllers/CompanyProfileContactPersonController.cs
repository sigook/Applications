using Covenant.Common.Entities.Company;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.CompanyModule.CompanyProfileContactPerson.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyProfileContactPersonController : ControllerBase
    {
        private readonly ICompanyRepository companyRepository;
        private readonly ICompanyService companyService;

        public CompanyProfileContactPersonController(ICompanyRepository companyRepository, ICompanyService companyService)
        {
            this.companyRepository = companyRepository;
            this.companyService = companyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllContactPersons()
        {
            var contacts = await companyRepository.GetContactPersons(c => c.CompanyProfile.CompanyId == User.GetCompanyId());
            return Ok(contacts);
        }

        [HttpPost]
        public async Task<IActionResult> CreateContact([FromBody] CompanyProfileContactPersonModel model)
        {
            await companyService.CreateContact(model);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContactPerson(Guid id)
        {
            var contact = await companyRepository.GetContactPerson(id);
            companyRepository.Delete(contact);
            await companyRepository.SaveChangesAsync();
            return NoContent();
        }
    }
}
