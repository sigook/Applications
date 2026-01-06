using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Constants;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Accounting;

[Route("api/agency/accounting/[controller]")]
[ApiController]
[ServiceFilter(typeof(AgencyIdFilter))]
public class InvoicesController : ControllerBase
{
    private readonly IAccountingService accountingService;

    public InvoicesController(IAccountingService accountingService)
    {
        this.accountingService = accountingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetInvoices([FromQuery] GetInvoicesFilterV2 filter)
    {
        var data = await accountingService.GetInvoices(filter);
        return Ok(data);
    }

    [HttpGet("file")]
    public async Task<IActionResult> GetInvoicesFile([FromQuery] GetInvoicesFilterV2 filter)
    {
        var file = await accountingService.GetInvoicesFile(filter);
        return File(file.Document, CovenantConstants.ExcelMime, file.DocumentName);
    }

    [HttpPost("Preview")]
    public async Task<IActionResult> Preview([FromBody] CreateInvoiceModel model)
    {
        var result = await accountingService.PreviewInvoice(model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateInvoiceModel model)
    {
        var result = await accountingService.CreateInvoice(model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }
}
