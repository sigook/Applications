using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.PayStub;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Resources;
using Covenant.Deductions.Models;
using Covenant.Deductions.Services;
using Covenant.PayStubs.Services;
using Moq;
using Xunit;

namespace Covenant.Tests.Billing.PayStubTest
{
    public class TestPayStubBuilder
    {
        private readonly Mock<IPayrollDeductionsAndContributionsCalculator> _calculatePayrollDeductionsService;
        public TestPayStubBuilder() => _calculatePayrollDeductionsService = new Mock<IPayrollDeductionsAndContributionsCalculator>();

        /// <summary>
        /// Regular : 40 * 20 = 800
        /// Deduction : 1 * 20 = 20
        /// Total paid : 800 - 20 = 780
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Build()
        {
            Result<PayStubItem> regular = PayStubItem.CreateItem("Regular Hours", 40, 20, PayStubItemType.Regular);
            var items = new List<PayStubItem> { regular.Value };

            _calculatePayrollDeductionsService
                .Setup(s => s.CalculateFor(It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<WorkerProfileTaxCategory>()))
                .ReturnsAsync(new PayrollDeductionsAndContributionsResult(1, 1, 1, 1));

            Result<PayStub> sub = await PayStubBuilder
                .PayStub(new Rates(), _calculatePayrollDeductionsService.Object, Mock.Of<IWorkerRepository>())
                .WithPayStubNumber(001)
                .WithWorkerProfileId(Guid.NewGuid()).WithTypeOfWork("General Labour")
                .WithWorkBeginning(new DateTime(2019, 01, 01)).WithWorkEnding(new DateTime(2019, 01, 07))
                .WithCreationDate(new DateTime(2019, 01, 01))
                .WithItems(items).WithoutWageDetails().WithoutPublicHolidaysToPay()
                .WithOtherDeductions(PayStubOtherDeduction.CreateDefaultDeduction(20, "Deuction")).WithoutReimbursement().PayVacations().Build();

            Assert.True(sub);
            Assert.NotNull(sub.Value);
            Assert.Equal(800, sub.Value.TotalEarnings);
            Assert.Equal(776, sub.Value.TotalPaid);
            Assert.Equal(20, sub.Value.OtherDeductions);
            Assert.Equal(24, sub.Value.TotalDeductions);
        }

        [Fact]
        public async Task PayStub_With_Wage_Detail()
        {
            List<PayStubWageDetail> wageDetails = Enumerable.Range(1, 10)
                .Select(i => new PayStubWageDetail(i, i, i, i, i, i, i, i, Guid.NewGuid()))
                .ToList();
            PayStub sub = (await PayStubBuilder
                .PayStub(new Rates(), _calculatePayrollDeductionsService.Object, Mock.Of<IWorkerRepository>())
                .WithPayStubNumber(001)
                .WithWorkerProfileId(Guid.NewGuid()).WithTypeOfWork(default)
                .WithWorkBeginning(new DateTime(2019, 01, 01)).WithWorkEnding(new DateTime(2019, 01, 07))
                .WithCreationDate(new DateTime(2019, 01, 01))
                .WithItems(new[] { PayStubItem.CreateBonusOthersItem(1, "Bonus").Value })
                .WithWageDetails(wageDetails)
                .WithoutPublicHolidaysToPay()
                .WithNoMoreDeductions()
                .WithoutReimbursement()
                .PayVacations().Build()).Value;

            Assert.Equal(wageDetails.Count, sub.WageDetails.Count());
            Assert.Contains(wageDetails, w =>
                sub.WageDetails.Any(d => d.TimeSheetTotalId == w.TimeSheetTotalId
                && d.WorkerRate == w.WorkerRate
                && d.Regular == w.Regular
                && d.Missing == w.Missing
                && d.MissingOvertime == w.MissingOvertime
                && d.NightShift == w.NightShift
                && d.Holiday == w.Holiday
                && d.Overtime == w.Overtime));
        }

