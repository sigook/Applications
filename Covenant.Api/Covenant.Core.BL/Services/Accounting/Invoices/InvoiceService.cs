using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.Invoices;
using Covenant.Common.Entities.Accounting.Subcontractors;
using Covenant.Common.Entities.Requests;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces.Adapters;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Accounting.Invoices;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Notifications;
using Covenant.Common.Models.Requests.TimeSheets;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Accounting.Invoices;
using Covenant.Common.Repositories.Accounting.PayStubs;
using Covenant.Common.Repositories.Accounting.Subcontractors;
using Covenant.Common.Repositories.Companies;
using Covenant.Common.Repositories.Requests;
using Covenant.Common.Repositories;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces.Accounting.Invoices;
using Covenant.Core.BL.Interfaces.Accounting;
using Covenant.Documents.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Mime;

namespace Covenant.Core.BL.Services.Accounting.Invoices;

public class InvoiceService(
    ITimesheetRepository timeSheetRepository,
    IInvoiceRepository invoiceRepository,
    ICompanyRepository companyRepository,
    ICatalogRepository catalogRepository,
    IPayStubRepository payStubRepository,
    TimeProvider timeProvider,
    Rates rates,
    ISubcontractorRepository subcontractorRepository,
    TimeLimits timeLimits,
    ITimesheetCalculatorService calculatorService,
    IInvoiceCountryStrategyResolver strategies,
    ICurrentUserService currentUserService,
    IInvoicesContainer invoicesContainer,
    IRazorViewToStringRenderer renderer,
    IEmailService emailService,
    IMediator mediator,
    IPayStubsContainer payStubsContainer,
    ITeamsService teamsService,
    IOptions<TeamsWebhookConfiguration> teamsOptions,
    IInvoiceDocumentAdapter invoiceDocumentAdapter,
    IValidator<ChangeInvoiceStatusModel> changeStatusValidator,
    ILogger<InvoiceService> logger) : IInvoiceService
{
    private const string InvoiceEmailTemplate = "/Views/Billing/Invoice/InvoiceEmail.cshtml";
    private const int MaxInvoiceNumbersInNotification = 20;

    private readonly TeamsWebhookConfiguration teamsConfiguration = teamsOptions.Value;

    private sealed record InvoiceDraft(Invoice Invoice, IInvoiceCountryStrategy Strategy);

    public async Task<InvoiceListModelWithTotals> GetInvoices(GetInvoicesFilter filter)
    {
        var result = await invoiceRepository.GetInvoicesForAgency(currentUserService.GetAgencyIds(), filter);
        SetDueDays(result.Detail.Items);
        return result;
    }

    public async Task<ResultGenerateDocument<byte[]>> GetInvoicesFile(GetInvoicesFilter filter)
    {
        var result = await GetAllInvoices(filter);
        return await mediator.Send(new GenerateInvoicesReport(result));
    }

    public async Task<ResultGenerateDocument<byte[]>> GetInvoicesPaymentsReport(GetInvoicesFilter filter)
    {
        var result = await GetAllInvoices(filter);
        return await mediator.Send(new GenerateInvoicesPaymentsReport(result));
    }

    public async Task<int> MarkOverdueInvoices()
    {
        var invoiceNumbers = await invoiceRepository.MarkPendingInvoicesAsDue(timeProvider.GetLocalNow().DateTime);
        if (invoiceNumbers.Count > 0)
        {
            var result = await teamsService.SendNotification(teamsConfiguration.Accounting, TeamsNotificationModel.CreateWarning("Invoices marked as Due", BuildDueNotificationText(invoiceNumbers)));
            if (result.IsFailure)
            {
                logger.LogWarning("Teams notification for {Count} invoices marked as Due failed: {Error}", invoiceNumbers.Count, result.StringErrors);
            }
        }
        return invoiceNumbers.Count;
    }

    private static string BuildDueNotificationText(IReadOnlyList<string> invoiceNumbers)
    {
        if (invoiceNumbers.Count == 1) return $"Invoice {invoiceNumbers[0]} is Due";
        string listed = string.Join(", ", invoiceNumbers.Take(MaxInvoiceNumbersInNotification));
        int remaining = invoiceNumbers.Count - MaxInvoiceNumbersInNotification;
        return remaining > 0
            ? $"{invoiceNumbers.Count} invoices are Due: {listed} and {remaining} more"
            : $"Invoices {listed} are Due";
    }

    private async Task<List<InvoiceListModel>> GetAllInvoices(GetInvoicesFilter filter)
    {
        var result = await invoiceRepository.GetAllInvoicesForAgency(currentUserService.GetAgencyIds(), filter);
        SetDueDays(result);
        return result;
    }

    private void SetDueDays(IEnumerable<InvoiceListModel> invoices)
    {
        var today = timeProvider.GetLocalNow().DateTime;
        foreach (var invoice in invoices) invoice.SetDueDays(today);
    }

    public async Task<Result<InvoicePreviewModel>> PreviewInvoice(CreateInvoiceModel model)
    {
        var result = await BuildInvoice(currentUserService.GetAgencyIds(), model);
        if (!result) return Result.Fail<InvoicePreviewModel>(result.Errors);

        var (invoice, strategy) = result.Value;
        var items = ConsolidatePreviewItems(invoice.InvoiceTotals)
            .Concat(ConsolidatePreviewAdditionalItems(invoice.AdditionalItems))
            .Concat(ConsolidatePreviewHolidays(invoice.Holidays))
            .ToList();

        return Result.Ok(new InvoicePreviewModel
        {
            SubTotal = invoice.SubTotal,
            Tax = invoice.Tax,
            TaxName = strategy.TaxName,
            Total = invoice.TotalNet,
            Items = items,
            Discounts = ConsolidatePreviewDiscounts(invoice.Discounts)
        });
    }

    public async Task<Result<Guid>> CreateInvoice(CreateInvoiceModel model)
    {
        var agencyIds = currentUserService.GetAgencyIds();
        var result = await BuildInvoice(agencyIds, model);
        if (!result) return Result.Fail<Guid>(result.Errors);

        var invoice = result.Value.Invoice;
        await invoiceRepository.Create(invoice);
        await invoiceRepository.SaveChangesAsync();
        await CreateSubcontractorReportsAsync(agencyIds, model.CompanyProfileId);

        return Result.Ok(invoice.Id);
    }

    public async Task<Result> ChangeInvoiceStatus(Guid invoiceId, ChangeInvoiceStatusModel model)
    {
        var validationResult = await changeStatusValidator.ValidateAsync(model);
        if (!validationResult.IsValid) return validationResult.ToResultFailure();

        var invoice = await invoiceRepository.GetInvoiceForAgency(currentUserService.GetAgencyIds(), invoiceId);
        if (invoice is null) return Result.Fail("Invoice not found");

        var result = invoice.ChangeStatus(model.Status, currentUserService.GetUserId(), timeProvider.GetLocalNow().DateTime, model.PaidAt);
        if (!result) return result;
        await invoiceRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<InvoiceDocument> GetInvoicePdf(Guid invoiceId)
    {
        var model = await invoiceRepository.GetInvoiceSummaryById(invoiceId);
        if (model is null) return null;
        (await strategies.ResolveForAgency(model.AgencyId)).Decorate(model);

        string fileName = invoiceId.ToInvoiceBlobName();
        byte[] content = await invoicesContainer.Download(fileName);
        if (content is not { Length: > 0 }) content = await UploadInvoicePdf(model);
        if (content is not { Length: > 0 }) return null;

        return new InvoiceDocument(content, fileName, model);
    }

    public async Task<Result> SendInvoiceEmail(Guid invoiceId, InvoiceEmailModel model)
    {
        var document = await GetInvoicePdf(invoiceId);
        if (document is null) return Result.Fail("Invoice not found");

        var invoice = document.Model;
        var emailAttachments = model.Attachments ?? [];
        var invoiceStream = new MemoryStream(document.Content);
        emailAttachments.Add(new EmailAttachment($"Invoice {invoice.InvoiceNumber}.pdf", MediaTypeNames.Application.Pdf, invoiceStream));

        string body = await renderer.RenderViewToStringAsync(InvoiceEmailTemplate, invoiceDocumentAdapter.MapToInvoiceEmailViewModel(invoice, model.Message));
        bool wasSent = await emailService.SendCovenantEmail(new EmailParams(invoice.Email, model.Subject, body)
        {
            Cc = model.Cc?.ToList() ?? [],
            EmailSettingName = invoice.InvoicePayroll.EmailSettingName,
            Attachments = emailAttachments
        });
        return wasSent ? Result.Ok() : Result.Fail("Email delivery failed please try again");
    }

    public async Task DeleteInvoice(Guid invoiceId, DeleteInvoiceModel model)
    {
        var (deletedInvoiceId, invoiceNumber) = await invoiceRepository.DeleteInvoiceAndReportsSubcontractor(invoiceId);
        IReadOnlyList<string> payStubsDeleted = [];
        if (model?.PayStubs != null && model.PayStubs.Any())
        {
            payStubsDeleted = await payStubRepository.DeletePayStubs(model.PayStubs);
        }
        await invoiceRepository.SaveChangesAsync();
        await invoicesContainer.DeleteFileIfExists(deletedInvoiceId.ToInvoiceBlobName());
        await payStubsContainer.DeleteFilesIfExists(model?.PayStubs?.Select(p => p.ToPayStubBlobName()));

        string text = $"{invoiceNumber} {(payStubsDeleted.Any() ? " - " : string.Empty)}{string.Join(" - ", payStubsDeleted)}";
        string name = currentUserService.GetNickname();
        await teamsService.SendNotification(teamsConfiguration.Accounting, TeamsNotificationModel.CreateWarning($"Invoice deleted by {name}", text));
    }

    private async Task<Result<InvoiceDraft>> BuildInvoice(IEnumerable<Guid> agencyIds, CreateInvoiceModel model)
    {
        var companyProfile = await companyRepository.GetCompanyProfile(cp => cp.Id == model.CompanyProfileId);
        if (companyProfile is null) return Result.Fail<InvoiceDraft>("Company not found");
        var strategy = await strategies.ResolveForAgency(companyProfile.AgencyId);

        var timesheets = await timeSheetRepository.GetTimeSheetForCreatingInvoice(agencyIds, model);
        if (timesheets.Count == 0 && !model.DirectHiring)
        {
            return Result.Fail<InvoiceDraft>("No approved timesheets found for the selected period");
        }

        var holidays = new List<DateTime>();
        if (timesheets.Count != 0)
        {
            var from = timesheets.Min(ts => ts.Date);
            var to = timesheets.Max(ts => ts.Date);
            holidays.AddRange(await GetHolidaysForPeriod(from, to, timesheets[0].CountryCode));
        }

        var invoiceTotals = new List<InvoiceTotal>();
        var invoiceHolidays = new List<InvoiceHoliday>();
        var timesheetsTax = 0m;
        if (!model.DirectHiring)
        {
            invoiceTotals.AddRange(ProcessTimesheets(timesheets, holidays, out timesheetsTax));
            if (strategy.BillsUnworkedHolidays)
            {
                invoiceHolidays = await GetInvoiceHolidaysAsync(timesheets, holidays, model.CompanyProfileId);
            }
        }

        var additionalItems = (model.AdditionalItems ?? [])
            .Select(a => new InvoiceAdditionalItem(a.Quantity, a.UnitPrice, a.Description))
            .ToList();
        var discounts = (model.Discounts ?? [])
            .Select(d => new InvoiceDiscount(d.Quantity, d.UnitPrice, d.Description))
            .ToList();

        var subtotal = invoiceTotals.Sum(t => t.TotalGross)
            + additionalItems.Sum(a => a.Total)
            + invoiceHolidays.Sum(h => h.Amount)
            - discounts.Sum(d => d.Amount);
        var tax = strategy.CalculateTax(subtotal, timesheetsTax, model);

        var createdAt = model.InvoiceDate ?? timeProvider.GetLocalNow().DateTime;
        var nextNumber = await invoiceRepository.GetNextInvoiceNumber(companyProfile.AgencyId);

        var invoice = Invoice.Create(
            model.CompanyProfileId,
            nextNumber.NextNumber,
            strategy.BuildInvoiceNumber(nextNumber.NextNumber, createdAt),
            model.Email,
            subtotal,
            tax,
            createdAt,
            model.DirectHiring || timesheets.Count == 0 ? null : timesheets.Max(t => t.Date).GetWeekEndingCurrentWeek());

        invoice.AddInvoiceTotals(invoiceTotals);
        invoice.AddHolidays(invoiceHolidays);
        invoice.AddAdditionalItems(additionalItems);
        invoice.AddDiscounts(discounts);
        invoice.SetAdditionalDetail(model.ClientSiteAddress);

        return Result.Ok(new InvoiceDraft(invoice, strategy));
    }

    private async Task<byte[]> UploadInvoicePdf(InvoiceSummaryModel model)
    {
        string fileName = model.Id.ToInvoiceBlobName();
        byte[] pdf = await mediator.Send(new GenerateInvoicePdf(invoiceDocumentAdapter.MapToInvoiceViewModel(model)));
        try
        {
            await invoicesContainer.UploadAsync(pdf, fileName, MediaTypeNames.Application.Pdf, new Dictionary<string, string>
            {
                {nameof(model.InvoiceNumber), model.InvoiceNumber.Trim()}
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return pdf;
    }

    private async Task<List<InvoiceHoliday>> GetInvoiceHolidaysAsync(
        IEnumerable<TimeSheetApprovedBillingModel> timesheets,
        List<DateTime> holidays,
        Guid companyProfileId)
    {
        var invoiceHolidays = new List<InvoiceHoliday>();
        var paidHolidays = timesheets.FirstOrDefault()?.PaidHolidays ?? false;
        if (!paidHolidays || holidays.Count == 0)
        {
            return invoiceHolidays;
        }

        foreach (var holiday in holidays)
        {
            var holidayWeekEnd = holiday.GetEnd();
            var lookbackStart = holidayWeekEnd.GetStart();
            var qualifyingDays = holiday.GetRangeOfDaysWorkerMustWorkToReceiveHolidayPay();
            var workersCharges = await invoiceRepository.GetCompanyRegularCharges(companyProfileId, holiday, lookbackStart, holidayWeekEnd, qualifyingDays);

            foreach (var workerCharge in workersCharges)
            {
                var result = InvoiceHoliday.Create(holiday, workerCharge.HoursToPay, workerCharge.AmountToPay, workerCharge.WorkerProfileId);
                if (result)
                {
                    invoiceHolidays.Add(result.Value);
                }
            }
        }

        return invoiceHolidays;
    }

    private bool IsPublicHoliday(DateTime date, List<DateTime> holidays) => holidays.Any(h => h.Date == date.Date);

    private TimeSheetHoursBreakdown CalculateHoursForItem(ITimeSheetHoursCalculable item, ref TimeSpan accumulatedRegularHours, bool isHoliday)
    {
        if (!item.TimeOutApproved.HasValue || !item.TimeInApproved.HasValue)
        {
            return new TimeSheetHoursBreakdown();
        }

        return calculatorService.CalculateHoursBreakdown(
            item.TimeInApproved.Value,
            item.TimeOutApproved.Value,
            item.DurationBreak,
            item.BreakIsPaid,
            isHoliday: isHoliday,
            holidayIsPaid: true,
            ref accumulatedRegularHours,
            item.OvertimeStartsAfter,
            timeLimits.MaxHoursWeek);
    }

    private async Task<List<DateTime>> GetHolidaysForPeriod(DateTime? from, DateTime? to, string countryCode)
    {
        if (!from.HasValue || !to.HasValue)
            return new List<DateTime>();

        var holidays = new List<DateTime>();
        var currentDate = from.Value;

        while (currentDate <= to.Value)
        {
            var holidaysInWeek = await catalogRepository.GetHolidaysInWeek(currentDate, countryCode);
            holidays.AddRange(holidaysInWeek);
            currentDate = currentDate.AddDays(7);
        }

        return [.. holidays.Distinct()];
    }

    private List<InvoiceTotal> ProcessTimesheets(List<TimeSheetApprovedBillingModel> timesheets, List<DateTime> holidays, out decimal tax)
    {
        tax = 0m;
        var items = new List<InvoiceTotal>();

        var workerWeekGroups = timesheets
            .GroupBy(t => new { Week = t.Date.GetWeekEndingCurrentWeek(), t.WorkerId, t.RequestId });

        foreach (var group in workerWeekGroups)
        {
            TimeSpan accumulatedRegularHours = TimeSpan.Zero;

            foreach (var timesheet in group.OrderBy(t => t.Date))
            {
                var itemsBeforeTimesheet = items.Count;
                bool isHoliday = IsPublicHoliday(timesheet.Date, holidays);
                var hoursBreakdown = CalculateHoursForItem(timesheet, ref accumulatedRegularHours, isHoliday);

                var timeSheetTotal = calculatorService.CreateTimeSheetTotalEntity(
                    timesheet.TimeSheetId,
                    hoursBreakdown,
                    accumulatedRegularHours);

                var jobTitle = timesheet.JobTitle;
                var agencyRate = timesheet.AgencyRate;

                var regularHours = hoursBreakdown.RegularHours.TotalHours + hoursBreakdown.OtherRegularHours.TotalHours;
                if (regularHours > 0)
                {
                    var regularAmount = calculatorService.CalculateRegularAmount(agencyRate, regularHours);
                    items.Add(CreateItem(timeSheetTotal, regularHours, agencyRate, $"Charge for {jobTitle} / Regular", agencyRate, regularAmount, item => item.ChargeRegular(regularAmount)));
                }

                var overtimeHours = hoursBreakdown.OvertimeHours.TotalHours;
                if (overtimeHours > 0)
                {
                    var overtimeAmount = calculatorService.CalculateOvertimeAmount(agencyRate, rates.OverTime, overtimeHours);
                    items.Add(CreateItem(timeSheetTotal, overtimeHours, agencyRate * rates.OverTime, $"Charge for {jobTitle} / Overtime", agencyRate, overtimeAmount, item => item.ChargeOvertime(overtimeAmount)));
                }

                var holidayHours = hoursBreakdown.HolidayHours.TotalHours;
                if (holidayHours > 0)
                {
                    var holidayAmount = calculatorService.CalculateHolidayAmount(agencyRate, rates.Holiday, holidayHours);
                    items.Add(CreateItem(timeSheetTotal, holidayHours, agencyRate * rates.Holiday, $"Charge for {jobTitle} / Holiday", agencyRate, holidayAmount, item => item.ChargeHoliday(holidayAmount)));
                }

                var missingHours = timesheet.MissingHours.TotalHours;
                if (missingHours > 0)
                {
                    var missingAmount = calculatorService.CalculateMissingAmount(agencyRate, missingHours);
                    items.Add(CreateItem(timeSheetTotal, missingHours, agencyRate, $"Charge for {jobTitle} / Missing Hours", agencyRate, missingAmount, item => item.ChargeMissing(missingAmount)));
                }

                var missingOvertimeHours = timesheet.MissingHoursOvertime.TotalHours;
                if (missingOvertimeHours > 0)
                {
                    var missingOvertimeAmount = calculatorService.CalculateOvertimeAmount(agencyRate, rates.OverTime, missingOvertimeHours);
                    items.Add(CreateItem(timeSheetTotal, missingOvertimeHours, agencyRate * rates.OverTime, $"Charge for {jobTitle} / Missing Overtime Hours", agencyRate, missingOvertimeAmount, item => item.ChargeMissingOvertime(missingOvertimeAmount)));
                }

                var timesheetGross = items.Skip(itemsBeforeTimesheet).Sum(i => i.Total);
                tax += timesheetGross * timesheet.Tax;
            }
        }

        return items;
    }

    private static InvoiceTotal CreateItem(TimeSheetTotal timeSheetTotal, double quantity, decimal unitPrice, string description, decimal agencyRate, decimal amount, Action<InvoiceTotal> setBucket)
    {
        var item = InvoiceTotal.Create(timeSheetTotal, quantity, unitPrice, description, agencyRate, amount);
        setBucket(item);
        return item;
    }

    private static List<InvoiceSummaryItemModel> ConsolidatePreviewItems(IEnumerable<InvoiceTotal> items) =>
        items
            .GroupBy(i => i.Description)
            .Select(group => ToPreviewItem(group.Key, group.Sum(i => i.Quantity), group.Sum(i => i.Total)))
            .ToList();

    private static List<InvoiceSummaryItemModel> ConsolidatePreviewAdditionalItems(IEnumerable<InvoiceAdditionalItem> additionalItems) =>
        additionalItems
            .Select(item => new InvoiceSummaryItemModel
            {
                Description = item.Description,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Total = item.Total
            })
            .ToList();

    private static List<InvoiceSummaryItemModel> ConsolidatePreviewDiscounts(IEnumerable<InvoiceDiscount> discounts) =>
        discounts
            .GroupBy(d => d.Description)
            .Select(group => ToPreviewItem(group.Key, group.Sum(d => d.Quantity), group.Sum(d => d.Amount)))
            .ToList();

    private static List<InvoiceSummaryItemModel> ConsolidatePreviewHolidays(IEnumerable<InvoiceHoliday> holidays) =>
        holidays
            .GroupBy(h => h.Holiday.Date)
            .Select(group => ToPreviewItem($"Charge for Holiday {group.Key:D}", group.Sum(h => h.Hours), group.Sum(h => h.Amount)))
            .ToList();

    private static InvoiceSummaryItemModel ToPreviewItem(string description, double quantity, decimal total) => new()
    {
        Description = description,
        Quantity = quantity,
        UnitPrice = quantity > 0 ? total / (decimal)quantity : 0,
        Total = total
    };

    private async Task CreateSubcontractorReportsAsync(IEnumerable<Guid> agencyIds, Guid companyProfileId)
    {
        var timesheets = await timeSheetRepository.GetTimeSheetForCreatingReportsSubcontractor(agencyIds, companyProfileId);
        if (timesheets.Count == 0) return;

        var workerWeekGroups = timesheets
            .GroupBy(t => new { t.WorkerId, Week = t.Date.GetWeekEndingCurrentWeek() });

        foreach (var group in workerWeekGroups)
        {
            var first = group.First();
            var workerProfileId = first.WorkerProfileId;

            var from = group.Min(t => t.Date);
            var to = group.Max(t => t.Date);
            var holidays = await GetHolidaysForPeriod(from, to, first.CountryCode);

            var wageDetails = new List<ReportSubcontractorWageDetail>();

            foreach (var requestGroup in group.GroupBy(t => t.RequestId))
            {
                TimeSpan accumulatedRegularHours = TimeSpan.Zero;

                foreach (var timesheet in requestGroup.OrderBy(t => t.Date))
                {
                    bool isHoliday = IsPublicHoliday(timesheet.Date, holidays);
                    var hoursBreakdown = CalculateHoursForItem(timesheet, ref accumulatedRegularHours, isHoliday);

                    var workerRate = timesheet.WorkerRate;
                    var regularHours = hoursBreakdown.RegularHours.TotalHours + hoursBreakdown.OtherRegularHours.TotalHours;
                    var overtimeHours = hoursBreakdown.OvertimeHours.TotalHours;
                    var holidayHours = hoursBreakdown.HolidayHours.TotalHours;

                    var regularAmount = calculatorService.CalculateRegularAmount(workerRate, regularHours);
                    var overtimeAmount = calculatorService.CalculateOvertimeAmount(workerRate, rates.OverTime, overtimeHours);
                    var holidayAmount = calculatorService.CalculateHolidayAmount(workerRate, rates.Holiday, holidayHours);

                    var timeSheetTotal = calculatorService.CreateTimeSheetTotalPayrollEntity(
                        timesheet.TimeSheetId,
                        hoursBreakdown,
                        accumulatedRegularHours);

                    wageDetails.Add(new ReportSubcontractorWageDetail(
                        workerRate: workerRate,
                        regular: regularAmount,
                        otherRegular: 0,
                        missing: 0,
                        missingOvertime: 0,
                        nightShift: 0,
                        holiday: holidayAmount,
                        overtime: overtimeAmount).AttachTimeSheetTotal(timeSheetTotal));
                }
            }

            var totalRegular = wageDetails.Sum(w => w.Regular);
            var totalOvertime = wageDetails.Sum(w => w.Overtime);
            var totalHoliday = wageDetails.Sum(w => w.Holiday);
            var gross = totalRegular + totalOvertime + totalHoliday;

            var report = ReportSubcontractor.Create(
                workerProfileId,
                totalRegular,
                gross,
                group.Min(t => t.Date),
                group.Max(t => t.Date),
                group.Key.Week);

            report.AddWageDetail(wageDetails);

            var otherDeductions = group
                .Where(t => t.DeductionsOthers > 0)
                .Select(t => ReportSubContractorOtherDeduction.CreateDefaultDeduction(t.DeductionsOthers, t.DeductionsOthersDescription));

            report.AddOtherDeductionsDetail(otherDeductions);

            await subcontractorRepository.Create(report);
        }

        await subcontractorRepository.SaveChangesAsync();
    }
}
