using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyRequestWorkerTimeSheet.Controllers
{
    [Route(RouteName)]
    [ApiController]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class V2AgencyRequestWorkerTimeSheetController : ControllerBase
    {
        public const string RouteName = "api/v2/AgencyRequest/{requestId}/Worker/{workerId}/TimeSheet";

        private readonly ITimeSheetRepository timeSheetRepository;
        private readonly ITimesheetService timeSheetService;

        public V2AgencyRequestWorkerTimeSheetController(ITimeSheetRepository timeSheetRepository, ITimesheetService timeSheetService)
        {
            this.timeSheetRepository = timeSheetRepository;
            this.timeSheetService = timeSheetService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromRoute] Guid requestId, 
            [FromRoute] Guid workerId, 
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate) =>
            Ok(await timeSheetRepository.GetTimeSheetsListModel(workerId, requestId, startDate, endDate));

        [HttpPost]
        public async Task<IActionResult> Post([FromRoute] Guid requestId, [FromRoute] Guid workerId, [FromBody] TimeSheetModel model)
        {
            if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
            var result = await timeSheetService.CreateTimesheet(workerId, requestId, model);
            if (result) return Ok(new TimeSheetListModel { Id = result.Value });
            return BadRequest(ModelState.AddErrors(result.Errors));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] TimeSheetModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await timeSheetService.UpdateTimesheet(id, model);
            if (!result) return BadRequest(result.Errors);
            return Ok();
        }

        [HttpGet("{id:guid}/Usages")]
        public async Task<IActionResult> Usages([FromRoute] Guid requestId, [FromRoute] Guid workerId, [FromRoute] Guid id)
        {
            TimeSheetUsagesModel model = await timeSheetRepository.GetTimeSheetUsages(requestId, workerId, id);
            if (model is null) return NotFound();
            return Ok(model);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid requestId, Guid workerId, Guid id)
        {
            var result = await timeSheetService.RemoveTimeSheet(id);
            if (result)
            {
                return Ok();
            }
            return BadRequest(ModelState.AddErrors(result.Errors));
        }
    }
}