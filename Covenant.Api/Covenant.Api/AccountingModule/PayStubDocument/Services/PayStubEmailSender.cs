using Covenant.Api.AccountingModule.PayStubDocument.Controllers;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.HtmlTemplates.Views.Billing.Payroll;
using System.Net.Mime;

namespace Covenant.Api.AccountingModule.PayStubDocument.Services;

public record PayStubEmailDocument(string PdfPath, string FileName, PayrollEmailViewModel Model);

public record PayStubEmailResult(Guid PayStubId, bool Success, string WorkerFullName, string PayrollNumber)
{
    public static PayStubEmailResult Failed(Guid payStubId) => new(payStubId, false, string.Empty, string.Empty);
}

public interface IPayStubEmailSender
{
    Task<PayStubEmailResult> SendOne(Guid payStubId);
}

public class PayStubEmailSender : IPayStubEmailSender
{
    private const string PayStubEmailTemplatePath = "/Views/Billing/Payroll/PayrollEmail.cshtml";
    private readonly PayStubPdf _payStubPdf;
    private readonly IEmailService _emailService;

    public PayStubEmailSender(PayStubPdf payStubPdf, IEmailService emailService)
    {
        _payStubPdf = payStubPdf;
        _emailService = emailService;
    }

    public async Task<PayStubEmailResult> SendOne(Guid payStubId)
    {
        var document = await _payStubPdf.GetPdfAndModel(payStubId);
        if (document is null) return PayStubEmailResult.Failed(payStubId);

        var model = document.Model;
        var attachment = new EmailAttachment(document.FileName, MediaTypeNames.Application.Pdf, document.PdfPath);
        string body = await _payStubPdf.Renderer.RenderViewToStringAsync(PayStubEmailTemplatePath, model);
        var emailParams = new EmailParams(model.WorkerEmail, $"PayStub {model.PayrollNumber}", body)
        {
            Attachments = [attachment],
            EmailSettingName = EmailSettingName.PayrollCovenant
        };
        bool wasSent = await _emailService.SendCovenantEmail(emailParams);
        return new PayStubEmailResult(payStubId, wasSent, model.WorkerFullName, model.PayrollNumber);
    }
}
