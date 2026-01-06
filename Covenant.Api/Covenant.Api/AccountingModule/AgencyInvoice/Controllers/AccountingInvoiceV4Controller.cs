using Covenant.Api.AccountingModule.Shared;
using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Billing.Services;
using Covenant.Billing.Services.Impl;
using Covenant.Billing.Utils;
using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Notification;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Documents.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Covenant.Api.AccountingModule.AgencyInvoice.Controllers;

[Route(RouteName)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class AccountingInvoiceV4Controller : AccountingBaseController
{
    public const string RouteName = "api/v4/Accounting/Invoice";

    private readonly IInvoicesContainer invoicesContainer;
    private readonly IPayStubsContainer payStubsContainer;

    public AccountingInvoiceV4Controller(IInvoicesContainer invoicesContainer, IPayStubsContainer payStubsContainer)
    {
        this.invoicesContainer = invoicesContainer;
        this.payStubsContainer = payStubsContainer;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromServices] IAgencyRepository agencyRepository,
        [FromServices] IInvoiceRepository invoiceRepository,
        Guid id)
    {
        var billingLocation = await agencyRepository.GetBillingLocation(User.GetAgencyId());
        var invoice = billingLocation?.IsUSA == true
            ? await invoiceRepository.GetInvoiceUSASummaryById(id)
            : await invoiceRepository.GetInvoiceSummaryById(id);
        if (invoice == null) return NotFound();
        return Ok(invoice);
    }

    [HttpPost("Preview")]
    public async Task<IActionResult> Preview(
        [FromServices] IAgencyRepository agencyRepository,
        [FromServices] ITimeSheetRepository timeSheetRepository,
        [FromServices] ICreateInvoiceWithoutTimeSheet createInvoiceWithoutTimeSheet,
        [FromServices] ICreateInvoiceUsingTimeSheet createInvoiceUsingTimeSheet,
        [FromServices] CreateInvoiceUSA createInvoice,
        [FromBody] CreateInvoiceModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        InvoicePreviewModel preview;
        var agencyIds = new List<Guid> { User.GetAgencyId() };
        var billingLocation = await agencyRepository.GetBillingLocation(User.GetAgencyId());
        var timeSheet = await timeSheetRepository.GetTimeSheetForCreatingInvoice(agencyIds, model);
        if (billingLocation?.IsUSA == true)
        {
            var result = await createInvoice.Preview(User.GetAgencyId(), model.CompanyProfileId, timeSheet, model);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            preview = result.Value.ToInvoicePreview();
        }
        else
        {
            Invoice invoice;
            if (timeSheet.Any())
            {
                var result = await createInvoiceUsingTimeSheet.Preview(User.GetAgencyId(), model.CompanyProfileId, timeSheet, model);
                if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
                invoice = result.Value;
            }
            else
            {
                var result = await createInvoiceWithoutTimeSheet.Preview(model.CompanyProfileId, model);
                if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
                invoice = result.Value;
            }
            preview = invoice.ToInvoicePreview(timeSheet);
        }
        return Ok(preview);
    }

    [HttpPost]
    public async Task<IActionResult> Post(
        [FromServices] AccountingCreateInvoiceAndReportsSubcontractor andReportsSubcontractor,
        [FromServices] IAgencyRepository agencyRepository,
        [FromServices] ITimeSheetRepository timeSheetRepository,
        [FromServices] CreateInvoiceUSA createInvoice,
        [FromBody] CreateInvoiceModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        Guid id;
        var billingLocation = await agencyRepository.GetBillingLocation(User.GetAgencyId());
        if (billingLocation?.IsUSA == true)
        {
            var agencyIds = new List<Guid> { User.GetAgencyId() };
            var timeSheet = await timeSheetRepository.GetTimeSheetForCreatingInvoice(agencyIds, model);
            var result = await createInvoice.Create(User.GetAgencyId(), model.CompanyProfileId, timeSheet, model);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            id = result.Value.Id;
        }
        else
        {
            var result = await andReportsSubcontractor.Create(model, User.GetAgencyId());
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            id = result.Value.Id;
        }
        return CreatedAtRoute(new { id }, new { });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromServices] IOptions<TeamsWebhookConfiguration> options,
        [FromServices] ITeamsService service,
        [FromServices] IInvoiceRepository invoiceRepository,
        [FromServices] IPayStubRepository payStubRepository,
        [FromServices] IAgencyRepository agencyRepository,
        [FromRoute] Guid id,
        [FromBody] DeleteInvoiceModel model)
    {
        if (!Invoice.VerificationCode(id).Equals(model?.VerificationCode, StringComparison.InvariantCultureIgnoreCase))
        {
            return BadRequest("Invalid Verification Code");
        }
        (Guid InvoiceId, string InvoiceNumber) invoicesDeleted;
        IReadOnlyList<string> payStubsDeleted = Array.Empty<string>();
        Location billingLocation = await agencyRepository.GetBillingLocation(User.GetAgencyId());
        if (billingLocation?.IsUSA == true)
        {
            invoicesDeleted = await invoiceRepository.DeleteInvoiceUSA(id);
        }
        else
        {
            invoicesDeleted = await invoiceRepository.DeleteInvoiceAndReportsSubcontractor(id);
            if (model?.PayStubs != null && model.PayStubs.Any())
            {
                payStubsDeleted = await payStubRepository.Delete(model.PayStubs);
            }
        }
        await invoiceRepository.SaveChangesAsync();
        await invoicesContainer.DeleteFileIfExists(invoicesDeleted.InvoiceId.ToInvoiceBlobName());
        await payStubsContainer.DeleteFilesIfExists(model.PayStubs?.Select(p => p.ToPayStubBlobName()));
        string text = $"{invoicesDeleted.InvoiceNumber} {(payStubsDeleted.Any() ? " - " : string.Empty)}{string.Join(" - ", payStubsDeleted)}";
        string name = User.GetNickname();
        var configuration = options.Value;
        await service.SendNotification(configuration.Accounting, TeamsNotificationModel.CreateWarning($"Invoice deleted by {name}", text));
        return Ok();
    }

    [HttpPost("{id:guid}/SendVerificationCode")]
    public async Task<IActionResult> SendVerificationCode([FromServices] IOptions<TeamsWebhookConfiguration> options, [FromServices] ITeamsService service, Guid id)
    {
        var configuration = options.Value;
        Result result = await service.SendNotification(configuration.Accounting, TeamsNotificationModel.CreateSuccess("Verification Code", Invoice.VerificationCode(id)));
        if (result) return Ok();
        return BadRequest(ModelState.AddErrors(result.Errors));
    }
}