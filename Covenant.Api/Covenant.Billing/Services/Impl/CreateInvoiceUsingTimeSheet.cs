using Covenant.Billing.Utils;
using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Entities.Request;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Utils.Extensions;
using Covenant.Invoices.Services;
using Covenant.TimeSheetTotal.Models;
using Covenant.TimeSheetTotal.Services;

namespace Covenant.Billing.Services.Impl
{
    public class CreateInvoiceUsingTimeSheet : ICreateInvoiceUsingTimeSheet
    {
        private readonly TimeLimits _timeLimits;
        private readonly Rates _rates;
        private readonly ICatalogRepository _catalogRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

        public CreateInvoiceUsingTimeSheet(TimeLimits timeLimits, Rates rates,
            ICatalogRepository catalogRepository,
            IInvoiceRepository invoiceRepository)
        {
            _timeLimits = timeLimits;
            _rates = rates;
            _catalogRepository = catalogRepository;
            _invoiceRepository = invoiceRepository;
        }

        public Task<Result<Invoice>> Preview(Guid agencyId, Guid companyProfileId, IReadOnlyCollection<TimeSheetApprovedBillingModel> list, CreateInvoiceModel model) =>
            PrivateCreate(agencyId, companyProfileId, list, model, true);

        public Task<Result<Invoice>> Create(Guid agencyId, Guid companyProfileId, IReadOnlyCollection<TimeSheetApprovedBillingModel> list, CreateInvoiceModel model) =>
            PrivateCreate(agencyId, companyProfileId, list, model);

        private async Task<Result<Invoice>> PrivateCreate(Guid agencyId, Guid companyProfileId, IReadOnlyCollection<TimeSheetApprovedBillingModel> list, CreateInvoiceModel model, bool preview = false)
        {
            if (!list.Any()) return Result.Fail<Invoice>();
            await SemaphoreSlim.WaitAsync();
            try
            {
                var publicHolidays = new List<InvoiceHoliday>();
                var paidHolidays = list.First().PaidHolidays;
                var totals = new List<(ITimeSheetTotal tst, TotalDailyPrice totalDailyPrice)>();
                var timeSheetByWeek = list.GroupTimeSheetByWeek();
                foreach (var timeSheetWeek in timeSheetByWeek)
                {
                    var timesheetByWorker = timeSheetWeek.GroupTimeSheetByWorker();
                    var totalTimesheetByWorker = GetTotals(timesheetByWorker);
                    totals.AddRange(totalTimesheetByWorker);
                    if (paidHolidays)
                    {
                        var date = timeSheetWeek.First().Date;
                        var holidaysInWeek = await _catalogRepository.GetHolidaysInWeek(date);
                        publicHolidays.AddRange(await GetPublicHolidays(holidaysInWeek, companyProfileId));
                    }
                }

                var discounts = model.Discounts.ToInvoiceDiscounts();
                model.Discounts = Array.Empty<CreateInvoiceItemModel>(); //Only can be discount once
                var additionalItems = model.AdditionalItems.ToInvoiceAdditionalItems();
                model.AdditionalItems = Array.Empty<CreateInvoiceItemModel>(); //Only can be added once

                var invoiceTotals = totals.Select(t => t.totalDailyPrice.ToInvoiceTotal(t.tst)).ToList();
                var weekEnding = list.Max(m => m.Date).GetWeekEndingCurrentWeek();
                var invoiceNumber = (await _invoiceRepository.GetNextInvoiceNumber()).NextNumber;
                var invoiceSummaryItemModels = GetInvoiceSummaryItemModels(invoiceTotals, list).ToList();

                Result<Invoice> rInvoice = InvoiceBuilder.Invoice(_rates)
                    .WithInvoiceNumber(invoiceNumber).WithInvoiceDate(model.InvoiceDate)
                    .WithWeekEnding(weekEnding).WithCompanyProfileId(companyProfileId)
                    .WithEmail(model.Email).WithInvoiceTotals(invoiceTotals, invoiceSummaryItemModels)
                    .WithInvoiceHolidays(publicHolidays).WithAdditionalItems(additionalItems)
                    .WithDiscounts(discounts).Build();

                if (!rInvoice || preview) return rInvoice;
                await _invoiceRepository.Create(rInvoice.Value);
                await _invoiceRepository.SaveChangesAsync();
                return rInvoice;
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        private async Task<IReadOnlyCollection<InvoiceHoliday>> GetPublicHolidays(IEnumerable<DateTime> holidaysInWeek, Guid companyProfileId)
        {
            var items = new List<InvoiceHoliday>();
            foreach (DateTime holiday in holidaysInWeek)
            {
                var p = new ParamsToGetRegularWages(companyProfileId, holiday);
                var result = await _invoiceRepository.GetCompanyRegularCharges(p);
                items.AddRange(
                    from item in result
                    select InvoiceHoliday.Create(holiday, item.HoursToPay, item.AmountToPay, item.WorkerProfileId) into rInvoiceHoliday
                    where rInvoiceHoliday
                    select rInvoiceHoliday.Value);
            }
            return items;
        }

        private IEnumerable<(ITimeSheetTotal tst, TotalDailyPrice totalDailyPrice)> GetTotals(IEnumerable<IGrouping<Guid, TimeSheetApprovedBillingModel>> timeSheetGroupedByWorker)
        {
            var totals = new List<(ITimeSheetTotal tst, TotalDailyPrice totalDailyPrice)>();
            foreach (var workerTimeSheet in timeSheetGroupedByWorker)
            {
                var first = workerTimeSheet.First();
                var totalParams = workerTimeSheet.TotalizatorParams();
                var totalsWorker = totalParams.GetInvoiceTotals(_rates, _timeLimits.MaxHoursWeek, first.OvertimeStartsAfter);
                totals.AddRange(totalsWorker);
            }
            return totals;
        }

        private IEnumerable<InvoiceSummaryItemModel> GetInvoiceSummaryItemModels(IEnumerable<InvoiceTotal> invoiceTotals, IEnumerable<TimeSheetApprovedBillingModel> timeSheetApproveds)
        {
            var invoiceItemsTotal = from it in invoiceTotals
                                    join ts in timeSheetApproveds on it.TimeSheetTotal.TimeSheetId equals ts.TimeSheetId
                                    select new InvoiceItemTotalModel
                                    {
                                        RequestId = ts.RequestId,
                                        JobTitle = ts.JobTitle,
                                        Regular = it.Regular,
                                        OtherRegular = it.OtherRegular,
                                        MissingRateAgency = ts.MissingRateAgency,
                                        Missing = it.Missing,
                                        MissingOvertime = it.MissingOvertime,
                                        Holiday = it.Holiday,
                                        Overtime = it.Overtime,
                                        RegularHours = it.TimeSheetTotal.RegularHours.TotalHours,
                                        OtherRegularHours = it.TimeSheetTotal.OtherRegularHours.TotalHours,
                                        MissingHours = ts.MissingHours.TotalHours,
                                        MissingHoursOvertime = ts.MissingHoursOvertime.TotalHours,
                                        HolidayHours = it.TimeSheetTotal.HolidayHours.TotalHours,
                                        OvertimeHours = it.TimeSheetTotal.OvertimeHours.TotalHours
                                    };
            var items = invoiceItemsTotal.ToList().ToInvoiceItems();
            return items;
        }
    }
}