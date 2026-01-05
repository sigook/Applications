using Covenant.Billing.Services.Impl;
using Covenant.Common.Configuration;
using Covenant.Common.Entities.Company;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Utils.Extensions;
using Moq;
using Xunit;

namespace Covenant.Tests.Billing
{
    public class TestCreateInvoiceUsingTimeSheet
    {
        private CreateInvoiceUsingTimeSheet _sut;
        private readonly Mock<ICatalogRepository> _catalogRepository;
        private readonly Mock<IInvoiceRepository> _invoiceRepository;

        public TestCreateInvoiceUsingTimeSheet()
        {
            _catalogRepository = new Mock<ICatalogRepository>();
            _invoiceRepository = new Mock<IInvoiceRepository>();
            _invoiceRepository.Setup(r => r.GetNextInvoiceNumber()).ReturnsAsync(new NextNumberModel());
            _sut = new CreateInvoiceUsingTimeSheet(TimeLimits.DefaultTimeLimits, Rates.DefaultRates, _catalogRepository.Object, _invoiceRepository.Object);
        }
        [Theory]
        [InlineData(1, "7", "0.91", "7.91")]
        [InlineData(1, "7", "0.91", "7.91", false)]
        [InlineData(2, "14", "1.82", "15.82")]
        [InlineData(4, "28", "3.64", "31.64")]
        [InlineData(4, "28", "3.64", "31.64", false)]
        public async Task Create(int weeks, string subTotal, string hst, string totalNet, bool sameWorker = true)
        {
            var agencyId = Guid.NewGuid();
            var companyProfileId = Guid.NewGuid();
            var data = GetData(new DateTime(2021, 01, 03), weeks, sameWorker);
            var result = await _sut.Create(agencyId, companyProfileId, data, new CreateInvoiceModel());
            Assert.True(result);
            var invoice = result.Value;
            Assert.Equal(decimal.Parse(subTotal), invoice.SubTotal);
            Assert.Equal(decimal.Parse(hst), invoice.Hst);
            Assert.Equal(decimal.Parse(totalNet), invoice.TotalNet);
            Assert.Equal(data.Count, invoice.InvoiceTotals.Count());
            Assert.Empty(invoice.Discounts);
            Assert.Empty(invoice.Holidays);
            Assert.Empty(invoice.AdditionalItems);
        }

        [Theory]
        [InlineData(1, "7", "6", "1")]
        [InlineData(2, "14", "12", "2")]
        [InlineData(4, "28", "24", "4")]
        public async Task Overtime(int weeks, string subTotal, string totalOvertime, string totalRegular)
        {
            var agencyId = Guid.NewGuid();
            var companyProfileId = Guid.NewGuid();
            var defaultRates = Rates.DefaultRates;
            defaultRates.OverTime = 1;
            TimeSpan overtimeStarts = TimeSpan.FromHours(1);
            var data = GetData(new DateTime(2021, 01, 03), weeks, overtimeStarts: overtimeStarts);
            _sut = new CreateInvoiceUsingTimeSheet(TimeLimits.DefaultTimeLimits, defaultRates, _catalogRepository.Object, _invoiceRepository.Object);
            var result = await _sut.Create(agencyId, companyProfileId, data, new CreateInvoiceModel());
            var invoice = result.Value;
            Assert.Equal(decimal.Parse(subTotal), invoice.SubTotal);
            Assert.Equal(decimal.Parse(totalOvertime), invoice.TotalOvertime);
            Assert.Equal(decimal.Parse(totalRegular), invoice.TotalRegular);
        }

        [Theory]
        [InlineData(1, "21", "7", "14")]
        [InlineData(2, "42", "14", "28")]
        [InlineData(4, "84", "28", "56")]
        public async Task OvertimeMoreThanOneWorker(int weeks, string subTotal, string totalOvertime, string totalRegular)
        {
            var agencyId = Guid.NewGuid();
            var companyProfileId = Guid.NewGuid();
            var defaultRates = Rates.DefaultRates;
            defaultRates.OverTime = 1;
            TimeSpan overtimeStarts = TimeSpan.FromHours(2);
            const int dailyHours = 3;
            var data = GetData(new DateTime(2021, 01, 03),
                weeks, overtimeStarts: overtimeStarts, sameWorker: false, dailyHours: dailyHours);
            _sut = new CreateInvoiceUsingTimeSheet(TimeLimits.DefaultTimeLimits, defaultRates, _catalogRepository.Object, _invoiceRepository.Object);
            var result = await _sut.Create(agencyId, companyProfileId, data, new CreateInvoiceModel());
            var invoice = result.Value;
            Assert.Equal(decimal.Parse(subTotal), invoice.SubTotal);
            Assert.Equal(decimal.Parse(totalOvertime), invoice.TotalOvertime);
            Assert.Equal(decimal.Parse(totalRegular), invoice.TotalRegular);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task Holidays(int weeks)
        {
            var agencyId = Guid.NewGuid();
            var companyProfileId = Guid.NewGuid();
            var now = new DateTime(2021, 01, 03);
            _catalogRepository.Setup(r => r.GetHolidaysInWeek(It.IsAny<DateTime>()))
                .Returns<DateTime>(firstDateOfTheWeek => Task.FromResult(firstDateOfTheWeek.GetWeekOfYearStartSunday() == now.GetWeekOfYearStartSunday()
                        ? new List<DateTime> { now }
                        : new List<DateTime>()));
            var companyRegularChargesByWorker = new CompanyRegularChargesByWorker(Guid.NewGuid(), Guid.NewGuid(), 1, 10, 10);
            _invoiceRepository.Setup(r => r.GetCompanyRegularCharges(It.IsAny<ParamsToGetRegularWages>()))
                .ReturnsAsync(new List<CompanyRegularChargesByWorker> { companyRegularChargesByWorker });

            var data = GetData(now, paidHolidays: true, weeks: weeks);
            var result = await _sut.Create(agencyId, companyProfileId, data, new CreateInvoiceModel());
            var invoice = result.Value;
            Assert.NotEmpty(invoice.Holidays);
            var invoiceHoliday = invoice.Holidays.Single();
            Assert.Equal(1, invoiceHoliday.Amount);
            Assert.Equal(1, invoiceHoliday.Hours);
            Assert.Equal(companyRegularChargesByWorker.AgencyRate, invoiceHoliday.UnitPrice);
        }

        private static List<TimeSheetApprovedBillingModel> GetData(DateTime now,
            int weeks = 1,
            bool sameWorker = true,
            int dailyHours = 1,
            TimeSpan overtimeStarts = default,
            bool paidHolidays = false) =>
            Enumerable.Range(0, weeks * 7).Select(i => new TimeSheetApprovedBillingModel
            {
                PaidHolidays = paidHolidays,
                OvertimeStartsAfter = overtimeStarts == default ? CompanyProfile.DefaultOvertimeStarts : overtimeStarts,
                RequestId = default,
                JobTitle = default,
                AgencyRate = 1,
                BreakIsPaid = default,
                DurationBreak = default,
                HolidayIsPaid = default,
                WorkerId = sameWorker ? Guid.Empty : Guid.NewGuid(),
                TimeSheetId = Guid.NewGuid(),
                Week = now.AddDays(i).GetWeekOfYearStartSunday(),
                Date = now.AddDays(i),
                TimeInApproved = now.AddDays(i),
                TimeOutApproved = now.AddDays(i).AddHours(dailyHours),
                MissingHours = default,
                MissingHoursOvertime = default,
                MissingRateAgency = default,
                IsHoliday = default
            }).ToList();
    }
}