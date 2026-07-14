using Covenant.Api.Authorization;
using Covenant.Common.Constants;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Requests;

[Route(RouteName)]
[ApiController]
[Authorize(Policy = PolicyConfiguration.Agency)]
public class TimeSheetsController(ITimesheetService timesheetService) : ControllerBase
{
    public const string RouteName = "api/agency/requests/{requestId}/TimeSheets";

    /// <summary>Generates and downloads an Excel timesheet report for a request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    [HttpGet("File")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFile(Guid requestId)
    {
        var result = await timesheetService.GetRequestTimesheetFile(requestId);
        return File(result.Document.ToArray(), CovenantConstants.ExcelMime, result.DocumentName);
    }
}
