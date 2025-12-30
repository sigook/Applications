using Covenant.Api.AccountingModule.Shared;
using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Utils.Extensions;
using Covenant.HtmlTemplates.Views.Billing.Payroll;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Covenant.Api.AccountingModule.PayStubDocument.Controllers;

[Route(RouteName)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class AccountingPayStubDocumentV4Controller : AccountingBaseController
{
    public const string RouteName = "api/v4/Accounting/PayStub/{payStubId}/Document";
    private const string PayStubEmailTemplatePath = "/Views/Billing/Payroll/PayrollEmail.cshtml";

    [HttpGet("PDF")]
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

    [HttpPost]
    [Route("Email")]
    public async Task<IActionResult> Post(
        [FromServices] PayStubPdf payStubPdf,
        [FromServices] IAgencyRepository agencyRepository,
        [FromServices] IEmailService emailService,
        Guid payStubId)
    {
        async Task<IActionResult> Send(string path, string name, PayrollEmailViewModel model)
        {
            var emailAttachment = new EmailAttachment(name, MediaTypeNames.Application.Pdf, path);
            string emailBody = await payStubPdf.Renderer.RenderViewToStringAsync(PayStubEmailTemplatePath, model);
            var emailParams = new EmailParams(model.WorkerEmail, $"PayStub {model.PayrollNumber}", emailBody)
            {
                Attachments = new[] { emailAttachment },
                EmailSettingName = EmailSettingName.PayrollCovenant
            };
            bool wasSend = await emailService.SendEmail(emailParams);
            if (wasSend) return Ok();
            return BadRequest(ModelState.AddError("Email delivery failed please try again"));
        }
        Location billingLocation = await agencyRepository.GetBillingLocation(User.GetAgencyId());
        return await payStubPdf.GetPdf(payStubId, Send);
    }
}