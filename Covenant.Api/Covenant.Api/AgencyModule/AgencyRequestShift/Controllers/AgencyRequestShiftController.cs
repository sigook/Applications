using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyRequestShift.Controllers
{
    [Route(RouteName)]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class AgencyRequestShiftController : Controller
    {
        public const string RouteName = "api/AgencyRequest/{requestId}/Shift";

        private readonly IRequestRepository requestRepository;
        private readonly IShiftRepository shiftRepository;

        public AgencyRequestShiftController(IRequestRepository requestRepository, IShiftRepository shiftRepository)
        {
            this.requestRepository = requestRepository;
            this.shiftRepository = shiftRepository;
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromRoute] Guid requestId, [FromBody] ShiftModel model)
        {
            if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
            var entity = await requestRepository.GetRequest(r => r.Id == requestId);
            if (entity is null) return BadRequest();
            var newShift = model.ToShift();
            entity.OnNewShift += async (sender, e) => await shiftRepository.Create(newShift);
            Result result = entity.UpdateShift(newShift);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            await requestRepository.Update(entity);
            await requestRepository.SaveChangesAsync();
            return Ok(new AgencyRequestDetailModel { Id = entity.Id, DisplayShift = newShift.DisplayShift });
        }
    }
}