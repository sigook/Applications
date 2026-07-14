using Covenant.Api.Authorization;
using Covenant.Common.Constants;
using Covenant.Common.Models.Request;
using Covenant.Core.BL.Interfaces;
using Covenant.Documents.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Sales;

[Route("api/agency/sales/[controller]")]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Sales)]
[ServiceFilter(typeof(AgencyIdFilter))]
[ServiceFilter(typeof(AgencyPersonnelIdFilter))]
public class RequestsController(IMediator mediator, ISalesService salesService) : ControllerBase
{
    /// <summary>Gets a paginated list of requests for the sales module. Sales users only see requests where they are the assigned sales representative.</summary>
    /// <param name="pagination">Request filter and pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(AgencyRequestsPagedResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(GetRequestForAgencyFilter pagination) =>
        Ok(await salesService.GetRequests(pagination));

    /// <summary>Generates and downloads an Excel report of the sales module requests.</summary>
    /// <param name="pagination">Request filter parameters.</param>
    [HttpGet("File")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFile(GetRequestForAgencyFilter pagination)
    {
        var data = salesService.GetRequestsForReport(pagination).ToList();
        var file = await mediator.Send(new GenerateAgencyRequestsReport(data));
        return File(file.Document.ToArray(), CovenantConstants.ExcelMime, file.DocumentName);
    }
}
