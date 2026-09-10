using Covenant.Api.Authorization;
using Covenant.Common.Constants;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Requests;

[ApiController]
[Authorize(Policy = PolicyConfiguration.Agency)]
[Route(RouteName)]
public class WorkersReportController(IRequestService requestService) : ControllerBase
{
    public const string RouteName = "api/agency/requests/{requestId:guid}/WorkersReport";

    /// <summary>Generates the workers report for a request as an Excel file.</summary>
    /// <param name="requestId">Request identifier.</param>
    [HttpGet]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromRoute] Guid requestId)
    {
        var file = await requestService.GetWorkersReportFile(requestId);
        return File(file.Document.ToArray(), CovenantConstants.ExcelMime, file.DocumentName);
    }
}
