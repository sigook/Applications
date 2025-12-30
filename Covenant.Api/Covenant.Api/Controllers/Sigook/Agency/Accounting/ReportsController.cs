using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Constants;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Repositories.Accounting;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Accounting;

[Route("api/agency/accounting/[controller]")]
[ApiController]
[Authorize]
[ServiceFilter(typeof(AgencyIdFilter))]
public class ReportsController : ControllerBase
{
    private readonly IAccountingService accountingService;
    private readonly ITimesheetService timeSheetService;
    private readonly IPayStubService payStubService;
    private readonly IPayStubRepository payStubRepository;

    public ReportsController(
        IAccountingService accountingService,
        ITimesheetService timeSheetService,
        IPayStubService payStubService,
        IPayStubRepository payStubRepository)
    {
        this.accountingService = accountingService;
        this.timeSheetService = timeSheetService;
        this.payStubService = payStubService;
        this.payStubRepository = payStubRepository;
    }

    [HttpGet("hours-worked")]
    public async Task<IActionResult> GetHoursWorked([FromQuery] HoursWorkedFilter filter)
    {
        var result = await timeSheetService.GetHoursWorked(filter);
        return Ok(result);
    }

    [HttpGet("hours-worked/file")]
    public async Task<IActionResult> GetHoursWorkedFile([FromQuery] HoursWorkedFilter filter)
    {
        var file = await timeSheetService.GetHoursWorkedFile(filter);
        return File(file.Document.ToArray(), CovenantConstants.ExcelMime, file.DocumentName);
    }

    [HttpGet("{companyId}/job-positions")]
    public async Task<IActionResult> GetJobPositions([FromRoute] Guid companyId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var model = await timeSheetService.GetJobPositions(companyId, startDate, endDate);
        if (model is null) return NotFound();
        return Ok(model);
    }

    [HttpGet("t4")]
    public async Task<IActionResult> GetT4([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        if (!startDate.HasValue || !endDate.HasValue)
        {
            return BadRequest("Dates are mandatory");
        }
        var file = await payStubService.GenerateT4(startDate.Value, endDate.Value);
        return File(file.Value.Document, CovenantConstants.ExcelMime, file.Value.DocumentName);
    }

    [HttpGet("payments")]
    public async Task<IActionResult> GetPayments([FromQuery] Pagination pagination)
    {
        var result = await accountingService.GetWeeklyPayrollGroupByPaymentDate(pagination);
        return Ok(result);
    }

    [HttpGet("payments/file")]
    public async Task<IActionResult> GetPaymentsFile([FromQuery] string weekEnding)
    {
        var file = await accountingService.GetWeeklyPayrollGroupByPaymentDateFile(weekEnding);
        if (file.IsSuccess)
        {
            return File(file.Value.Document, CovenantConstants.ExcelMime, file.Value.DocumentName);
        }
        return BadRequest(ModelState.AddErrors(file.Errors));
    }

    [HttpGet("subcontractors")]
    public async Task<IActionResult> GetSubcontractors([FromQuery] Pagination filter)
    {
        var result = await accountingService.GetSubcontractors(filter);
        return Ok(result);
    }

    [HttpGet("subcontractors/file")]
    public async Task<IActionResult> GetSubcontractorFile([FromQuery] string weekEnding)
    {
        var file = await accountingService.GetSubcontractorFile(weekEnding);
        if (file)
        {
            return File(file.Value.Document, CovenantConstants.ExcelMime, file.Value.DocumentName);
        }
        return BadRequest(ModelState.AddErrors(file.Errors));
    }
}
