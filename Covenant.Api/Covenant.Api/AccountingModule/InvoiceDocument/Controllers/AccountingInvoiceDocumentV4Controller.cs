using Covenant.Api.AccountingModule.InvoiceDocument.Models;
using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Entities;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Utils.Extensions;
using Covenant.HtmlTemplates.Views.Billing.Invoice;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Net.Mime;

namespace Covenant.Api.AccountingModule.InvoiceDocument.Controllers
{
    [Route(RouteName)]
    [ServiceFilter(typeof(AgencyIdFilter))]
    [ApiController]
    public class AccountingInvoiceDocumentV4Controller : ControllerBase
    {
        public const string RouteName = "api/v4/Accounting/Invoice/{invoiceId}/Document";

        private readonly InvoicePdf invoicePdf;
        private readonly IRazorViewToStringRenderer renderer;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IAgencyRepository _agencyRepository;
        private readonly IEmailService emailService;

        public AccountingInvoiceDocumentV4Controller(InvoicePdf invoicePdf,
            IRazorViewToStringRenderer renderer,
            IInvoiceRepository invoiceRepository,
            IAgencyRepository agencyRepository,
            IEmailService emailService)
        {
            this.invoicePdf = invoicePdf;
            this.renderer = renderer;
            _invoiceRepository = invoiceRepository;
            _agencyRepository = agencyRepository;
            this.emailService = emailService;
        }

        [HttpGet]
        [Route("PDF")]
        public async Task<IActionResult> GetPdf(Guid invoiceId)
        {
            Location billingLocation = await _agencyRepository.GetBillingLocation(User.GetAgencyId());
            var model = billingLocation?.IsUSA == true ?
                await _invoiceRepository.GetInvoiceUSASummaryById(invoiceId) :
                await _invoiceRepository.GetInvoiceSummaryById(invoiceId);
            string pdfPath = await invoicePdf.GetPdf(invoiceId, () => Task.FromResult(model));
            if (string.IsNullOrEmpty(pdfPath)) return NotFound();
            return PhysicalFile(pdfPath, MediaTypeNames.Application.Pdf, invoiceId.ToInvoiceBlobName());
        }

        [HttpPost]
        [Route("Email")]
        public async Task<IActionResult> Post(Guid invoiceId, [FromForm] InvoiceEmailModel model)
        {
            Location billingLocation = await _agencyRepository.GetBillingLocation(User.GetAgencyId());
            var invoiceSummary = billingLocation?.IsUSA == true ?
                await _invoiceRepository.GetInvoiceUSASummaryById(invoiceId) :
                await _invoiceRepository.GetInvoiceSummaryById(invoiceId);
            if (invoiceSummary != null)
            {
                var invoiceModel = invoiceSummary;
                string path = await invoicePdf.GetPdf(invoiceModel.Id, () => Task.FromResult(invoiceModel));
                if (string.IsNullOrEmpty(path)) return BadRequest();
                var emailAttachments = new List<EmailAttachment>();
                foreach (var file in model.Files)
                {
                    var contentType = file.ContentType;
                    if (string.IsNullOrEmpty(contentType))
                    {
                        var fileProvider = new FileExtensionContentTypeProvider();
                        if (fileProvider.TryGetContentType(file.FileName, out string result))
                        {
                            contentType = result;
                        }
                    }
                    using var reader = new BinaryReader(file.OpenReadStream());
                    var data = reader.ReadBytes((int)file.OpenReadStream().Length);
                    emailAttachments.Add(new EmailAttachment(file.FileName, contentType, new MemoryStream(data)));
                }
                var invoiceStream = new MemoryStream(System.IO.File.ReadAllBytes(path));
                emailAttachments.Add(new EmailAttachment($"Invoice {invoiceModel.InvoiceNumber}.pdf", MediaTypeNames.Application.Pdf, invoiceStream));
                string emailMessage = await renderer.RenderViewToStringAsync("/Views/Billing/Invoice/InvoiceEmail.cshtml", new InvoiceEmailViewModel
                {
                    AgencyAddress = invoiceModel.AgencyAddress,
                    AgencyPhone = invoiceModel.AgencyPhone,
                    AgencyFax = invoiceModel.AgencyFax,
                    AgencyWebSite = invoiceModel.AgencyWebSite,
                    AgencyLogoFileName = invoiceModel.AgencyLogoFileName,
                    InvoicePayroll = invoiceModel.InvoicePayroll,
                    Message = model.Message
                });
                bool wasSend = await emailService.SendEmail(new EmailParams(invoiceModel.Email, model.Subject, emailMessage)
                {
                    Cc = model.Cc ?? new List<string>(),
                    EmailSettingName = invoiceModel.InvoicePayroll.EmailSettingName,
                    Attachments = emailAttachments
                });
                if (wasSend) return Ok();
                return BadRequest(ModelState.AddError("Email delivery failed please try again"));
            }
            return BadRequest();
        }
    }
}