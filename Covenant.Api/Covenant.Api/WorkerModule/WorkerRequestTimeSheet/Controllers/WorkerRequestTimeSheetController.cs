using Covenant.Api.Authorization;
using Covenant.Api.Utils;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.WorkerModule.WorkerRequestTimeSheet.Controllers
{
    [Route(RouteName)]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Policy = PolicyConfiguration.Worker)]
    [LogBadRequestFilter]
    public class WorkerRequestTimeSheetController : ControllerBase
    {
        public const string RouteName = "api/WorkerRequest/{requestId}/TimeSheet";
        private readonly ITimesheetService timeSheetService;
        private readonly ITimeSheetRepository _timeSheetRepository;

        public WorkerRequestTimeSheetController(ITimesheetService timeSheetService, ITimeSheetRepository timeSheetRepository)
        {
            this.timeSheetService = timeSheetService;
            _timeSheetRepository = timeSheetRepository;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Get(Guid requestId, Pagination pagination) => Ok(await _timeSheetRepository.GetTimeSheetsForWorker(User.GetUserId(), requestId, pagination));

        [HttpPost]
        public async Task<IActionResult> Post([FromRoute] Guid requestId, [FromBody] WorkerLocationModel model)
        {
            var result = await timeSheetService.Register(requestId, model);
            if (result) return Ok(result.Value);
            return BadRequest(ModelState.AddErrors(result.Errors));
        }

        [HttpGet("clock-type")]
        public async Task<IActionResult> GetClockType([FromRoute] Guid requestId, [FromQuery] DateTime? date)
        {
            var result = await timeSheetService.GetClockType(requestId, date);
            return Ok(result.Value);
        }
    }
}