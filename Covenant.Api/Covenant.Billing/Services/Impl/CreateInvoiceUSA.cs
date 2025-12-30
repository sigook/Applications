using Covenant.Billing.Utils;
using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Entities.Request;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.TimeSheetTotal.Models;
using Covenant.TimeSheetTotal.Services;

namespace Covenant.Billing.Services.Impl
{
    public class CreateInvoiceUSA
    {
        private readonly TimeLimits _timeLimits;
        private readonly Rates _rates;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ITimeService _timeService;
        private readonly IAgencyRepository _agencyRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly ILocationRepository locationRepository;
        private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

        public CreateInvoiceUSA(
            TimeLimits timeLimits,
            Rates rates,
            IInvoiceRepository invoiceRepository,
            ITimeService timeService,
            IAgencyRepository agencyRepository,
            ICompanyRepository companyRepository,
            ILocationRepository locationRepository)
        {
            _timeLimits = timeLimits;
            _rates = rates;
            _invoiceRepository = invoiceRepository;
            _timeService = timeService;
            _agencyRepository = agencyRepository;
            _companyRepository = companyRepository;
            this.locationRepository = locationRepository;
        }

        public Task<Result<InvoiceUSA>> Preview(
            Guid agencyId,
            Guid companyProfileId,
            IReadOnlyCollection<TimeSheetApprovedBillingModel> list, CreateInvoiceModel model) =>
            PrivateCreate(agencyId, companyProfileId, list, model, true);

        public Task<Result<InvoiceUSA>> Create(
            Guid agencyId,
            Guid companyProfileId,
            IReadOnlyCollection<TimeSheetApprovedBillingModel> list, CreateInvoiceModel model) =>
            PrivateCreate(agencyId, companyProfileId, list, model);

        private async Task<Result<InvoiceUSA>> PrivateCreate(Guid agencyId, Guid companyProfileId, IReadOnlyCollection<TimeSheetApprovedBillingModel> list, CreateInvoiceModel model, bool preview = false)
        {
            await SemaphoreSlim.WaitAsync();
            try
            {
                var totals = new List<(List<ITimeSheetTotal> totals, List<InvoiceItemTotalModel> items)>();
                var timeSheetByWeek = list.GroupTimeSheetByWeek();
                foreach (var timeSheetWeek in timeSheetByWeek)
                {
                    foreach (var workerTimeSheet in timeSheetWeek.GroupTimeSheetByWorker())
                    {
                        var first = workerTimeSheet.First();
                        var totalsWorker = GetInvoiceUSATotals(workerTimeSheet.TotalizatorParams(), _rates, _timeLimits.MaxHoursWeek, first.OvertimeStartsAfter);
                        totals.AddRange(totalsWorker);
                    }
                }

                var items = totals.SelectMany(s => s.items).ToInvoiceItems().ToInvoiceUSAItems();
                var salesTax = model.ProvinceId.HasValue ? await locationRepository.GetProvinceSalesTax(model.ProvinceId.Value) : default;
                var rInvoice = InvoiceUSA.Create(
                    (await _invoiceRepository.GetNextInvoiceUSANumber()).NextNumber,
                    model.InvoiceDate ?? _timeService.GetCurrentDateTime(), companyProfileId,
                    items.Concat(model.AdditionalItems.ToInvoiceUSAAdditionalItems()),
                    model.Discounts.ToInvoiceUSADiscounts(),
                    salesTax);
                if (!rInvoice || preview) return rInvoice;

                var invoice = rInvoice.Value;
                invoice.WeekEnding = list.Any() ? list.Max(m => m.Date).GetWeekEndingCurrentWeek() : null;
                invoice.BillToEmail = model.Email;
                invoice.AddTimesheetTotals(totals.SelectMany(s => s.totals));
                var agency = await _agencyRepository.GetAgency(agencyId);
                if (agency != null)
                {
                    invoice.BillFromAddress = agency.BillingAddress?.FormattedAddress;
                    invoice.BillFromPhone = agency.FormattedPhone;
                }
                var company = await _companyRepository.GetCompanyProfile(cp => cp.Id == companyProfileId);
                if (company != null)
                {
                    if (string.IsNullOrEmpty(invoice.BillToEmail)) invoice.BillToEmail = company.Company?.Email;
                    invoice.BillToAddress = company.Locations.FirstOrDefault(f => f.IsBilling)?.Location?.FormattedAddress;
                    invoice.BillToPhone = company.FormattedPhone;
                    invoice.BillToFax = company.FormattedFax;
                }

                await _invoiceRepository.Create(invoice);
                await _invoiceRepository.SaveChangesAsync();
                return rInvoice;
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        /// <summary>
        /// Timesheet must be group by requestId in order to calculate overtime
        /// </summary>
        /// <param name="list"></param>
        /// <param name="rates"></param>
        /// <param name="maxHoursWeek"></param>
        /// <param name="overtimeStartsFrom"></param>
        /// <returns></returns>
        private static IEnumerable<(List<ITimeSheetTotal> totals, List<InvoiceItemTotalModel> items)> GetInvoiceUSATotals(IEnumerable<TotalizatorCreateInvoiceParams> list, Rates rates, TimeSpan maxHoursWeek, TimeSpan overtimeStartsFrom)
        {
            return list.GroupBy(m => m.RequestId).Select(m =>
            {
                string jobTitle = m.First().JobTitle;
                TimeSpan accumulatedRegularHours = TimeSpan.Zero;
                var totals = new List<ITimeSheetTotal>();
                var items = new List<InvoiceItemTotalModel>();
                foreach (var t in m.OrderBy(o => o.Date))
                {
                    ITimeSheetTotal tst = TimeSheetTotalCalculator.Calculate(t.ToCalculateTimeSheetTotalParams(maxHoursWeek, overtimeStartsFrom), ref accumulatedRegularHours);
                    var totalDailyPrice = new TotalDailyPrice(rates, t.AgencyRate, t.MissingRateAgency, t.MissingHours, t.MissingHoursOvertime,
                        tst.RegularHours, tst.OtherRegularHours, tst.NightShiftHours, tst.HolidayHours, tst.OvertimeHours);
                    var model = new InvoiceItemTotalModel
                    {
                        RequestId = m.Key,
                        JobTitle = jobTitle,
                        Regular = totalDailyPrice.Regular,
                        OtherRegular = totalDailyPrice.OtherRegular,
                        Missing = totalDailyPrice.Missing,
                        MissingOvertime = totalDailyPrice.MissingOvertime,
                        Holiday = totalDailyPrice.Holiday,
                        Overtime = totalDailyPrice.Overtime,
                        RegularHours = tst.RegularHours.TotalHours,
                        OtherRegularHours = tst.OtherRegularHours.TotalHours,
                        MissingHours = t.MissingHours.TotalHours,
                        MissingHoursOvertime = t.MissingHoursOvertime.TotalHours,
                        HolidayHours = tst.HolidayHours.TotalHours,
                        OvertimeHours = tst.OvertimeHours.TotalHours,
                        MissingRateAgency = t.MissingRateAgency
                    };
                    totals.Add(tst);
                    items.Add(model);
                }

                return (totals, items);
            }).ToList();
        }
    }
}