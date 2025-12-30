using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Agency;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyPersonnel.Controllers
{
    [Route(RouteName)]
    [ApiController]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class AgencyPersonnelController : ControllerBase
    {
        public const string RouteName = "api/AgencyPersonnel";
        private readonly IAgencyService agencyService;
        private readonly IAgencyRepository agencyRepository;

        public AgencyPersonnelController(IAgencyService agencyService, IAgencyRepository agencyRepository)
        {
            this.agencyService = agencyService;
            this.agencyRepository = agencyRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await agencyRepository.GetAllPersonnel(User.GetAgencyId()));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AgencyPersonnelModel model)
        {
            var result = await agencyService.CreateAgencyPersonnel(model);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            return Ok();
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var model = await agencyRepository.GetPersonnel(User.GetAgencyId(), id);
            if (model is null) return NotFound();
            return Ok(model);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(
            [FromServices] IIdentityServerService service,
            [FromRoute] Guid id)
        {
            Guid agencyId = User.GetAgencyId();
            var entity = await agencyRepository.GetPersonnel(id);
            if (entity is null || entity.AgencyId != agencyId) return NotFound();
            await agencyRepository.DeletePersonnel(entity);
            await agencyRepository.SaveChangesAsync();
            var result = await service.DeleteUserOrClaim(entity.UserId, new IdModel(agencyId));
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            return Ok();
        }
    }
}