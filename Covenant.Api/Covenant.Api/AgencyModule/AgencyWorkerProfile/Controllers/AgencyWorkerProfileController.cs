using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Constants;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Security;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using Covenant.Documents.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyWorkerProfile.Controllers
{
    [Route(RouteName)]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class AgencyWorkerProfileController : Controller
    {
        public const string RouteName = "api/AgencyWorkerProfile";
        private IMediator mediator;

        private readonly IWorkerRepository _workerRepository;
        private readonly IWorkerService workerService;
        private readonly IAgencyService agencyService;

        public AgencyWorkerProfileController(
            IWorkerRepository workerRepository,
            IWorkerService workerService,
            IAgencyService agencyService)
        {
            _workerRepository = workerRepository;
            this.workerService = workerService;
            this.agencyService = agencyService;
        }

        protected IMediator Mediator => mediator ?? (mediator = HttpContext.RequestServices.GetService<IMediator>());

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetWorkerProfileFilter filter) =>
            Ok(await _workerRepository.GetWorkersProfile(User.GetAgencyId(), filter));

        [HttpGet("File")]
        public async Task<IActionResult> GetFile([FromQuery] GetWorkerProfileFilter filter)
        {
            var data = _workerRepository.GetAllWorkersProfile(User.GetAgencyId(), filter).ToList();
            var file = await Mediator.Send(new GenerateAgencyWorkersProfileReport(data));
            return File(file.Document.ToArray(), CovenantConstants.ExcelMime, file.DocumentName);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateWorkerProfile([FromQuery] int? orderId)
        {
            var result = await workerService.CreateWorker(orderId);
            if (result)
            {
                return Ok(result.Value);
            }
            return BadRequest(ModelState.AddErrors(result.Errors));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var detail = await _workerRepository.GetWorkerProfileDetail(id);
            if (detail is null) return NotFound();
            return Ok(detail);
        }

        [HttpGet("{id:guid}/OtherDocument")]
        public async Task<IActionResult> OtherDocument(Guid id) =>
            Ok(await _workerRepository.GetOtherDocuments(id));

        [HttpPut("{id:guid}/Dnu")]
        public async Task<IActionResult> Dnu(Guid id)
        {
            Guid agencyId = User.GetAgencyId();
            var entity = await _workerRepository.GetProfile(w => w.Id == id && w.AgencyId == agencyId);
            if (entity is null) return BadRequest();
            entity.UpdateDnu();
            await _workerRepository.UpdateProfile(entity);
            await _workerRepository.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:guid}/IsContractor")]
        public async Task<IActionResult> IsContractor([FromServices] ITimeService service, Guid id)
        {
            Guid agencyId = User.GetAgencyId();
            var entity = await _workerRepository.GetProfile(w => w.Id == id && w.AgencyId == agencyId);
            if (entity is null) return BadRequest();
            var result = entity.UpdateContractor(service.GetCurrentDateTime());
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            await _workerRepository.UpdateProfile(entity);
            await _workerRepository.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:guid}/IsSubcontractor")]
        public async Task<IActionResult> IsSubcontractor([FromServices] ITimeService service, Guid id)
        {
            Guid agencyId = User.GetAgencyId();
            var entity = await _workerRepository.GetProfile(w => w.Id == id && w.AgencyId == agencyId);
            if (entity is null) return BadRequest();
            var result = entity.UpdateSubcontractor(service.GetCurrentDateTime());
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            entity.UpdateTextSearch();
            await _workerRepository.UpdateProfile(entity);
            await _workerRepository.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{workerProfileId:guid}/Email")]
        public async Task<IActionResult> Email([FromServices] IIdentityServerService service, [FromRoute] Guid workerProfileId, [FromBody] UpdateEmailModel model)
        {
            Guid agencyId = User.GetAgencyId();
            var profile = await _workerRepository.GetProfile(c => c.Id == workerProfileId && c.AgencyId == agencyId);
            if (profile is null) return BadRequest();
            var rUpdate = await service.UpdateUserEmail(new UpdateEmailModel(profile.WorkerId) { NewEmail = model?.NewEmail });
            if (!rUpdate) return BadRequest(ModelState.AddErrors(rUpdate.Errors));
            return Ok();
        }

        [HttpPut("{id}/ApprovedToWork")]
        public async Task<IActionResult> UpdateApprovedToWork([FromServices] ITimeService timeService, Guid id)
        {
            var entity = await _workerRepository.GetProfile(p => p.Id == id);
            if (entity is null || entity.AgencyId != User.GetAgencyId()) return BadRequest();
            Result result = entity.UpdateApprovedToWork(timeService.GetCurrentDateTime());
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            entity.UpdateTextSearch();
            await _workerRepository.UpdateProfile(entity);
            await _workerRepository.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id}/tax-category")]
        public async Task<IActionResult> UpdateTaxCategory([FromRoute] Guid id, [FromBody] WorkerProfileDetailModel model)
        {
            await agencyService.UpdateWorkerProfileTaxCategory(id, model);
            return Ok();
        }

        [HttpPut("{id}/tax-rate")]
        public async Task<IActionResult> UpdateTaxRate([FromRoute] Guid id, [FromBody] WorkerProfileDetailModel model)
        {
            await agencyService.UpdateWorkerProfileTaxRate(id, model);
            return Ok();
        }

        [HttpGet("Dropdown")]
        public async Task<IActionResult> GetAllWorkerProfiles([FromQuery] string searchTerm)
        {
            var data = await _workerRepository.GetWorkerProfilesDropdown(User.GetAgencyIds(), searchTerm);
            return Ok(data);
        }
    }
}