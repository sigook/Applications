using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.PayStub;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Utils.Extensions;
using Covenant.Deductions.Services;
using Covenant.PayStubs.Services;
using Moq;
using Xunit;

namespace Covenant.Tests.Billing
{
    public class TestCreatePayStubUsingTimeSheet
    {
        private readonly Mock<IPayStubRepository> _payStubRepository;
        private readonly Mock<ICatalogRepository> _catalogRepository;
        private readonly Mock<IPayrollDeductionsAndContributionsCalculator> _deductions;
        private readonly Mock<IPayStubPublicHolidays> _payStubPublicHolidays;
        private readonly Mock<IWorkerRepository> _workerRepository;

        public TestCreatePayStubUsingTimeSheet()
        {
            _payStubRepository = new Mock<IPayStubRepository>();
            _payStubPublicHolidays = new Mock<IPayStubPublicHolidays>();
            _catalogRepository = new Mock<ICatalogRepository>();
            _deductions = new Mock<IPayrollDeductionsAndContributionsCalculator>();
            _workerRepository = new Mock<IWorkerRepository>();
            _payStubRepository.Setup(r => r.GetNextPayStubNumbers(It.IsAny<int>()))
                .Returns<int>(limit =>
                    Task.FromResult(Enumerable.Range(1, limit).Select(i => new NextNumberModel { NextNumber = i }).ToList()));
            _workerRepository.Setup(r => r.GetWorkerProfileTaxCategory(It.IsAny<Guid>())).ReturnsAsync(new WorkerProfileTaxCategory
            {
                WorkerProfileId = Guid.NewGuid(),
                FederalCategory = TaxCategory.Cc1,
                ProvincialCategory = TaxCategory.Cc1
            });
        }

        [Theory]
        [InlineData(1, 1, "7")]
        [InlineData(2, 1, "14")]
        [InlineData(1, 2, "7")]
        [InlineData(2, 2, "14")]
        public async Task Create(int weeks, int workers, string regularWage)
        {
            var defaultRates = Rates.DefaultRates;
            defaultRates.Vacations = 0;
            var sut = new CreatePayStubUsingTimeSheet(
                TimeLimits.DefaultTimeLimits,
                defaultRates,
                _catalogRepository.Object,
                _deductions.Object,
                _payStubPublicHolidays.Object,
                _payStubRepository.Object,
                _workerRepository.Object);
            var now = new DateTime(2021, 01, 03);
            Result result = await sut.Create(Data(now, weeks, workers));
            Assert.True(result);
            Assert.Equal(workers, sut.PayStubs.Count);
            Assert.True(sut.PayStubs.All(c =>
            {
                decimal expected = decimal.Parse(regularWage);
                return c.RegularWage == expected && c.TotalEarnings == expected && c.TotalPaid == expected;
            }));
            Assert.All(sut.PayStubs, p => Assert.Single(p.Items));
            Assert.All(sut.PayStubs, p => Assert.Equal(weeks * 7, p.WageDetails.Count()));
            _payStubRepository.Verify(r => r.Create(It.IsAny<PayStub>()), Times.Exactly(workers));
            _payStubRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Theory]
        [InlineData(1, 1, 6)]
        [InlineData(2, 2, 12)]
        [InlineData(4, 4, 24)]
        public async Task Overtime(int weeks, int regular, int overtime)
        {
            var defaultRates = Rates.DefaultRates;
            defaultRates.OverTime = 1;
            var sut = new CreatePayStubUsingTimeSheet(
                TimeLimits.DefaultTimeLimits,
                defaultRates,
                _catalogRepository.Object,
                _deductions.Object,
                _payStubPublicHolidays.Object,
                _payStubRepository.Object,
                _workerRepository.Object);
            var now = new DateTime(2021, 01, 03);
            IEnumerable<TimeSheetApprovedPayrollModel> data = Data(now, weeks: weeks, overtimeStarts: 1);
            Result result = await sut.Create(data);
            Assert.True(result);
            PayStub payStub = sut.PayStubs.Single();
            Assert.Equal(regular, payStub.Items.Where(c => c.Type == PayStubItemType.Regular).Sum(s => s.Total));
            Assert.Equal(overtime, payStub.Items.Where(c => c.Type == PayStubItemType.Overtime).Sum(s => s.Total));
        }

        private readonly Dictionary<int, DateTime[]> _holidays = new Dictionary<int, DateTime[]>
        {
            {2,new []{new  DateTime(2021,01,04)}},
            {3,new []{new  DateTime(2021,01,15)}},
            {4,new DateTime[0]},
            {5,new []{new  DateTime(2021,01,21),new  DateTime(2021,01,22)}}
        };

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 2)]
        [InlineData(4, 4)]
        public async Task Holidays(int weeks, int expected)
        {
            var defaultRates = Rates.DefaultRates;
            var now = new DateTime(2021, 01, 03);
            _catalogRepository.Setup(r => r.GetHolidaysInWeek(It.IsAny<DateTime>()))
                .Returns<DateTime>(firstDate => Task.FromResult(_holidays[firstDate.GetWeekOfYearStartSunday()].ToList()));

            _payStubPublicHolidays.Setup(ps => ps.GetWorkerPublicHolidays(It.IsAny<IEnumerable<DateTime>>(), It.IsAny<Guid>()))
                .Returns<IEnumerable<DateTime>, Guid>((holidaysInWeek, _) =>
                    Task.FromResult(Result.Ok<IReadOnlyCollection<PayStubPublicHoliday>>(holidaysInWeek.Select(h => PayStubPublicHoliday.Create(h, 1).Value)
                        .ToList())));
            var sut = new CreatePayStubUsingTimeSheet(
                TimeLimits.DefaultTimeLimits,
                defaultRates,
                _catalogRepository.Object,
                _deductions.Object,
                _payStubPublicHolidays.Object,
                _payStubRepository.Object,
                _workerRepository.Object);

            Result result = await sut.Create(Data(now, weeks));
            Assert.True(result);
            PayStub payStub = sut.PayStubs.Single();
            Assert.Equal(expected, payStub.Holidays.Count());
            Assert.Equal(expected, payStub.PublicHolidayPay);
        }

        private static IEnumerable<TimeSheetApprovedPayrollModel> Data(
            DateTime now,
            int weeks = 1,
            int workers = 1,
            double overtimeStarts = default) =>
            Enumerable.Range(0, workers).SelectMany(w =>
            {
                var workerId = Guid.NewGuid();
                Guid workerProfileId = workerId;
                return Enumerable.Range(0, weeks * 7).Select(i =>
                    new TimeSheetApprovedPayrollModel(
                        overtimeStarts.Equals(default) ? CompanyProfile.DefaultOvertimeStarts : TimeSpan.FromHours(overtimeStarts),
                        default,
                        1,
                        default,
                        default,
                        default,
                        workerId,
                        workerProfileId,
                        default,
                        now.AddDays(i).GetWeekOfYearStartSunday(),
                        now.AddDays(i),
                        now.AddDays(i),
                        now.AddDays(i).AddHours(1),
                        default,
                        default,
                        default,
                        default,
                        default,
                        default,
                        default,
                        default,
                        default,
                        default));
            });
    }
}