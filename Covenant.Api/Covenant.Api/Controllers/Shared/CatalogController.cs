using Covenant.Api.Authorization;
using Covenant.Api.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Common.Models;
using Covenant.Common.Repositories;
using Covenant.Core.BL.Interfaces.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Covenant.Api.Controllers.Shared;

[Route(RouteName)]
[ApiController]
public class CatalogController(ICatalogRepository repository) : ControllerBase
{
    public const string RouteName = "api/catalog";
    private readonly ICatalogRepository _repository = repository;

    /// <summary>Gets the list of WSIB groups.</summary>
    [HttpGet("wsibgroup")]
    [ResponseCache(Duration = 3600)]
    [ProducesResponseType(typeof(List<BaseModel<Guid>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWsibGroup()
    {
        var wsibGroups = await _repository.GetWsibGroups();
        return Ok(wsibGroups);
    }

    /// <summary>Gets the list of availability options.</summary>
    [HttpGet("availability")]
    [ResponseCache(Duration = 3600)]
    [ProducesResponseType(typeof(List<BaseModel<Guid>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailability()
    {
        var availabilities = await _repository.GetAvailability();
        return Ok(availabilities);
    }

    /// <summary>Gets the list of availability time options.</summary>
    [HttpGet("availabilityTime")]
    [ResponseCache(Duration = 3600)]
    [ProducesResponseType(typeof(List<BaseModel<Guid>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailabilityTime()
    {
        var availabilityTimes = await _repository.GetAvailabilityTime();
        return Ok(availabilityTimes);
    }

    /// <summary>Gets the list of day options.</summary>
    [HttpGet("day")]
    [ResponseCache(Duration = 3600)]
    [ProducesResponseType(typeof(List<BaseModel<Guid>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDay()
    {
        var days = await _repository.GetDay();
        return Ok(days);
    }

    /// <summary>Gets the list of gender options.</summary>
    [HttpGet("gender")]
    [ResponseCache(Duration = 43200)]
    [ProducesResponseType(typeof(List<BaseModel<Guid>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGender()
    {
        var genders = await _repository.GetGender();
        return Ok(genders);
    }

    /// <summary>Gets the list of identification type options.</summary>
    [HttpGet("identificationType")]
    [ResponseCache(Duration = 3600)]
    [ProducesResponseType(typeof(List<IdentificationTypeModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetIdentificationType()
    {
        var identificationTypes = await _repository.GetIdentificationType();
        return Ok(identificationTypes);
    }

    /// <summary>Gets the list of language options.</summary>
    [HttpGet("language")]
    [ResponseCache(Duration = 3600)]
    [ProducesResponseType(typeof(List<BaseModel<Guid>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLanguage()
    {
        var languages = await _repository.GetLanguage();
        return Ok(languages);
    }

    /// <summary>Gets the list of lift options.</summary>
    [HttpGet("lift")]
    [ResponseCache(Duration = 3600)]
    [ProducesResponseType(typeof(List<BaseModel<Guid>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLif()
    {
        var lifts = await _repository.GetLift();
        return Ok(lifts);
    }

    /// <summary>Gets the list of industries.</summary>
    [HttpGet("industry")]
    [ProducesResponseType(typeof(List<BaseModel<Guid>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetIndustry()
    {
        var industries = await _repository.GetIndustries();
        return Ok(industries);
    }

    /// <summary>Gets the cancellation reasons.</summary>
    [HttpGet("reasonCancellationRequest")]
    [ResponseCache(Duration = 3600)]
    [ProducesResponseType(typeof(IEnumerable<BaseModel<Guid>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ReasonCancellationRequest()
    {
        var reasons = await _repository.GetReasonCancellationRequest();
        return Ok(reasons.Select(c => new BaseModel<Guid>
        {
            Id = c.Id,
            Value = c.Value
        }));
    }

    /// <summary>Gets the available company status values.</summary>
    [HttpGet("companyStatus")]
    [ResponseCache(Duration = 3600)]
    [ProducesResponseType(typeof(IEnumerable<BaseModel<int>>), StatusCodes.Status200OK)]
    public IActionResult GetCompanyStatus()
    {
        var statuses = Enum.GetValues<CompanyStatus>().Select(cs => new BaseModel<int>
        {
            Id = (int)cs,
            Value = cs.ToString()
        });
        return Ok(statuses);
    }

    /// <summary>Gets the predefined list of worker skills.</summary>
    [HttpGet("skills")]
    [ResponseCache(Duration = 43200)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Skills()
    {
        var skills = await _repository.GetSkillCatalog();
        return Ok(skills.Select(s => new SkillModel(s.Value)));
    }

    /// <summary>Gets the list of candidate sources.</summary>
    [HttpGet("source")]
    [ResponseCache(Duration = 43200)]
    [ProducesResponseType(typeof(List<BaseModel<Guid>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSources()
    {
        var sources = await _repository.GetSources();
        return Ok(sources);
    }

    /// <summary>Gets the list of sources eligible to be assigned to requests (job boards).</summary>
    [HttpGet("source/requests")]
    [ResponseCache(Duration = 43200)]
    [ProducesResponseType(typeof(List<BaseModel<Guid>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSourcesForRequests()
    {
        var sources = await _repository.GetSourcesForRequests();
        return Ok(sources);
    }

    /// <summary>Gets the available tax category values.</summary>
    [HttpGet("tax-categories")]
    [ResponseCache(Duration = 43200)]
    [ProducesResponseType(typeof(IEnumerable<BaseModel<int>>), StatusCodes.Status200OK)]
    public IActionResult GetTaxCategories()
    {
        var taxLevels = Enum.GetValues<TaxCategory>().Select(cs => new BaseModel<int>
        {
            Id = (int)cs,
            Value = cs.ToString()
        });
        return Ok(taxLevels);
    }

    /// <summary>Creates a new industry.</summary>
    /// <param name="model">Industry data to create.</param>
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [HttpPost("industry")]
    [ProducesResponseType(typeof(Industry), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddIndustry([FromServices] ICatalogService catalogService, [FromBody] BaseModel<Guid> model)
    {
        if (model is null) return BadRequest("Model can not be null");
        var result = await catalogService.AddIndustry(model.Value);
        if (!result) return BadRequest(result.StringErrors);
        return Ok(result.Value);
    }
}