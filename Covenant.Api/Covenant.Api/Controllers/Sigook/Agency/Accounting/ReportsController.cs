using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Constants;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Accounting.Subcontractor;
using Covenant.Common.Models.Company;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Accounting;

[Route("api/agency/accounting/[controller]")]
[ApiController]
[Authorize]
[ServiceFilter(typeof(AgencyIdFilter))]
public class ReportsController(
    IAccountingService accountingService,
    ITimesheetService timeSheetService,
    IPayStubService payStubService) : ControllerBase
{
    private readonly IAccountingService accountingService = accountingService;
    private readonly ITimesheetService timeSheetService = timeSheetService;
    private readonly IPayStubService payStubService = payStubService;

    /// <summary>Gets the hours-worked report summary for the given filter.</summary>
    /// <param name="filter">Filter criteria for the report.</param>
    [HttpGet("hours-worked")]
    [ProducesResponseType(typeof(HoursWorkedResume), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHoursWorked([FromQuery] HoursWorkedFilter filter)
    {
        var result = await timeSheetService.GetHoursWorked(filter);
        return Ok(result);
    }

    /// <summary>Exports the hours-worked report to an Excel file.</summary>
    /// <param name="filter">Filter criteria for the report.</param>
    [HttpGet("hours-worked/file")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHoursWorkedFile([FromQuery] HoursWorkedFilter filter)
    {
        var file = await timeSheetService.GetHoursWorkedFile(filter);
        return File(file.Document.ToArray(), CovenantConstants.ExcelMime, file.DocumentName);
    }

    /// <summary>Exports the timesheets report to an Excel file for the given date range (USA agencies).</summary>
    /// <param name="filter">Filter criteria for the report.</param>
    [HttpGet("timesheets/file")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTimesheetsReportFile([FromQuery] TimesheetsReportFilter filter)
    {
        var file = await timeSheetService.GetTimesheetsReportFile(filter);
        return File(file.Document.ToArray(), CovenantConstants.ExcelMime, file.DocumentName);
    }

    /// <summary>Gets the job positions worked for a company within a date range.</summary>
    /// <param name="companyProfileId">Company profile identifier.</param>
    /// <param name="startDate">Start of the date range.</param>
    /// <param name="endDate">End of the date range.</param>
    [HttpGet("{companyProfileId}/job-positions")]
    [ProducesResponseType(typeof(IEnumerable<CompanyProfileJobPositionRateModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetJobPositions([FromRoute] Guid companyProfileId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var model = await timeSheetService.GetJobPositions(companyProfileId, startDate, endDate);
        if (model is null) return NotFound();
        return Ok(model);
    }

    /// <summary>Generates the T4 tax report as an Excel file for the given date range.</summary>
    /// <param name="startDate">Start of the date range.</param>
    /// <param name="endDate">End of the date range.</param>
    [HttpGet("t4")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetT4([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        if (!startDate.HasValue || !endDate.HasValue)
        {
            return BadRequest("Dates are mandatory");
        }
        var file = await payStubService.GenerateT4(startDate.Value, endDate.Value);
        if (file.IsSuccess)
        {
            return File(file.Value.Document, CovenantConstants.ExcelMime, file.Value.DocumentName);
        }
        return BadRequest(ModelState.AddErrors(file.Errors));
    }

    /// <summary>Generates the CRA payroll report as an Excel file for the given date range.</summary>
    /// <param name="startDate">Start of the date range.</param>
    /// <param name="endDate">End of the date range.</param>
    [HttpGet("cra-payroll")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCraPayroll([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        if (!startDate.HasValue || !endDate.HasValue)
        {
            return BadRequest("Dates are mandatory");
        }
        var file = await payStubService.GenerateCraPayroll(startDate.Value, endDate.Value);
        if (file.IsSuccess)
        {
            return File(file.Value.Document, CovenantConstants.ExcelMime, file.Value.DocumentName);
        }
        return BadRequest(ModelState.AddErrors(file.Errors));
    }

    /// <summary>Gets the weekly payroll grouped by payment date.</summary>
    /// <param name="pagination">Pagination criteria.</param>
    [HttpGet("payments")]
    [ProducesResponseType(typeof(PaginatedList<WeeklyPayrollModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPayments([FromQuery] Pagination pagination)
    {
        var result = await accountingService.GetWeeklyPayrollGroupByPaymentDate(pagination);
        return Ok(result);
    }

    /// <summary>Exports the weekly payroll for a given week-ending date to an Excel file.</summary>
    /// <param name="weekEnding">Week-ending date.</param>
    [HttpGet("payments/file")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPaymentsFile([FromQuery] string weekEnding)
    {
        var file = await accountingService.GetWeeklyPayrollGroupByPaymentDateFile(weekEnding);
        if (file.IsSuccess)
        {
            return File(file.Value.Document, CovenantConstants.ExcelMime, file.Value.DocumentName);
        }
        return BadRequest(ModelState.AddErrors(file.Errors));
    }

    /// <summary>Gets the paginated list of subcontractors.</summary>
    /// <param name="filter">Pagination criteria.</param>
    [HttpGet("subcontractors")]
    [ProducesResponseType(typeof(PaginatedList<PayrollSubContractorListModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubcontractors([FromQuery] Pagination filter)
    {
        var result = await accountingService.GetSubcontractors(filter);
        return Ok(result);
    }

    /// <summary>Exports the subcontractor report for a given week-ending date to an Excel file.</summary>
    /// <param name="weekEnding">Week-ending date.</param>
    [HttpGet("subcontractors/file")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
