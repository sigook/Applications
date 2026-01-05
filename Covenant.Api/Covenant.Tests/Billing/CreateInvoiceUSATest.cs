using Covenant.Billing.Services.Impl;
using Covenant.Common.Configuration;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Company;
using Moq;
using Xunit;

namespace Covenant.Tests.Billing
{
    public class CreateInvoiceUSATest
    {
        private static readonly DateTime FakeNow = new DateTime(2019, 01, 01);
        private readonly CreateInvoiceUSA _sut;

        public CreateInvoiceUSATest()
        {
            var repository = new Mock<IInvoiceRepository>();
            repository.Setup(s => s.GetNextInvoiceUSANumber()).ReturnsAsync(new NextNumberModel { NextNumber = 1 });
            var timeService = new Mock<ITimeService>();
            timeService.Setup(s => s.GetCurrentDateTime()).Returns(FakeNow);
            var agencyRepository = new Mock<IAgencyRepository>();
            var companyRepository = new Mock<ICompanyRepository>();
            var locationRepository = new Mock<ILocationRepository>();
            locationRepository.Setup(l => l.GetProvinceSalesTax(It.IsAny<Guid>())).ReturnsAsync(new ProvinceTaxModel { Tax1 = 0.06m });
            _sut = new CreateInvoiceUSA(TimeLimits.DefaultTimeLimits, Rates.DefaultRates, repository.Object,
                timeService.Object, agencyRepository.Object, companyRepository.Object, locationRepository.Object);
        }

        [Fact]
        public async Task CreateRegular()
        {
            var result = await _sut.Create(Guid.NewGuid(), Guid.NewGuid(), _timesheet(2), new CreateInvoiceModel
            {
                ProvinceId = Guid.NewGuid()
            });
            Assert.True(result);
            Assert.Single(result.Value.Items);
            Assert.Equal(16, result.Value.Items.ElementAt(0).Quantity);
            Assert.Equal(2, result.Value.Items.ElementAt(0).UnitPrice);
            Assert.Equal(32, result.Value.Items.ElementAt(0).Total);
            Assert.Equal("Charge for General Labour / Regular", result.Value.Items.ElementAt(0).Description);
            Assert.Equal(32, result.Value.SubTotal);
            Assert.Equal(1.92m, result.Value.Tax);
            Assert.Equal(33.92m, result.Value.TotalNet);
            Assert.Equal(FakeNow, result.Value.CreatedAt.Date);
            Assert.Empty(result.Value.Discounts);
            Assert.Equal(2, result.Value.TimeSheetTotals.Count());
        }

        [Fact]
        public async Task CreateWithDiscounts()
        {
            var result = await _sut.Create(Guid.NewGuid(), Guid.NewGuid(), _timesheet(),
                new CreateInvoiceModel
                {
                    ProvinceId = Guid.NewGuid(),
                    Discounts = new[] { new CreateInvoiceItemModel(1, 5, "Discount missing hours") }
                });
            Assert.True(result);
            Assert.Single(result.Value.Items);
            Assert.Equal(8, result.Value.Items.ElementAt(0).Quantity);
            Assert.Equal(2, result.Value.Items.ElementAt(0).UnitPrice);
            Assert.Equal(16, result.Value.Items.ElementAt(0).Total);
            Assert.Equal("Charge for General Labour / Regular", result.Value.Items.ElementAt(0).Description);
            Assert.Single(result.Value.Discounts);
            Assert.Equal(1, result.Value.Discounts.ElementAt(0).Quantity);
            Assert.Equal(5m, result.Value.Discounts.ElementAt(0).UnitPrice);
            Assert.Equal(5m, result.Value.Discounts.ElementAt(0).Total);
            Assert.Equal("Discount missing hours", result.Value.Discounts.ElementAt(0).Description);
            Assert.Equal(11, result.Value.SubTotal);
            Assert.Equal(0.66m, result.Value.Tax);
            Assert.Equal(11.66m, result.Value.TotalNet);
        }

