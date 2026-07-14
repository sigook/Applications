using Covenant.Api.CompanyModule.CompanyRequestWorkerTimeSheet.Models;
using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Covenant.Infrastructure.Repositories.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.CompanyModule.CompanyRequestWorkerTimeSheet.Controllers
{
    [Route(RouteName)]
    [ApiController]
    [Authorize(Policy = PolicyConfiguration.Company)]
    [ServiceFilter(typeof(CompanyIdFilter))]
    public class V2CompanyRequestWorkerTimeSheetController : ControllerBase
    {
        public const string RouteName = "api/v2/CompanyRequest/{requestId}/Worker/{workerId}/TimeSheet";
        private readonly ITimesheetService timeSheetService;
        private readonly ITimesheetRepository timeSheetRepository;

        public V2CompanyRequestWorkerTimeSheetController(ITimesheetService timeSheetService, ITimesheetRepository timeSheetRepository)
        {
            this.timeSheetService = timeSheetService;
            this.timeSheetRepository = timeSheetRepository;
        }

        /// <summary>Creates a timesheet for a worker on the specified request.</summary>
        /// <param name="requestId">Request identifier.</param>
        /// <param name="workerId">Worker identifier.</param>
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
        /// <param name="id">Timesheet identifier.</param>
        /// <param name="model">Updated timesheet data.</param>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] TimeSheetModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await timeSheetService.UpdateTimesheet(id, model);
            if (!result) return BadRequest(result.Errors);
            return Ok();
        }

        /// <summary>Approves all completed timesheets for a worker on the specified request.</summary>
        /// <param name="requestId">Request identifier.</param>
        /// <param name="workerId">Worker identifier.</param>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromRoute] Guid requestId, [FromRoute] Guid workerId)
        {
            var timeSheets = await timeSheetRepository.GetTimeSheets(workerId, requestId, r => r.CompanyId == User.GetCompanyId());
            foreach (var timeSheet in timeSheets)
            {
                if (timeSheet.TimeOut.HasValue)
                {
                    var result = timeSheet.AddApprovedTime(timeSheet.TimeIn, timeSheet.TimeOut.GetValueOrDefault());
                    if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
                    timeSheet.Comment = $"Approved on {DateTime.Now:D}";
                }
            }
            await timeSheetRepository.SaveChangesAsync();
            return Ok();
        }

        /// <summary>Registers a clock-in entry for a worker on the specified request.</summary>
        /// <param name="requestId">Request identifier.</param>
        /// <param name="workerId">Worker identifier.</param>
        /// <param name="model">Clock-in data.</param>
        [HttpPost("ClockIn")]
        [ProducesResponseType(typeof(RegisterTimeSheetResultModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ClockIn([FromRoute] Guid requestId, [FromRoute] Guid workerId, [FromBody] CompanyClockInModel model)
        {
            if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
            Result<RegisterTimeSheetResultModel> result = await timeSheetService.AddClockIn(requestId, workerId, model.ClockIn);
            if (result) return Ok(result.Value);
            return BadRequest(ModelState.AddErrors(result.Errors));
        }

        /// <summary>Gets the timesheets of a worker on a request within an optional date range.</summary>
        /// <param name="requestId">Request identifier.</param>
        /// <param name="workerId">Worker identifier.</param>
        /// <param name="startDate">Optional range start date.</param>
        /// <param name="endDate">Optional range end date.</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TimeSheetListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(
            [FromRoute] Guid requestId,
            [FromRoute] Guid workerId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate) =>
            Ok(await timeSheetRepository.GetTimeSheetsListModel(workerId, requestId, startDate, endDate));

        /// <summary>Deletes a timesheet by its identifier.</summary>
        /// <param name="requestId">Request identifier.</param>
        /// <param name="workerId">Worker identifier.</param>
        /// <param name="id">Timesheet identifier.</param>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute] Guid requestId, [FromRoute] Guid workerId, [FromRoute] Guid id)
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