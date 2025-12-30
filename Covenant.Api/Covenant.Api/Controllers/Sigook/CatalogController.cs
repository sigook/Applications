using Covenant.Api.Authorization;
using Covenant.Api.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Common.Models;
using Covenant.Common.Models.Location;
using Covenant.Common.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Covenant.Api.Controllers.Sigook
{
    [Route(RouteName)]
    [ApiController]
    [AllowAnonymous]
    public class CatalogController : ControllerBase
    {
        public const string RouteName = "api/Catalog";
        private readonly ICatalogRepository _repository;

        public CatalogController(ICatalogRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("wsibgroup")]
        [ResponseCache(Duration = 3600)]
        public async Task<IActionResult> GetWsibGroup()
        {
            var wsibGroups = await _repository.GetWsibGroups();
            return Ok(wsibGroups);
        }

        [HttpGet("availability")]
        [ResponseCache(Duration = 3600)]
        public async Task<IActionResult> GetAvailability()
        {
            var availabilities = await _repository.GetAvailability();
            return Ok(availabilities);
        }

        [HttpGet("availabilityTime")]
        [ResponseCache(Duration = 3600)]
        public async Task<IActionResult> GetAvailabilityTime()
        {
            var availabilityTimes = await _repository.GetAvailabilityTime();
            return Ok(availabilityTimes);
        }

        [HttpGet("day")]
        [ResponseCache(Duration = 3600)]
        public async Task<IActionResult> GetDay()
        {
            var days = await _repository.GetDay();
            return Ok(days);
        }

        [HttpGet("gender")]
        [ResponseCache(Duration = 43200)]
        public async Task<IActionResult> GetGender()
        {
            var genders = await _repository.GetGender();
            return Ok(genders);
        }

        [HttpGet("identificationType")]
        [ResponseCache(Duration = 3600)]
        public async Task<IActionResult> GetIdentificationType()
        {
            var identificationTypes = await _repository.GetIdentificationType();
            return Ok(identificationTypes);
        }

        [HttpGet("language")]
        [ResponseCache(Duration = 3600)]
        public async Task<IActionResult> GetLanguage()
        {
            var languages = await _repository.GetLanguage();
            return Ok(languages);
        }

        [HttpGet("lift")]
        [ResponseCache(Duration = 3600)]
        public async Task<IActionResult> GetLif()
        {
            var lifts = await _repository.GetLift();
            return Ok(lifts);
        }

        [HttpGet("industry")]
        public async Task<IActionResult> GetIndustry()
        {
            var industries = await _repository.GetIndustries();
            return Ok(industries);
        }

        [HttpGet("jobPosition")]
        [ResponseCache(Duration = 3600)]
        public async Task<IActionResult> GetJobPosition() => Ok(await _repository.GetJobPositions());

        [HttpGet("reasonCancellationRequest")]
        [ResponseCache(Duration = 3600)]
        public async Task<IActionResult> ReasonCancellationRequest()
        {
            var jobPositions = await _repository.GetReasonCancellationRequest();
            switch (CultureInfo.CurrentCulture.Name)
            {
                case ApiServicesConfiguration.FrCulture:
                    return Ok(jobPositions.Select(c => new BaseModel<Guid>
                    {
                        Id = c.Id,
                        Value = c.Value.Fr ?? c.Value.En
                    }));
                case ApiServicesConfiguration.EsCulture:
                    return Ok(jobPositions.Select(c => new BaseModel<Guid>
                    {
                        Id = c.Id,
                        Value = c.Value.Es ?? c.Value.En
                    }));
                default:
                    return Ok(jobPositions.Select(c => new BaseModel<Guid>
                    {
                        Id = c.Id,
                        Value = c.Value.En
                    }));
            }
        }

        [HttpGet("companyStatus")]
        [ResponseCache(Duration = 3600)]
        public IActionResult GetCompanyStatus()
        {
            var statuses = Enum.GetValues<CompanyStatus>().Select(cs => new BaseModel<int>
            {
                Id = (int)cs,
                Value = cs.ToString()
            });
            return Ok(statuses);
        }

        [HttpGet("skills")]
        [ResponseCache(Duration = 43200)]
        public IActionResult Skills()
        {
            var skills = new[]
            {
                new {Skill = "Packing"},
                new {Skill = "Shipping/Receiving"},
                new {Skill = "Machine operator"},
                new {Skill = "Assembly"},
                new {Skill = "Order picking"},
                new {Skill = "Ticketing"},
                new {Skill = "Soldering"},
                new {Skill = "Inventory"},
                new {Skill = "RF Scanning"},
                new {Skill = "Computer"},
                new {Skill = "Accounting"},
                new {Skill = "Reception"},
                new {Skill = "Administration Assistant"},
                new {Skill = "Data Entry"},
                new {Skill = "Administration"},
                new {Skill = "Administrative"},
                new {Skill = "Az Driver"},
                new {Skill = "Truck Driver"},
                new {Skill = "Bakery"},
                new {Skill = "Cleaning"},
                new {Skill = "Food Service"},
                new {Skill = "Clerical"},
                new {Skill = "Construction"},
                new {Skill = "Counter Balance"},
                new {Skill = "Customer Service"},
                new {Skill = "Data Analytics"},
                new {Skill = "Database"},
                new {Skill = "Delivery"},
                new {Skill = "Dispatcher"},
                new {Skill = "Driver"},
                new {Skill = "Electrician"},
                new {Skill = "Forklift"},
                new {Skill = "General Labour"},
                new {Skill = "General Packer"},
                new {Skill = "Heavy Lifting"},
                new {Skill = "Logistics"},
                new {Skill = "Lumper"},
                new {Skill = "Machine Builder"},
                new {Skill = "Mixer"},
                new {Skill = "Plumbing"},
                new {Skill = "Recruiting"},
                new {Skill = "Sanitation"},
                new {Skill = "Welding"},
                new {Skill = "Training"},
            };
            return Ok(skills.OrderBy(s => s.Skill));
        }

        [HttpGet("tax-categories")]
        [ResponseCache(Duration = 43200)]
        public IActionResult GetTaxCategories()
        {
            var taxLevels = Enum.GetValues<TaxCategory>().Select(cs => new BaseModel<int>
            {
                Id = (int)cs,
                Value = cs.ToString()
            });
            return Ok(taxLevels);
        }

        [Authorize(Policy = PolicyConfiguration.Agency)]
        [HttpPost("industry")]
        public async Task<IActionResult> AddIndustry([FromBody] BaseModel<Guid> model)
        {
            if (model != null)
            {
                var industry = Industry.Create(model.Value);
                await _repository.Create(industry.Value);
                await _repository.SaveChangesAsync();
                return Ok(industry.Value);
            }
            return BadRequest("Model can not be null");
        }
    }
}