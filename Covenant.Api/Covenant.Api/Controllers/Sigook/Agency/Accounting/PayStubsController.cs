using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Constants;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Accounting;

[Route("api/agency/accounting/[controller]")]
[ApiController]
[ServiceFilter(typeof(AgencyIdFilter))]
public class PayStubsController : ControllerBase
{
    private readonly IAccountingService accountingService;
    private readonly ITimeSheetRepository timeSheetRepository;
    private readonly ISkipPayrollNumberRepository skipPayrollNumberRepository;

    public PayStubsController(
        IAccountingService accountingService,
        ITimeSheetRepository timeSheetRepository,
        ISkipPayrollNumberRepository skipPayrollNumberRepository)
    {
        this.accountingService = accountingService;
        this.timeSheetRepository = timeSheetRepository;
        this.skipPayrollNumberRepository = skipPayrollNumberRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetPayStubs([FromQuery] GetPayStubsFilter filter)
    {
        var data = await accountingService.GetPayStubs(filter);
        return Ok(data);
    }

    [HttpGet("file")]
    public async Task<IActionResult> GetPayStubsFile([FromQuery] GetPayStubsFilter filter)
    {
        var file = await accountingService.GetPayStubsFile(filter);
        return File(file.Document, CovenantConstants.ExcelMime, file.DocumentName);
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GeneratePayStubs([FromBody] IEnumerable<Guid> workerIds)
    {
        var result = await accountingService.GeneratePayStubs(workerIds);
        if (result)
        {
            return Ok();
        }
        return BadRequest(ModelState.AddErrors(result.Errors));
    }

    [HttpGet("skip-payroll-number")]
    public async Task<IActionResult> GetSkipPayrollNumber([FromQuery] string searchTerm)
    {
        var result = await skipPayrollNumberRepository.Get(searchTerm);
        return Ok(result);
    }

    [HttpPost("skip-payroll-number")]
    public async Task<IActionResult> CreateSkipPayrollNumber([FromBody] BaseModel<Guid> model)
    {
        var result = await accountingService.CreateSkipPayrollNumber(model);
        if (result)
        {
            return Ok();
        }
        return BadRequest(ModelState.AddErrors(result.Errors));
    }

    [HttpGet("WorkersReadyForPayStub")]
    public async Task<IActionResult> GetWorkersReadyForPayStub()
    {
        var data = await timeSheetRepository.GetWorkersReadyForPayStub(User.GetAgencyIds());
        return Ok(data);
    }
}