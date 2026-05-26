using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.PayStub;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Core.BL.Services;
using Covenant.Core.BL.Services.Shared;
using Moq;
using Xunit;

namespace Covenant.Tests.Billing.Holidays;

public class GeneratePayStubHolidayPayTest
{
    private static readonly DateTime Holiday = new(2026, 05, 18);
    private static readonly Guid WorkerId = Guid.NewGuid();

    private readonly Mock<IPayStubRepository> _payStubRepository = new();
    private readonly Mock<ITimeSheetRepository> _timeSheetRepository = new();
    private readonly Mock<ICatalogRepository> _catalogRepository = new();
    private readonly PayStubService _sut;
    private PayStub _captured;

    public GeneratePayStubHolidayPayTest()
    {
        var rates = Rates.DefaultRates;
        var timeLimits = TimeLimits.DefaultTimeLimits;

        var calculator = new TimesheetCalculatorService(
            Mock.Of<IDeductionsRepository>(),
            Mock.Of<IWorkerRepository>(),
            rates,
            timeLimits);

        _payStubRepository.Setup(r => r.GetNextPayStubNumbers(1))
            .ReturnsAsync(new List<NextNumberModel> { new() { NextNumber = 1 } });
        _payStubRepository.Setup(r => r.Create(It.IsAny<PayStub>()))
            .Callback<PayStub>(p => _captured = p)
            .Returns(Task.CompletedTask);
        _payStubRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        _timeSheetRepository.Setup(r => r.GetTimeSheetForCreatingPayStubs(It.IsAny<IEnumerable<Guid>>(), It.IsAny<Guid>()))
            .ReturnsAsync(new List<TimeSheetApprovedPayrollModel> { WorkedDay(rate: 20, hours: 8) });

        _catalogRepository.Setup(r => r.GetHolidaysInWeek(It.IsAny<DateTime>(), It.IsAny<string>()))
            .ReturnsAsync(new List<DateTime> { Holiday });

        _sut = new PayStubService(
            _payStubRepository.Object,
            calculator,
            _timeSheetRepository.Object,
            _catalogRepository.Object,
            rates,
            timeLimits);
    }

    [Fact]
    public async Task Pays_Holiday_Using_The_Regular_Formula_When_Entitled()
    {
        SetupHoliday(new RegularWageWorker { IsEntitledToReceiveHolidayPay = true });
        SetupWindow(WorkedDay(rate: 20, hours: 8));

        var result = await _sut.Generate(new[] { Guid.NewGuid() }, new[] { WorkerId });

        Assert.True(result);
        var holidayPay = Assert.Single(_captured.Holidays);
        Assert.True(holidayPay.Amount > 0);
    }

    [Fact]
    public async Task Pays_Holiday_Using_The_Custom_Value()
    {
        const decimal customValue = 99m;
        SetupHoliday(new RegularWageWorker { IsEntitledToReceiveHolidayPay = true, CustomPublicHolidayValue = customValue });
        SetupWindow();

        var result = await _sut.Generate(new[] { Guid.NewGuid() }, new[] { WorkerId });

        Assert.True(result);
        var holidayPay = Assert.Single(_captured.Holidays);
        Assert.Equal(customValue, holidayPay.Amount);
    }

    [Fact]
    public async Task Does_Not_Pay_Holiday_When_It_Was_Already_Paid()
    {
        SetupHoliday(new RegularWageWorker { IsEntitledToReceiveHolidayPay = true, HolidayWasPaid = true });
        SetupWindow(WorkedDay(rate: 20, hours: 8));

        var result = await _sut.Generate(new[] { Guid.NewGuid() }, new[] { WorkerId });

        Assert.True(result);
        Assert.Empty(_captured.Holidays);
    }

    [Fact]
    public async Task Does_Not_Pay_Holiday_When_Worker_Is_Not_Entitled()
    {
        SetupHoliday(new RegularWageWorker { IsEntitledToReceiveHolidayPay = false });
        SetupWindow(WorkedDay(rate: 20, hours: 8));

        var result = await _sut.Generate(new[] { Guid.NewGuid() }, new[] { WorkerId });

        Assert.True(result);
        Assert.Empty(_captured.Holidays);
    }

    private void SetupHoliday(RegularWageWorker wages) =>
        _payStubRepository
            .Setup(r => r.GetWorkerRegularWages(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<IEnumerable<DateTime>>()))
            .ReturnsAsync(wages);

    private void SetupWindow(params TimeSheetApprovedPayrollModel[] timesheets) =>
        _timeSheetRepository
            .Setup(r => r.GetApprovedTimeSheetsInRange(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(timesheets.ToList());

    private static TimeSheetApprovedPayrollModel WorkedDay(decimal rate, double hours)
    {
        var date = new DateTime(2026, 05, 19);
        return new TimeSheetApprovedPayrollModel(
            overtimeStartsAfter: TimeLimits.DefaultTimeLimits.MaxHoursWeek,
            requestId: Guid.NewGuid(),
            workerRate: rate,
            breakIsPaid: false,
            durationBreak: TimeSpan.Zero,
            holidayIsPaid: false,
            workerId: WorkerId,
            workerProfileId: Guid.NewGuid(),
            timeSheetId: Guid.NewGuid(),
            week: 1,
            date: date,
            timeInApproved: date.Date.AddHours(9),
            timeOutApproved: date.Date.AddHours(9 + hours),
            missingHours: TimeSpan.Zero,
            missingHoursOvertime: TimeSpan.Zero,
            missingRateWorker: 0,
            isHoliday: false,
            bonusOrOthers: 0,
            bonusOrOthersDescription: null,
            deductionsOthers: 0,
            deductionsOthersDescription: null,
            reimbursements: 0,
            reimbursementsDescription: null)
        {
            TypeOfWork = "Lab Technician",
            CountryCode = "CA"
        };
    }
}
