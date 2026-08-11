using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.PayStub;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Core.BL.Services;
using Covenant.Core.BL.Services.Accounting.Shared;
using MediatR;
using Moq;
using Xunit;

namespace Covenant.Tests.Billing.Holidays;

public class GeneratePayStubHolidayPayTest
{
    private static readonly DateTime Holiday = new(2026, 05, 18);
    private static readonly Guid WorkerId = Guid.NewGuid();

    private readonly Mock<IPayStubRepository> _payStubRepository = new();
    private readonly Mock<ITimesheetRepository> _timeSheetRepository = new();
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
            _timeSheetRepository.Object,
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
            Mock.Of<IPayStubsContainer>(),
            Mock.Of<IRazorViewToStringRenderer>(),
            Mock.Of<IPdfGeneratorService>(),
            Mock.Of<IEmailService>(),
            Mock.Of<IIdentityServerService>(),
            Mock.Of<ISkipPayrollNumberRepository>(),
            Mock.Of<IMediator>(),
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
        Assert.Equal(8.32m, holidayPay.Amount);
    }

    [Fact]
    public async Task Excludes_Overtime_Pay_From_The_Holiday_Pay_Base()
    {
        SetupHoliday(new RegularWageWorker { IsEntitledToReceiveHolidayPay = true });
        var requestId = Guid.NewGuid();
        SetupWindow(Enumerable.Range(0, 6)
            .Select(day => WorkedDay(rate: 20, hours: 8, date: new DateTime(2026, 04, 20).AddDays(day), requestId: requestId))
            .ToArray());

        var result = await _sut.Generate(new[] { Guid.NewGuid() }, new[] { WorkerId });

        Assert.True(result);
        var holidayPay = Assert.Single(_captured.Holidays);
        Assert.Equal(46m, holidayPay.Amount);
    }

    [Fact]
    public async Task Excludes_Worked_Holiday_Premium_From_The_Holiday_Pay_Base()
    {
        SetupHoliday(new RegularWageWorker { IsEntitledToReceiveHolidayPay = true });
        SetupWindow(WorkedDay(rate: 20, hours: 8, isHoliday: true));

        var result = await _sut.Generate(new[] { Guid.NewGuid() }, new[] { WorkerId });

        Assert.True(result);
        var holidayPay = Assert.Single(_captured.Holidays);
        Assert.Equal(0.48m, holidayPay.Amount);
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

    private static TimeSheetApprovedPayrollModel WorkedDay(
        decimal rate,
        double hours,
        DateTime? date = null,
        Guid? requestId = null,
        bool isHoliday = false)
    {
        var day = date ?? new DateTime(2026, 05, 19);
        return new TimeSheetApprovedPayrollModel(
            overtimeStartsAfter: TimeLimits.DefaultTimeLimits.MaxHoursWeek,
            requestId: requestId ?? Guid.NewGuid(),
            workerRate: rate,
            breakIsPaid: false,
            durationBreak: TimeSpan.Zero,
            holidayIsPaid: isHoliday,
            workerId: WorkerId,
            workerProfileId: Guid.NewGuid(),
            timeSheetId: Guid.NewGuid(),
            week: 1,
            date: day,
            timeInApproved: day.Date.AddHours(9),
            timeOutApproved: day.Date.AddHours(9 + hours),
            missingHours: TimeSpan.Zero,
            missingHoursOvertime: TimeSpan.Zero,
            missingRateWorker: 0,
            isHoliday: isHoliday,
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
