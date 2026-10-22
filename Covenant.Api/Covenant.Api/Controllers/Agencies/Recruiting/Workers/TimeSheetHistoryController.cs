using Covenant.Api.Authorization;
using Covenant.Common.Models.Request.TimeSheet.Agency;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Requests;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Agency.Recruiting.Workers;

[ApiController]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
[Route(RouteName)]
public class TimeSheetHistoryController(ITimesheetRepository timeSheetRepository) : ControllerBase
{
    public const string RouteName = "api/agency/recruiting/workers/{workerProfileId:guid}/timesheethistory";

    /// <summary>Gets the paginated timesheet history for a worker profile with running totals.</summary>
    /// <param name="workerProfileId">Worker profile identifier.</param>
    /// <param name="filter">Filter, sort and pagination criteria.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<TimeSheetHistoryModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid workerProfileId, [FromQuery] GetWorkerTimeSheetHistoryFilter filter) =>
        Ok(await timeSheetRepository.GetTimeSheetHistory(User.GetAgencyId(), workerProfileId, filter));
}
