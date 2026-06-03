using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        private readonly ITimesheetRepository timeSheetRepository;
        private readonly ITimesheetService timeSheetService;

        public V2AgencyRequestWorkerTimeSheetController(ITimesheetRepository timeSheetRepository, ITimesheetService timeSheetService)
        {
            this.timeSheetRepository = timeSheetRepository;
            this.timeSheetService = timeSheetService;
        }

        /// <summary>Gets the timesheets of a worker for a request within an optional date range.</summary>
        /// <param name="requestId">Identifier of the request.</param>
        /// <param name="workerId">Identifier of the worker.</param>
        /// <param name="startDate">Optional start date filter.</param>
        /// <param name="endDate">Optional end date filter.</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TimeSheetListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(
            [FromRoute] Guid requestId,
            [FromRoute] Guid workerId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate) =>
            Ok(await timeSheetRepository.GetTimeSheetsListModel(workerId, requestId, startDate, endDate));

        /// <summary>Creates a timesheet for a worker on the specified request.</summary>
        /// <param name="requestId">Identifier of the request.</param>
        /// <param name="workerId">Identifier of the worker.</param>
        /// <param name="model">Timesheet data.</param>
        [HttpPost]
        [ProducesResponseType(typeof(TimeSheetListModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromRoute] Guid requestId, [FromRoute] Guid workerId, [FromBody] TimeSheetModel model)
        {
            if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
            var result = await timeSheetService.CreateTimesheet(workerId, requestId, model);
            if (result) return Ok(new TimeSheetListModel { Id = result.Value });
            return BadRequest(ModelState.AddErrors(result.Errors));
        }

        /// <summary>Updates an existing timesheet.</summary>
        /// <param name="id">Identifier of the timesheet to update.</param>
        /// <param name="model">Updated timesheet data.</param>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] TimeSheetModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await timeSheetService.UpdateTimesheet(id, model);
            if (!result) return BadRequest(result.Errors);
            return Ok();
        }

        /// <summary>Gets the usage information of a specific timesheet.</summary>
        /// <param name="requestId">Identifier of the request.</param>
        /// <param name="workerId">Identifier of the worker.</param>
        /// <param name="id">Identifier of the timesheet.</param>
        [HttpGet("{id:guid}/Usages")]
        [ProducesResponseType(typeof(TimeSheetUsagesModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Usages([FromRoute] Guid requestId, [FromRoute] Guid workerId, [FromRoute] Guid id)
        {
            TimeSheetUsagesModel model = await timeSheetRepository.GetTimeSheetUsages(requestId, workerId, id);
            if (model is null) return NotFound();
            return Ok(model);
        }

        /// <summary>Deletes a timesheet.</summary>
        /// <param name="requestId">Identifier of the request.</param>
        /// <param name="workerId">Identifier of the worker.</param>
        /// <param name="id">Identifier of the timesheet to delete.</param>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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