        [Fact]
        public async Task PayStub_With_PublicHolidays()
        {
            PayStubPublicHoliday[] publicHolidays = {
                PayStubPublicHoliday.Create(new DateTime(2019,01,01),10,"10 holiday").Value,
                PayStubPublicHoliday.Create(new DateTime(2019,01,02),20,"20 holiday").Value,
            };
            PayStub sub = (await PayStubBuilder
                .PayStub(new Rates(), _calculatePayrollDeductionsService.Object, Mock.Of<IWorkerRepository>())
                .WithPayStubNumber(001)
                .WithWorkerProfileId(Guid.NewGuid()).WithTypeOfWork(default)
                .WithWorkBeginning(new DateTime(2019, 01, 01)).WithWorkEnding(new DateTime(2019, 01, 07))
                .WithCreationDate(new DateTime(2019, 01, 01))
                .WithItems(new[] { PayStubItem.CreateBonusOthersItem(1, "Bonus").Value })
                .WithoutWageDetails()
                .WithPublicHolidaysToPay(publicHolidays)
                .WithNoMoreDeductions()
                .WithoutReimbursement()
                .PayVacations().Build()).Value;

            Assert.Equal(31, sub.TotalPaid);
            Assert.NotEmpty(sub.Holidays);
            Assert.Equal(publicHolidays.Sum(h => h.Amount), sub.PublicHolidayPay);
            Assert.Contains(publicHolidays, h => sub.Holidays.Any(h2 =>
                h2.Id == h.Id
                && h2.Holiday == h.Holiday
                && h2.Amount == h.Amount
                && h2.Description == h.Description
                && h2.PayStubId == sub.Id));
        }

        [Theory]
        [InlineData(true, 200, 300)]
        [InlineData(false, 0, 100)]
        public async Task PayStub_With_Vacations(bool payVacations, decimal vacations, decimal totalPaid)
        {
            var rates = new Rates { Vacations = 2 };
            PayStub sub = (await PayStubBuilder
                .PayStub(rates, _calculatePayrollDeductionsService.Object, Mock.Of<IWorkerRepository>())
                .WithPayStubNumber(001)
                .WithWorkerProfileId(Guid.NewGuid()).WithTypeOfWork(default)
                .WithWorkBeginning(new DateTime(2019, 01, 01)).WithWorkEnding(new DateTime(2019, 01, 07))
                .WithCreationDate(new DateTime(2019, 01, 01))
                .WithItems(new[] { PayStubItem.CreateBonusOthersItem(100, "Bonus").Value })
                .WithoutWageDetails()
                .WithoutPublicHolidaysToPay()
                .WithNoMoreDeductions()
                .WithoutReimbursement()
                .PayVacations(payVacations).Build()).Value;

            Assert.Equal(vacations, sub.Vacations);
            Assert.Equal(totalPaid, sub.TotalPaid);
        }

        [Fact]
        public async Task If_No_Items_Build_Must_Fail()
        {
            var empty = new PayStubItem[0];
            Result<PayStub> sub = await PayStubBuilder
                .PayStub(new Rates(), _calculatePayrollDeductionsService.Object, Mock.Of<IWorkerRepository>())
                .WithPayStubNumber(001)
                .WithWorkerProfileId(Guid.NewGuid()).WithTypeOfWork("General Labour")
                .WithWorkBeginning(new DateTime(2019, 01, 01)).WithWorkEnding(new DateTime(2019, 01, 07))
                .WithCreationDate(new DateTime(2019, 01, 01))
                .WithItems(empty).WithoutWageDetails().WithoutPublicHolidaysToPay()
                .WithNoMoreDeductions().WithoutReimbursement().PayVacations().Build();

            Assert.False(sub);
            Assert.Equal(sub.Errors.First().Message, ApiResources.There_is_not_enough_information_to_generate_pay_stub);
        }

