using Covenant.Api.Authorization;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.CompanyProfiles;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class LogoController : Controller
{
    public const string RouteName = "api/agency/companyprofiles/{profileId}/Logo";

    /// <summary>Updates the logo of a company profile.</summary>
    /// <param name="repository">Company repository.</param>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="model">Logo file information.</param>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
