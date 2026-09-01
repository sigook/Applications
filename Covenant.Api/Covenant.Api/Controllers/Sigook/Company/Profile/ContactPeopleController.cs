using Covenant.Api.Authorization;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Company.Profile;

[Route(RouteName)]
[ApiController]
[Authorize(Policy = PolicyConfiguration.Company)]
[ServiceFilter(typeof(CompanyIdFilter))]
public class ContactPeopleController(ICompanyRepository companyRepository, ICompanyService companyService) : ControllerBase
{
    public const string RouteName = "api/company/profile/ContactPeople";

    /// <summary>Gets all contact persons of the current company.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CompanyProfileContactPersonModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllContactPeople()
    {
        var contacts = await companyRepository.GetContactPeople(c => c.CompanyProfile.CompanyId == User.GetCompanyId());
        return Ok(contacts);
    }

    /// <summary>Creates a new contact person for the company.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateContact([FromBody] CompanyProfileContactPersonModel model)
    {
        await companyService.CreateContact(model);
        return Ok();
    }

    /// <summary>Deletes a contact person by its identifier.</summary>
    /// <param name="id">Contact person identifier.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteContactPerson(Guid id)
    {
        var contact = await companyRepository.GetContactPerson(id);
        companyRepository.Delete(contact);
        await companyRepository.SaveChangesAsync();
        return NoContent();
    }
}
