using Covenant.Api.Authorization;
using Covenant.Common.Constants;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyRequestTimeSheet.Controllers;

[Route("api/AgencyRequest/{requestId}/TimeSheet")]
[ApiController]
[Authorize(Policy = PolicyConfiguration.Agency)]
public class AgencyRequestTimeSheetContoller : ControllerBase
{
    private readonly ITimesheetService timesheetService;

    public AgencyRequestTimeSheetContoller(ITimesheetService timesheetService)
    {
        this.timesheetService = timesheetService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(Guid requestId)
    {
        var result = await timesheetService.GetRequestTimesheetFile(requestId);
        return File(result.Document.ToArray(), CovenantConstants.ExcelMime, result.DocumentName);
    }
}
