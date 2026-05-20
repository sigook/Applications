using Covenant.Api.Authorization;
using Covenant.Common.Constants;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyRequestTimeSheet.Controllers;

[Route("api/AgencyRequest/{requestId}/TimeSheet")]
[ApiController]
[Authorize(Policy = PolicyConfiguration.Agency)]
public class AgencyRequestTimeSheetController : ControllerBase
{
    private readonly ITimesheetService timesheetService;

    public AgencyRequestTimeSheetController(ITimesheetService timesheetService)
    {
        this.timesheetService = timesheetService;
    }

    /// <summary>Generates and downloads an Excel timesheet report for a request.</summary>
    /// <param name="requestId">Identifier of the request.</param>
    [HttpGet]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid requestId)
    {
        var result = await timesheetService.GetRequestTimesheetFile(requestId);
        return File(result.Document.ToArray(), CovenantConstants.ExcelMime, result.DocumentName);
    }
}
