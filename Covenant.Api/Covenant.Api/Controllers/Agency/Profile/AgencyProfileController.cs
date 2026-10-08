using Covenant.Api.Authorization;
using Covenant.Api.Utils;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Models.Agency;
using Covenant.Common.Repositories.Agencies;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Agency.Profile;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ApiController]
[Produces("application/json")]
[ServiceFilter(typeof(AgencyIdFilter))]
public class AgencyProfileController(IAgencyRepository agencyRepository, IDefaultLogoProvider defaultLogoProvider) : ControllerBase
{
    public const string RouteName = "api/agency/profile";

    /// <summary>Gets the profile detail of the current agency.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(AgencyModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAgencyProfile()
    {
        Guid agencyId = User.GetAgencyId();
        var agency = await agencyRepository.GetAgencyDetail(agencyId);
        if (agency is null) return NotFound();
        return Ok(agency);
    }

    /// <summary>Updates the profile of the current agency.</summary>
    /// <param name="model">Updated agency data.</param>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromBody] AgencyModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        Guid agencyId = User.GetAgencyId();
        var entity = await agencyRepository.GetAgency(agencyId);
        if (entity is null) return BadRequest();
        if (string.IsNullOrEmpty(model.Logo?.FileName)) model.Logo = await defaultLogoProvider.GetLogo(entity.FullName);
        entity.BusinessNumber = model.BusinessNumber;
        entity.FullName = model.FullName;
        entity.HstNumber = model.HstNumber;
        entity.LogoId = model.Logo.Id;
        entity.PhonePrincipal = model.PhonePrincipal;
        entity.PhonePrincipalExt = model.PhonePrincipalExt;
        entity.WebPage = model.WebPage;
        entity.WsibGroup.Clear();
        foreach (var wsibGroup in model.WsibGroup)
        {
            var item = new AgencyWsibGroup(entity.Id, wsibGroup.Id);
            entity.WsibGroup.Add(item);
        }
        entity.ContactInformation.Clear();
        foreach (var contactInformation in model.ContactInformation)
        {
            var item = new AgencyContactInformation
            {
                Title = contactInformation.Title,
                FirstName = contactInformation.FirstName,
                MiddleName = contactInformation.MiddleName,
                LastName = contactInformation.LastName,
                MobileNumber = contactInformation.MobileNumber,
                OfficeNumber = contactInformation.OfficeNumber,
                OfficeNumberExt = contactInformation.OfficeNumberExt,
                Email = contactInformation.Email,
                Position = contactInformation.Position
            };
            entity.ContactInformation.Add(item);
        }
        agencyRepository.Update(entity);
        await agencyRepository.SaveChangesAsync();
        return Ok();
    }
}
