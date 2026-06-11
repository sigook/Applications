using Covenant.Api.AccountingModule.Shared;
using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Notification;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Http;
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

    /// <summary>
    /// Gets the invoice summary by its identifier, using the agency's billing location to select the format.
    /// </summary>
    /// <param name="agencyRepository">Agency repository.</param>
    /// <param name="invoiceRepository">Invoice repository.</param>
    /// <param name="id">Identifier of the invoice.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(InvoiceSummaryModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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


    /// <summary>
    /// Deletes an invoice and its related pay stubs, and notifies the accounting team.
    /// </summary>
    /// <param name="options">Teams webhook configuration.</param>
    /// <param name="service">Teams notification service.</param>
    /// <param name="invoiceRepository">Invoice repository.</param>
    /// <param name="payStubRepository">Pay stub repository.</param>
    /// <param name="agencyRepository">Agency repository.</param>
    /// <param name="id">Identifier of the invoice to delete.</param>
    /// <param name="model">The pay stubs to delete alongside the invoice.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(
        [FromServices] IOptions<TeamsWebhookConfiguration> options,
        [FromServices] ITeamsService service,
        [FromServices] IInvoiceRepository invoiceRepository,
        [FromServices] IPayStubRepository payStubRepository,
        [FromServices] IAgencyRepository agencyRepository,
        [FromRoute] Guid id,
        [FromBody] DeleteInvoiceModel model)
    {
        (Guid InvoiceId, string InvoiceNumber) invoicesDeleted;
        IReadOnlyList<string> payStubsDeleted = [];
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
}