        [Theory]
        [InlineData("")]
        [InlineData("10")]
        [InlineData("10,20")]
        [InlineData("10,20,30,40,50,60")]
        public async Task PayStub_With_Other_Deductions(string deductionsData)
        {
            var items = new List<PayStubItem> { PayStubItem.CreateBonusOthersItem(1, "Bonus").Value };
            IEnumerable<int> deductions = deductionsData.Split(",").Where(s => !string.IsNullOrEmpty(s)).Select(int.Parse).ToList();
            PayStub payStub = (await PayStubBuilder.PayStub(Rates.DefaultRates, _calculatePayrollDeductionsService.Object, Mock.Of<IWorkerRepository>())
                .WithPayStubNumber(001)
                .WithWorkerProfileId(Guid.NewGuid())
                .WithTypeOfWork(default)
                .WithWorkBeginning(new DateTime(2019, 01, 01)).WithWorkEnding(new DateTime(2019, 01, 07))
                .WithCreationDate(new DateTime(2019, 01, 01))
                .WithItems(items)
                .WithoutWageDetails()
                .WithoutPublicHolidaysToPay()
                .WithOtherDeductions(deductions.Select(i => PayStubOtherDeduction.CreateDefaultDeduction(i, $"Deduction {i}")))
                .WithoutReimbursement()
                .PayVacations()
                .Build()).Value;

            Assert.Equal(deductions.Count(), payStub.OtherDeductionsDetail.Count());
            Assert.Equal(deductions.Sum(), payStub.OtherDeductions);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("0,0")]
        public async Task PayStub_With_Other_Deductions_Zero_Must_Be_Ignore(string deductionsData)
        {
            var items = new List<PayStubItem> { PayStubItem.CreateBonusOthersItem(1, "Bonus").Value };
            IEnumerable<int> deductions = deductionsData.Split(",").Where(s => !string.IsNullOrEmpty(s)).Select(int.Parse).ToList();
            PayStub payStub = (await PayStubBuilder.PayStub(Rates.DefaultRates, _calculatePayrollDeductionsService.Object, Mock.Of<IWorkerRepository>())
                .WithPayStubNumber(001)
                .WithWorkerProfileId(Guid.NewGuid())
                .WithTypeOfWork(default)
                .WithWorkBeginning(new DateTime(2019, 01, 01)).WithWorkEnding(new DateTime(2019, 01, 07))
                .WithCreationDate(new DateTime(2019, 01, 01))
                .WithItems(items)
                .WithoutWageDetails()
                .WithoutPublicHolidaysToPay()
                .WithOtherDeductions(deductions.Select(i => PayStubOtherDeduction.CreateDefaultDeduction(i, $"Deduction {i}")))
                .WithoutReimbursement()
                .PayVacations()
                .Build()).Value;

            Assert.Empty(payStub.OtherDeductionsDetail);
            Assert.Equal(decimal.Zero, payStub.OtherDeductions);
        }

        [Fact]
        public async Task PayStub_With_Other_Deductions_Total_Has_Deductions()
        {
            var items = new List<PayStubItem> { PayStubItem.CreateBonusOthersItem(100, "Bonus").Value };
            PayStub payStub = (await PayStubBuilder.PayStub(Rates.DefaultRates, _calculatePayrollDeductionsService.Object, Mock.Of<IWorkerRepository>())
                .WithPayStubNumber(001)
                .WithWorkerProfileId(Guid.NewGuid())
                .WithTypeOfWork(default)
                .WithWorkBeginning(new DateTime(2019, 01, 01)).WithWorkEnding(new DateTime(2019, 01, 07))
                .WithCreationDate(new DateTime(2019, 01, 01))
                .WithItems(items)
                .WithoutWageDetails()
                .WithoutPublicHolidaysToPay()
                .WithOtherDeductions(PayStubOtherDeduction.CreateDefaultDeduction(10, "Deduction 10"))
                .WithoutReimbursement()
                .PayVacations(false)
                .Build()).Value;
            Assert.Equal(90, payStub.TotalPaid);
        }

        [Fact]
        public async Task PayStub_With_Payroll_Deductions_And_Other_Deductions()
        {
            var workDate = new DateTime(2019, 01, 01);
            const int earnings = 100;
            var items = new List<PayStubItem> { PayStubItem.CreateBonusOthersItem(earnings, "Bonus").Value };
            var payrollDeductions = new PayrollDeductionsAndContributionsResult(1, 2, 3, 4);
            _calculatePayrollDeductionsService.Setup(c => c.CalculateFor(earnings, 1, workDate.Year, It.IsAny<WorkerProfileTaxCategory>()))
                .ReturnsAsync(payrollDeductions);
            var otherDeduction = PayStubOtherDeduction.CreateDefaultDeduction(10, "Deduction 10");
            PayStub payStub = (await PayStubBuilder.PayStub(Rates.DefaultRates, _calculatePayrollDeductionsService.Object, Mock.Of<IWorkerRepository>())
                .WithPayStubNumber(001)
                .WithWorkerProfileId(Guid.NewGuid())
                .WithTypeOfWork(default)
                .WithWorkBeginning(workDate).WithWorkEnding(workDate)
                .WithCreationDate(new DateTime(2019, 01, 01))
                .WithItems(items)
                .WithoutWageDetails()
                .WithoutPublicHolidaysToPay()
                .WithOtherDeductions(otherDeduction)
                .WithoutReimbursement()
                .PayVacations(false)
                .Build()).Value;
            Assert.Equal(payrollDeductions.Cpp, payStub.Cpp);
            Assert.Equal(payrollDeductions.Ei, payStub.Ei);
            Assert.Equal(payrollDeductions.FederalTax, payStub.FederalTax);
            Assert.Equal(payrollDeductions.ProvincialTax, payStub.ProvincialTax);
            Assert.Equal(otherDeduction.Total, payStub.OtherDeductions);
            Assert.Equal(20, payStub.TotalDeductions);
            Assert.Equal(80, payStub.TotalPaid);
        }
    }
}