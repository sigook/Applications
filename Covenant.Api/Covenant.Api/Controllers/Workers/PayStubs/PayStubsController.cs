using Covenant.Api.Authorization;
using Covenant.Common.Models.Accounting.PayStubs;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Accounting.PayStubs;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces.Accounting.PayStubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Covenant.Api.Controllers.Workers.PayStubs;

[Route(RouteName)]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Worker)]
public class PayStubsController(IPayStubRepository payStubRepository, IPayStubService payStubService) : ControllerBase
{
    public const string RouteName = "api/worker/paystubs";

    /// <summary>Lists the pay stubs of the authenticated worker, newest week first.</summary>
    /// <param name="filter">Pagination and the optional year of the pay period end.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<WorkerPayStubListModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] GetWorkerPayStubsFilter filter) =>
        Ok(await payStubService.GetPayStubsForWorker(User.GetUserId(), filter));

    /// <summary>Lists the years that have pay stubs for the authenticated worker, newest first.</summary>
    [HttpGet("years")]
    [ProducesResponseType(typeof(List<int>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetYears() =>
        Ok(await payStubRepository.GetPayStubYearsForWorker(User.GetUserId()));

    /// <summary>Gets the year-to-date totals of the authenticated worker.</summary>
    /// <param name="year">Year of the pay period end.</param>
    [HttpGet("ytd/{year:int}")]
    [ProducesResponseType(typeof(PayStubYtdModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetYtd([FromRoute] int year) =>
        Ok(await payStubRepository.GetYtdSummaryForWorker(User.GetUserId(), year));

    /// <summary>Gets the PDF of one of the authenticated worker's pay stubs.</summary>
    /// <param name="payStubId">Identifier of the pay stub.</param>
    [HttpGet("{payStubId:guid}/pdf")]
    [Produces("application/octet-stream", "application/json")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPdf([FromRoute] Guid payStubId)
    {
        var file = await payStubService.GetPayStubPdfForWorker(User.GetUserId(), payStubId);
        if (file is null) return NotFound();
        return File(file.Content, MediaTypeNames.Application.Pdf, file.FileName);
    }
}
