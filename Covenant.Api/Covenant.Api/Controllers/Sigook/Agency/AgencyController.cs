using Covenant.Api.Authorization;
using Covenant.Api.Utils;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Models.Agency;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ApiController]
[Produces("application/json")]
[ServiceFilter(typeof(AgencyIdFilter))]
public class AgencyController : ControllerBase
{
    public const string RouteName = "api/Agency";

    private readonly IAgencyService agencyService;
    private readonly IAgencyRepository agencyRepository;
    private readonly IDefaultLogoProvider _defaultLogoProvider;

    public AgencyController(
        IAgencyService agencyService,
        IAgencyRepository repository,
        IDefaultLogoProvider defaultLogoProvider)
    {
        this.agencyService = agencyService;
        agencyRepository = repository;
        _defaultLogoProvider = defaultLogoProvider;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAgencies([FromQuery] GetAgenciesFilter filter)
    {
        var data = await agencyRepository.GetAgencies(User.GetAgencyId(), filter);
        return Ok(data);
    }

    [HttpGet("{agencyId}")]
    public async Task<IActionResult> GetAgency([FromRoute] Guid agencyId)
    {
        var agency = await agencyRepository.GetAgencyDetail(agencyId);
        return Ok(agency);
    }

    [HttpGet("Profile")]
    public async Task<IActionResult> GetAgencyProfile()
    {
        Guid agencyId = User.GetAgencyId();
        var agency = await agencyRepository.GetAgencyDetail(agencyId);
        if (agency is null) return NotFound();
        return Ok(agency);
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] AgencyModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        Guid agencyId = User.GetAgencyId();
        var entity = await agencyRepository.GetAgency(agencyId);
        if (entity is null) return BadRequest();
        if (string.IsNullOrEmpty(model.Logo?.FileName)) model.Logo = await _defaultLogoProvider.GetLogo(entity.FullName);
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

    [HttpPost]
    public async Task<IActionResult> CreateAgency([FromBody] AgencyModel model)
    {
        var result = await agencyService.CreateAgency(model);
        if (result) return Ok();
        return BadRequest(ModelState.AddErrors(result.Errors));
    }
}