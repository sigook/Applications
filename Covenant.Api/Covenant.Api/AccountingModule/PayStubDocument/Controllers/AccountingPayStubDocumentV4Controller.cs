using Covenant.Api.AccountingModule.PayStubDocument.Services;
using Covenant.Api.AccountingModule.Shared;
using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Repositories.Agency;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Covenant.Api.AccountingModule.PayStubDocument.Controllers;

[Route(RouteName)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class AccountingPayStubDocumentV4Controller : AccountingBaseController
{
    public const string RouteName = "api/v4/Accounting/PayStub/{payStubId}/Document";

    /// <summary>
    /// Generates and returns the pay stub document as a PDF file.
    /// </summary>
    /// <param name="payStubPdf">Pay stub PDF generator.</param>
    /// <param name="agencyRepository">Agency repository.</param>
    /// <param name="payStubId">Identifier of the pay stub.</param>
    [HttpGet("PDF")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPdf(
        [FromServices] PayStubPdf payStubPdf,
        [FromServices] IAgencyRepository agencyRepository,
        Guid payStubId)
    {
        return await payStubPdf.GetPdf(payStubId, async (pdfPath, fileName) =>
        {
            await Task.CompletedTask;
            return PhysicalFile(pdfPath, MediaTypeNames.Application.Pdf, fileName);
        });
    }

    /// <summary>
    /// Generates the pay stub PDF and emails it to the worker.
    /// </summary>
    /// <param name="sender">Pay stub email sender.</param>
    /// <param name="payStubId">Identifier of the pay stub.</param>
    [HttpPost]
    [Route("Email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(
        [FromServices] IPayStubEmailSender sender,
        Guid payStubId)
    {
        var result = await sender.SendOne(payStubId);
        if (result.Success) return Ok();
        return BadRequest(ModelState.AddError("Email delivery failed please try again"));
    }
}