        [Fact]
        public async Task CreateWithAdditionalItems()
        {
            var result = await _sut.Create(Guid.NewGuid(), Guid.NewGuid(), _timesheet(),
                new CreateInvoiceModel
                {
                    ProvinceId = Guid.NewGuid(),
                    AdditionalItems = new[] { new CreateInvoiceItemModel(1, 10, "Missing overtime") }
                });
            Assert.True(result);
            Assert.Equal(2, result.Value.Items.Count());
            Assert.Equal(8, result.Value.Items.ElementAt(0).Quantity);
            Assert.Equal(2, result.Value.Items.ElementAt(0).UnitPrice);
            Assert.Equal(16, result.Value.Items.ElementAt(0).Total);
            Assert.Equal("Charge for General Labour / Regular", result.Value.Items.ElementAt(0).Description);
            Assert.Equal(1, result.Value.Items.ElementAt(1).Quantity);
            Assert.Equal(10, result.Value.Items.ElementAt(1).UnitPrice);
            Assert.Equal(10, result.Value.Items.ElementAt(1).Total);
            Assert.Equal("Missing overtime", result.Value.Items.ElementAt(1).Description);
            Assert.Equal(26, result.Value.SubTotal);
            Assert.Equal(1.56m, result.Value.Tax);
            Assert.Equal(27.56m, result.Value.TotalNet);
            Assert.Empty(result.Value.Discounts);
            Assert.Single(result.Value.TimeSheetTotals);
        }

        [Fact]
        public async Task CreateWithMissingHours()
        {
            var timeSheet = new TimeSheetApprovedBillingModel
            {
                PaidHolidays = default,
                OvertimeStartsAfter = TimeSpan.FromHours(40),
                RequestId = RequestId,
                JobTitle = "AZ Driver",
                AgencyRate = 2,
                BreakIsPaid = false,
                DurationBreak = default,
                HolidayIsPaid = default,
                WorkerId = Guid.NewGuid(),
                TimeSheetId = Guid.NewGuid(),
                Date = FakeNow,
                Week = default,
                TimeInApproved = default,
                TimeOutApproved = default,
                MissingHours = TimeSpan.FromHours(10),
                MissingHoursOvertime = TimeSpan.Zero,
                MissingRateAgency = default,
                IsHoliday = default
            };
            var result = await _sut.Create(Guid.NewGuid(), Guid.NewGuid(), new[] { timeSheet }, new CreateInvoiceModel
            {
                ProvinceId = Guid.NewGuid()
            });
            Assert.True(result);
            Assert.Single(result.Value.Items);
            Assert.Equal(10, result.Value.Items.ElementAt(0).Quantity);
            Assert.Equal(2, result.Value.Items.ElementAt(0).UnitPrice);
            Assert.Equal(20, result.Value.Items.ElementAt(0).Total);
            Assert.Equal("Charge for AZ Driver / Missing", result.Value.Items.ElementAt(0).Description);
            Assert.Equal(20, result.Value.SubTotal);
            Assert.Equal(1.2m, result.Value.Tax);
            Assert.Equal(21.2m, result.Value.TotalNet);
            Assert.Empty(result.Value.Discounts);
            Assert.Single(result.Value.TimeSheetTotals);
        }

        [Fact]
        public async Task CreateWithoutTimesheet()
        {
            var result = await _sut.Create(Guid.NewGuid(), Guid.NewGuid(), Array.Empty<TimeSheetApprovedBillingModel>(),
                new CreateInvoiceModel
                {
                    ProvinceId = Guid.NewGuid(),
                    AdditionalItems = new[] { new CreateInvoiceItemModel(2, 1, "Regular hours") },
                    Discounts = new[] { new CreateInvoiceItemModel(1, 1, "Extra hours overtime") }
                });
            Assert.True(result);
            Assert.Single(result.Value.Items);
            Assert.Single(result.Value.Discounts);
            Assert.Equal(1, result.Value.SubTotal);
            Assert.Equal(0.06m, result.Value.Tax);
            Assert.Equal(1.06m, result.Value.TotalNet);
        }

        private static readonly Guid RequestId = Guid.NewGuid();
        private static IReadOnlyList<TimeSheetApprovedBillingModel> _timesheet(int days = 1) =>
            Enumerable.Range(0, days).Select(s => new TimeSheetApprovedBillingModel
            {
                PaidHolidays = default,
                OvertimeStartsAfter = TimeSpan.FromHours(40),
                RequestId = RequestId,
                JobTitle = "General Labour",
                AgencyRate = 2,
                BreakIsPaid = false,
                DurationBreak = default,
                HolidayIsPaid = default,
                WorkerId = Guid.NewGuid(),
                TimeSheetId = Guid.NewGuid(),
                Week = 1,
                Date = FakeNow,
                TimeInApproved = FakeNow,
                TimeOutApproved = FakeNow.AddHours(8),
                MissingHours = TimeSpan.Zero,
                MissingHoursOvertime = TimeSpan.Zero,
                MissingRateAgency = default,
                IsHoliday = default
            }).ToList();
    }
}