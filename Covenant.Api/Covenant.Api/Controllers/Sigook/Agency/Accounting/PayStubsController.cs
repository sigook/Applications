using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Constants;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Accounting;

[Route("api/agency/accounting/[controller]")]
[ApiController]
[ServiceFilter(typeof(AgencyIdFilter))]
public class PayStubsController : ControllerBase
{
    private readonly IAccountingService accountingService;
    private readonly ITimesheetRepository timeSheetRepository;
    private readonly ISkipPayrollNumberRepository skipPayrollNumberRepository;
    private readonly IPayStubService payStubService;
    private readonly IBulkPayStubEmailQueue bulkPayStubEmailQueue;

    public PayStubsController(
        IAccountingService accountingService,
        ITimesheetRepository timeSheetRepository,
        ISkipPayrollNumberRepository skipPayrollNumberRepository,
        IPayStubService payStubService,
        IBulkPayStubEmailQueue bulkPayStubEmailQueue)
    {
        this.accountingService = accountingService;
        this.timeSheetRepository = timeSheetRepository;
        this.skipPayrollNumberRepository = skipPayrollNumberRepository;
        this.payStubService = payStubService;
        this.bulkPayStubEmailQueue = bulkPayStubEmailQueue;
    }

    /// <summary>Gets a paginated list of pay stubs matching the given filter.</summary>
    /// <param name="filter">Filter and pagination criteria.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<PayStubListModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPayStubs([FromQuery] GetPayStubsFilter filter)
    {
        var data = await accountingService.GetPayStubs(filter);
        return Ok(data);
    }

    /// <summary>Exports the filtered pay stubs to an Excel file.</summary>
    /// <param name="filter">Filter and pagination criteria.</param>
    [HttpGet("file")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPayStubsFile([FromQuery] GetPayStubsFilter filter)
    {
        var file = await accountingService.GetPayStubsFile(filter);
        return File(file.Document, CovenantConstants.ExcelMime, file.DocumentName);
    }

    /// <summary>Generates pay stubs for the given workers.</summary>
    /// <param name="workerIds">Worker identifiers to generate pay stubs for.</param>
    [HttpPost("generate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GeneratePayStubs([FromBody] IEnumerable<Guid> workerIds)
    {
        var agencyIds = User.GetAgencyIds();
        var result = await payStubService.Generate(agencyIds, workerIds);
        if (result)
        {
            return Ok();
        }
        return BadRequest(ModelState.AddErrors(result.Errors));
    }

    /// <summary>Searches skipped payroll numbers by term.</summary>
    /// <param name="searchTerm">Term to search for.</param>
    [HttpGet("skip-payroll-number")]
    [ProducesResponseType(typeof(List<BaseModel<Guid>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSkipPayrollNumber([FromQuery] string searchTerm)
    {
        var result = await skipPayrollNumberRepository.Get(searchTerm);
        return Ok(result);
    }

    /// <summary>Creates a skipped payroll number entry.</summary>
    /// <param name="model">Skip payroll number data.</param>
    [HttpPost("skip-payroll-number")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSkipPayrollNumber([FromBody] BaseModel<Guid> model)
    {
        var result = await accountingService.CreateSkipPayrollNumber(model);
        if (result)
        {
            return Ok();
        }
        return BadRequest(ModelState.AddErrors(result.Errors));
    }

    /// <summary>Queues a bulk pay stub email job and notifies the accounting team on Teams when it finishes.</summary>
    /// <param name="model">The pay stubs to email.</param>
    [HttpPost("email/bulk")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendBulkEmail([FromBody] BulkPayStubEmailModel model)
    {
        if (model?.PayStubIds is null || !model.PayStubIds.Any())
        {
            return BadRequest(ModelState.AddError("At least one pay stub is required"));
        }
        var job = new BulkPayStubEmailJob(User.GetAgencyId(), User.GetNickname(), [.. model.PayStubIds]);
        await bulkPayStubEmailQueue.Enqueue(job);
        return Accepted();
    }

    /// <summary>Gets the workers that are ready to have pay stubs generated.</summary>
    [HttpGet("WorkersReadyForPayStub")]
    [ProducesResponseType(typeof(List<WorkerReadyForPayStubModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWorkersReadyForPayStub()
    {
        var data = await timeSheetRepository.GetWorkersReadyForPayStub(User.GetAgencyIds());
        return Ok(data);
    }
}