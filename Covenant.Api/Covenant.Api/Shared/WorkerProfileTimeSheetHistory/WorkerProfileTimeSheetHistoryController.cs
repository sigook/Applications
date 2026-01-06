using Covenant.Api.Authorization;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Shared.WorkerProfileTimeSheetHistory;

[ApiController]
[Authorize(Policy = PolicyConfiguration.Agency)]
[Route(Route)]
public class WorkerProfileTimeSheetHistoryController : ControllerBase
{
    private readonly ITimeSheetRepository _timeSheetRepository;
    public const string Route = "api/WorkerProfile/{workerProfileId}/TimeSheetHistory";

    public WorkerProfileTimeSheetHistoryController(ITimeSheetRepository timeSheetRepository) => _timeSheetRepository = timeSheetRepository;

    [HttpGet]
    public async Task<IActionResult> Get(Guid workerProfileId, Pagination pagination) =>
        Ok(await _timeSheetRepository.GetTimeSheetHistory(workerProfileId, pagination));

    [HttpGet("{rowNumber}")]
    public async Task<IActionResult> GetTimeSheetHistoryAccumulated([FromRoute] Guid workerProfileId, [FromRoute] int rowNumber)
    {
        var data = await _timeSheetRepository.GetTimesheetHistoryAccumulated(workerProfileId, rowNumber);
        return Ok(data);
    }
}