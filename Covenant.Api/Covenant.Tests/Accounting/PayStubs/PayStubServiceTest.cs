using ClosedXML.Excel;
using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.PayStubs;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces.Adapters;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Accounting.PayStubs;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Notifications;
using Covenant.Common.Models.Requests.TimeSheets;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Accounting.Deductions;
using Covenant.Common.Repositories.Accounting.Invoices;
using Covenant.Common.Repositories.Accounting.PayStubs;
using Covenant.Common.Repositories.Requests;
using Covenant.Common.Repositories.Workers;
using Covenant.Common.Repositories;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Services.Accounting.PayStubs;
using Covenant.Core.BL.Services.Accounting;
using Covenant.Core.BL.Services;
using Covenant.Documents.Services;
using MediatR;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Xunit;

namespace Covenant.Tests.Accounting.PayStubs;

public class PayStubServiceTest
{
    private const decimal Cpp = 10m;
    private const decimal FederalTax = 20m;
    private const decimal ProvincialTax = 5m;

    private static readonly Rates Rates = Rates.DefaultRates;
    private static readonly DateTime Monday = new(2026, 03, 02);
    private static readonly DateTime FakeNow = new(2027, 01, 15, 10, 30, 00);
    private static readonly FakeTimeProvider Clock = new(new DateTimeOffset(FakeNow, TimeSpan.Zero));

    private readonly Mock<IPayStubRepository> _payStubRepository = new();
    private readonly Mock<ITimesheetRepository> _timeSheetRepository = new();
    private readonly Mock<ICatalogRepository> _catalogRepository = new();
    private readonly Mock<IPayStubsContainer> _payStubsContainer = new();
    private readonly Mock<IRazorViewToStringRenderer> _renderer = new();
    private readonly Mock<IEmailService> _emailService = new();
    private readonly Mock<ISkipPayrollNumberRepository> _skipPayrollNumberRepository = new();
    private readonly Mock<IMediator> _mediator = new();
    private readonly Mock<IPayrollDocumentAdapter> _payrollDocumentAdapter = new();
    private readonly PayStubService _sut;
    private readonly List<PayStub> _created = [];

    public PayStubServiceTest()
    {
        var deductions = new Mock<IDeductionsRepository>();
        deductions.Setup(d => d.GetCpp(It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<PayPeriod>())).ReturnsAsync(Cpp);
        deductions.Setup(d => d.GetTax(It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<PayPeriod>(), TaxType.Federal, It.IsAny<TaxCategory>())).ReturnsAsync(FederalTax);
        deductions.Setup(d => d.GetTax(It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<PayPeriod>(), TaxType.Provincial, It.IsAny<TaxCategory>())).ReturnsAsync(ProvincialTax);

        var calculator = new TimesheetCalculatorService(
            deductions.Object,
            Mock.Of<IWorkerRepository>(),
            _timeSheetRepository.Object,
            Rates,
            TimeLimits.DefaultTimeLimits);

        _payStubRepository.Setup(r => r.GetNextPayStubNumbers(1))
            .ReturnsAsync([new NextNumberModel { NextNumber = 7 }]);
        _payStubRepository.Setup(r => r.Create(It.IsAny<PayStub>()))
            .Callback<PayStub>(_created.Add)
            .Returns(Task.CompletedTask);
        _catalogRepository.Setup(r => r.GetHolidaysInWeek(It.IsAny<DateTime>(), It.IsAny<string>()))
            .ReturnsAsync([]);

        _sut = new PayStubService(
            _payStubRepository.Object,
            calculator,
            _timeSheetRepository.Object,
            _catalogRepository.Object,
            _payStubsContainer.Object,
            _renderer.Object,
            _emailService.Object,
            Mock.Of<ICurrentUserService>(),
            _skipPayrollNumberRepository.Object,
            _mediator.Object,
            Rates,
            TimeLimits.DefaultTimeLimits,
            _payrollDocumentAdapter.Object,
            Clock);
    }

    private static decimal Ei(decimal totalEarnings) => decimal.Multiply(totalEarnings, Rates.EmploymentInsurance).DefaultMoneyRound();

    private void SetupTimeSheets(Guid workerId, params TimeSheetApprovedPayrollModel[] timeSheets) =>
        _timeSheetRepository
            .Setup(r => r.GetTimeSheetForCreatingPayStubs(It.IsAny<IEnumerable<Guid>>(), workerId))
            .ReturnsAsync([.. timeSheets]);

    private static TimeSheetApprovedPayrollModel WorkedDay(
        Guid workerId,
        DateTime day,
        double hours,
        decimal rate = 20m,
        Guid? requestId = null,
        bool approved = true,
        decimal bonus = 0,
        string bonusDescription = null,
        decimal reimbursement = 0,
        string reimbursementDescription = null,
        decimal deduction = 0,
        string deductionDescription = null) =>
        new(
            overtimeStartsAfter: TimeLimits.DefaultTimeLimits.MaxHoursWeek,
            requestId: requestId ?? Guid.NewGuid(),
            workerRate: rate,
            breakIsPaid: false,
            durationBreak: TimeSpan.Zero,
            holidayIsPaid: false,
            workerId: workerId,
            workerProfileId: workerId,
            timeSheetId: Guid.NewGuid(),
            week: 1,
            date: day,
            timeInApproved: approved ? day.Date.AddHours(8) : null,
            timeOutApproved: approved ? day.Date.AddHours(8 + hours) : null,
            missingHours: TimeSpan.Zero,
            missingHoursOvertime: TimeSpan.Zero,
            missingRateWorker: 0,
            isHoliday: false,
            bonusOrOthers: bonus,
            bonusOrOthersDescription: bonusDescription,
            deductionsOthers: deduction,
            deductionsOthersDescription: deductionDescription,
            reimbursements: reimbursement,
            reimbursementsDescription: reimbursementDescription)
        {
            TypeOfWork = "General Labour",
            CountryCode = "CA"
        };

    private static CreatePayStubModel ManualModel(params CreatePayStubItemModel[] items) => new()
    {
        WorkerProfileId = Guid.NewGuid(),
        Position = "General Labour",
        WorkBegins = Monday,
        WorkEnd = Monday.AddDays(4),
        Items = [.. items]
    };

    private static CreatePayStubItemModel Item(PayStubItemType type, double quantity, decimal unitPrice, string description = null) =>
        new() { Type = type, Quantity = quantity, UnitPrice = unitPrice, Description = description };

    [Fact]
    public async Task Generate_WithoutApprovedTimeSheets_DoesNotCreatePayStub()
    {
        var workerId = Guid.NewGuid();
        SetupTimeSheets(workerId);

        Result result = await _sut.Generate([Guid.NewGuid()], [workerId]);

        Assert.True(result);
        Assert.Empty(_created);
        _payStubRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Generate_RegularDay_ComputesVacationsDeductionsAndNetPay()
    {
        var workerId = Guid.NewGuid();
        SetupTimeSheets(workerId, WorkedDay(workerId, Monday, hours: 8));

        Result result = await _sut.Generate([Guid.NewGuid()], [workerId]);

        Assert.True(result);
        PayStub payStub = Assert.Single(_created);
        PayStubItem item = Assert.Single(payStub.Items);
        Assert.Equal(PayStubItemType.Regular, item.Type);
        Assert.Equal(8, item.Quantity);
        Assert.Equal(20m, item.UnitPrice);
        Assert.Equal(160m, payStub.GrossPayment);
        Assert.Equal(160m, payStub.RegularWage);
        Assert.Equal(6.40m, payStub.Vacations);
        Assert.Equal(166.40m, payStub.TotalEarnings);
        Assert.Equal(Cpp, payStub.Cpp);
        Assert.Equal(Ei(166.40m), payStub.Ei);
        Assert.Equal(Cpp + Ei(166.40m) + FederalTax + ProvincialTax, payStub.TotalDeductions);
        Assert.Equal(166.40m - payStub.TotalDeductions, payStub.TotalPaid);
        Assert.Single(payStub.WageDetails);
        Assert.Equal(DayOfWeek.Sunday, payStub.DateWorkBegins.DayOfWeek);
        Assert.Equal(DayOfWeek.Saturday, payStub.DateWorkEnd.DayOfWeek);
        Assert.Equal("General Labour", payStub.Position);
        Assert.Equal("PS-0007-27", payStub.PayStubNumber);
        Assert.Equal(FakeNow, payStub.CreatedAt);
        _payStubRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Generate_GroupsHoursByRate()
    {
        var workerId = Guid.NewGuid();
        SetupTimeSheets(workerId,
            WorkedDay(workerId, Monday, hours: 8, rate: 20m),
            WorkedDay(workerId, Monday.AddDays(1), hours: 8, rate: 20m),
            WorkedDay(workerId, Monday.AddDays(2), hours: 4, rate: 25m));

        Result result = await _sut.Generate([Guid.NewGuid()], [workerId]);

        Assert.True(result);
        PayStub payStub = Assert.Single(_created);
        Assert.Equal(2, payStub.Items.Count(i => i.Type == PayStubItemType.Regular));
        Assert.Equal(16, payStub.Items.Single(i => i.UnitPrice == 20m).Quantity);
        Assert.Equal(4, payStub.Items.Single(i => i.UnitPrice == 25m).Quantity);
        Assert.Equal(420m, payStub.GrossPayment);
        Assert.Equal(3, payStub.WageDetails.Count);
    }

    [Fact]
    public async Task Generate_HoursOverWeeklyLimitOnSameRequest_ArePaidAsOvertime()
    {
        var workerId = Guid.NewGuid();
        var requestId = Guid.NewGuid();
        SetupTimeSheets(workerId, [.. Enumerable.Range(0, 6).Select(d => WorkedDay(workerId, Monday.AddDays(d), hours: 8, requestId: requestId))]);

        Result result = await _sut.Generate([Guid.NewGuid()], [workerId]);

        Assert.True(result);
        PayStub payStub = Assert.Single(_created);
        PayStubItem overtime = Assert.Single(payStub.Items, i => i.Type == PayStubItemType.Overtime);
        Assert.Equal(20m * Rates.OverTime, overtime.UnitPrice);
        Assert.Equal(48, payStub.Items.Sum(i => i.Quantity));
        Assert.True(overtime.Quantity > 0);
    }

    [Fact]
    public async Task Generate_BonusAndReimbursement_AreGroupedByDescription_AndReimbursementSkipsDeductions()
    {
        var workerId = Guid.NewGuid();
        SetupTimeSheets(workerId,
            WorkedDay(workerId, Monday, hours: 8, bonus: 30m, bonusDescription: "Shift bonus", reimbursement: 15m, reimbursementDescription: "Parking"),
            WorkedDay(workerId, Monday.AddDays(1), hours: 8, bonus: 20m, bonusDescription: "Shift bonus", reimbursement: 10m, reimbursementDescription: "Parking"));

        Result result = await _sut.Generate([Guid.NewGuid()], [workerId]);

        Assert.True(result);
        PayStub payStub = Assert.Single(_created);
        PayStubItem bonus = Assert.Single(payStub.Items, i => i.Type == PayStubItemType.Other);
        Assert.Equal("Shift bonus", bonus.Description);
        Assert.Equal(50m, bonus.Total);
        PayStubItem reimbursement = Assert.Single(payStub.Items, i => i.Type == PayStubItemType.Reimbursement);
        Assert.Equal("Parking", reimbursement.Description);
        Assert.Equal(25m, reimbursement.Total);
        Assert.Equal(370m, payStub.GrossPayment);
        decimal totalEarnings = (370m + 370m * Rates.Vacations).DefaultMoneyRound();
        Assert.Equal(totalEarnings, payStub.TotalEarnings);
        Assert.Equal((totalEarnings - payStub.TotalDeductions + 25m).DefaultMoneyRound(), payStub.TotalPaid);
    }

    [Fact]
    public async Task Generate_OtherDeductions_AreAddedToTotalDeductions()
    {
        var workerId = Guid.NewGuid();
        SetupTimeSheets(workerId, WorkedDay(workerId, Monday, hours: 8, deduction: 12m, deductionDescription: "Boots"));

        Result result = await _sut.Generate([Guid.NewGuid()], [workerId]);

        Assert.True(result);
        PayStub payStub = Assert.Single(_created);
        PayStubOtherDeduction deduction = Assert.Single(payStub.OtherDeductions);
        Assert.Equal(12m, deduction.Total);
        Assert.Equal(Cpp + Ei(166.40m) + FederalTax + ProvincialTax + 12m, payStub.TotalDeductions);
    }

    [Fact]
    public async Task Generate_OnlyUnapprovedTimeSheets_Fails()
    {
        var workerId = Guid.NewGuid();
        SetupTimeSheets(workerId, WorkedDay(workerId, Monday, hours: 8, approved: false));

        Result result = await _sut.Generate([Guid.NewGuid()], [workerId]);

        Assert.False(result);
        Assert.Empty(_created);
    }

    [Fact]
    public async Task Generate_StopsAtFirstWorkerThatFails()
    {
        var failing = Guid.NewGuid();
        var next = Guid.NewGuid();
        SetupTimeSheets(failing, WorkedDay(failing, Monday, hours: 8, approved: false));
        SetupTimeSheets(next, WorkedDay(next, Monday, hours: 8));

        Result result = await _sut.Generate([Guid.NewGuid()], [failing, next]);

        Assert.False(result);
        _timeSheetRepository.Verify(r => r.GetTimeSheetForCreatingPayStubs(It.IsAny<IEnumerable<Guid>>(), next), Times.Never);
    }

    [Fact]
    public async Task Generate_HolidayInWeek_AddsStatutoryHolidayItem()
    {
        var workerId = Guid.NewGuid();
        var holiday = Monday.AddDays(2);
        SetupTimeSheets(workerId, WorkedDay(workerId, Monday, hours: 8));
        _catalogRepository.Setup(r => r.GetHolidaysInWeek(It.IsAny<DateTime>(), "CA")).ReturnsAsync([holiday]);
        _payStubRepository.Setup(r => r.GetWorkerRegularWages(workerId, holiday, It.IsAny<IEnumerable<DateTime>>()))
            .ReturnsAsync(new RegularWageWorker { IsEntitledToReceiveHolidayPay = true, CustomPublicHolidayValue = 40m });
        _timeSheetRepository.Setup(r => r.GetApprovedTimeSheetsInRange(workerId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync([]);

        Result result = await _sut.Generate([Guid.NewGuid()], [workerId]);

        Assert.True(result);
        PayStub payStub = Assert.Single(_created);
        PayStubItem statutory = Assert.Single(payStub.Items, i => i.Type == PayStubItemType.StatutoryHoliday);
        Assert.Equal(40m, statutory.Total);
        PayStubPublicHoliday publicHoliday = Assert.Single(payStub.Holidays);
        Assert.Equal(holiday, publicHoliday.Holiday);
        Assert.Equal(200m, payStub.GrossPayment);
    }

    [Fact]
    public async Task CreateManualPayStub_WithRegularHours_SavesPayStubWithWeekBoundaries()
    {
        CreatePayStubModel model = ManualModel(Item(PayStubItemType.Regular, 10, 20m));

        Result<PayStub> result = await _sut.CreateManualPayStub(model);

        Assert.True(result);
        PayStub payStub = Assert.Single(_created);
        Assert.Same(payStub, result.Value);
        Assert.Equal(PayStubItem.RegularHoursLabel, Assert.Single(payStub.Items).Description);
        Assert.Equal(200m, payStub.GrossPayment);
        Assert.Equal(0m, payStub.Vacations);
        Assert.Equal(200m, payStub.TotalEarnings);
        Assert.Equal(Monday.AddDays(-1), payStub.DateWorkBegins);
        Assert.Equal(Monday.AddDays(5), payStub.DateWorkEnd);
        Assert.Equal(model.WorkerProfileId, payStub.WorkerProfileId);
        Assert.Equal(200m - payStub.TotalDeductions, payStub.TotalPaid);
        _payStubRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateManualPayStub_PayVacations_AddsFourPercent()
    {
        CreatePayStubModel model = ManualModel(Item(PayStubItemType.Regular, 10, 20m));
        model.PayVacations = true;

        Result<PayStub> result = await _sut.CreateManualPayStub(model);

        Assert.True(result);
        Assert.Equal(8m, result.Value.Vacations);
        Assert.Equal(208m, result.Value.TotalEarnings);
    }

    [Fact]
    public async Task CreateManualPayStub_VacationsItem_IsPaidAsVacationsNotGross()
    {
        CreatePayStubModel model = ManualModel(
            Item(PayStubItemType.Regular, 10, 20m),
            Item(PayStubItemType.Vacations, 1, 50m));

        Result<PayStub> result = await _sut.CreateManualPayStub(model);

        Assert.True(result);
        Assert.Equal(200m, result.Value.GrossPayment);
        Assert.Equal(50m, result.Value.Vacations);
        Assert.Equal(250m, result.Value.TotalEarnings);
    }

    [Fact]
    public async Task CreateManualPayStub_OtherDeductions_AreAddedToTotalDeductions()
    {
        CreatePayStubModel model = ManualModel(Item(PayStubItemType.Regular, 10, 20m));
        model.OtherDeductions = 30m;
        model.OtherDeductionsDescription = "Advance";

        Result<PayStub> result = await _sut.CreateManualPayStub(model);

        Assert.True(result);
        PayStubOtherDeduction deduction = Assert.Single(result.Value.OtherDeductions);
        Assert.Equal(30m, deduction.Total);
        Assert.Equal(Cpp + Ei(200m) + FederalTax + ProvincialTax + 30m, result.Value.TotalDeductions);
    }

    [Fact]
    public async Task CreateManualPayStub_Reimbursement_SkipsGrossAndIsAddedToNetPay()
    {
        CreatePayStubModel model = ManualModel(
            Item(PayStubItemType.Regular, 10, 20m),
            Item(PayStubItemType.Reimbursement, 1, 35m, "Parking"));

        Result<PayStub> result = await _sut.CreateManualPayStub(model);

        Assert.True(result);
        Assert.Equal(200m, result.Value.GrossPayment);
        Assert.Equal(200m, result.Value.TotalEarnings);
        Assert.Equal(200m - result.Value.TotalDeductions + 35m, result.Value.TotalPaid);
    }

    [Fact]
    public async Task CreateManualPayStub_UsesClockForNumberAndCreatedAt()
    {
        Result<PayStub> result = await _sut.CreateManualPayStub(ManualModel(Item(PayStubItemType.Regular, 10, 20m)));

        Assert.True(result);
        Assert.Equal("PS-0007-27", result.Value.PayStubNumber);
        Assert.Equal(FakeNow, result.Value.CreatedAt);
    }

    [Fact]
    public async Task CreateManualPayStub_UsesCustomDescriptionWhenGiven()
    {
        CreatePayStubModel model = ManualModel(Item(PayStubItemType.Overtime, 2, 30m, "Weekend overtime"));

        Result<PayStub> result = await _sut.CreateManualPayStub(model);

        Assert.True(result);
        Assert.Equal("Weekend overtime", Assert.Single(result.Value.Items).Description);
    }

    [Fact]
    public async Task CreateManualPayStub_WithoutPositiveItems_Fails()
    {
        CreatePayStubModel model = ManualModel(Item(PayStubItemType.Regular, 0, 20m));

        Result<PayStub> result = await _sut.CreateManualPayStub(model);

        Assert.False(result);
        Assert.Empty(_created);
    }

    [Fact]
    public async Task CreateManualPayStub_UnknownItemType_Fails()
    {
        CreatePayStubModel model = ManualModel(Item((PayStubItemType)999, 1, 20m));

        Result<PayStub> result = await _sut.CreateManualPayStub(model);

        Assert.False(result);
        Assert.Contains(result.Errors, e => e.Message.Contains("Unknown item type"));
        Assert.Empty(_created);
    }

    [Fact]
    public async Task CreateManualPayStub_StartAfterEnd_Fails()
    {
        CreatePayStubModel model = ManualModel(Item(PayStubItemType.Regular, 10, 20m));
        model.WorkBegins = Monday.AddDays(14);

        Result<PayStub> result = await _sut.CreateManualPayStub(model);

        Assert.False(result);
        Assert.Empty(_created);
    }

    [Fact]
    public async Task GenerateT4_WithoutData_Fails()
    {
        _payStubRepository.Setup(r => r.GetPayStubsByDates(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync([]);

        var result = await _sut.GenerateT4(Monday, Monday.AddMonths(1));

        Assert.False(result);
    }

    [Fact]
    public async Task GenerateT4_WritesDetailAndReportSheets()
    {
        _payStubRepository.Setup(r => r.GetPayStubsByDates(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync([T4Worker()]);

        var result = await _sut.GenerateT4(Monday, Monday.AddMonths(1));

        Assert.True(result);
        Assert.Equal("T4_Resume_2027.xlsx", result.Value.DocumentName);
        using var workbook = new XLWorkbook(new MemoryStream(result.Value.Document));
        Assert.True(workbook.TryGetWorksheet("Detail", out _));
        IXLWorksheet report = workbook.Worksheet("Report");
        Assert.Equal("Jane Doe", report.Cell("B2").GetString());
        Assert.Equal(300m, report.Cell("E2").GetValue<decimal>());
        Assert.Equal(300m - 10m - 4m - 20m - 5m, report.Cell("N2").GetValue<decimal>());
    }

    [Fact]
    public async Task GenerateCraPayroll_WithoutData_Fails()
    {
        _payStubRepository.Setup(r => r.GetPayStubsByDates(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync([]);

        var result = await _sut.GenerateCraPayroll(Monday, Monday.AddMonths(1));

        Assert.False(result);
    }

    [Fact]
    public async Task GenerateCraPayroll_WritesOneRowPerPayStub()
    {
        _payStubRepository.Setup(r => r.GetPayStubsByDates(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync([T4Worker()]);

        var result = await _sut.GenerateCraPayroll(Monday, Monday.AddMonths(1));

        Assert.True(result);
        Assert.Equal("CRA_Payroll_2027.xlsx", result.Value.DocumentName);
        using var workbook = new XLWorkbook(new MemoryStream(result.Value.Document));
        IXLWorksheet sheet = workbook.Worksheet("Report");
        Assert.Equal("Jane Doe", sheet.Cell("B4").GetString());
        Assert.Equal("Acme", sheet.Cell("C4").GetString());
        Assert.Equal("PS-0001-26", sheet.Cell("D4").GetString());
        Assert.Equal(300m - 10m - 4m - 20m - 5m, sheet.Cell("N4").GetValue<decimal>());
    }

    [Fact]
    public async Task GetPayStubPdf_UnknownPayStub_ReturnsNull()
    {
        var id = Guid.NewGuid();
        _payStubRepository.Setup(r => r.GetPayStubDetail(id)).ReturnsAsync((PayStubDetailModel)null);

        PayStubDocument document = await _sut.GetPayStubPdf(id);

        Assert.Null(document);
    }

    [Fact]
    public async Task GetPayStubPdf_CachedBlob_IsReturnedWithoutRegenerating()
    {
        var id = Guid.NewGuid();
        byte[] cached = [1, 2, 3];
        _payStubsContainer.Setup(c => c.Download(id.ToPayStubBlobName())).ReturnsAsync(cached);
        _payStubRepository.Setup(r => r.GetPayStubDetail(id)).ReturnsAsync(Detail(id));

        PayStubDocument document = await _sut.GetPayStubPdf(id);

        Assert.Equal(cached, document.Content);
        Assert.Equal(id.ToPayStubBlobName(), document.FileName);
        _mediator.Verify(m => m.Send(It.IsAny<GeneratePayStubPdf>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetPayStubPdf_MissingBlob_GeneratesAndUploadsPdf()
    {
        var id = Guid.NewGuid();
        byte[] pdf = [9, 9];
        _payStubRepository.Setup(r => r.GetPayStubDetail(id)).ReturnsAsync(Detail(id));
        _mediator.Setup(m => m.Send(It.IsAny<GeneratePayStubPdf>(), It.IsAny<CancellationToken>())).ReturnsAsync(pdf);

        PayStubDocument document = await _sut.GetPayStubPdf(id);

        Assert.Equal(pdf, document.Content);
        _payStubsContainer.Verify(c => c.UploadAsync(pdf, id.ToPayStubBlobName(), "application/pdf", It.IsAny<Dictionary<string, string>>()), Times.Once);
    }

    [Fact]
    public async Task GetPayStubPdf_UploadFailure_StillReturnsPdf()
    {
        var id = Guid.NewGuid();
        byte[] pdf = [9, 9];
        _payStubRepository.Setup(r => r.GetPayStubDetail(id)).ReturnsAsync(Detail(id));
        _mediator.Setup(m => m.Send(It.IsAny<GeneratePayStubPdf>(), It.IsAny<CancellationToken>())).ReturnsAsync(pdf);
        _payStubsContainer.Setup(c => c.UploadAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
            .ThrowsAsync(new InvalidOperationException("storage down"));

        PayStubDocument document = await _sut.GetPayStubPdf(id);

        Assert.Equal(pdf, document.Content);
    }

    [Fact]
    public async Task SendPayStubEmail_UnknownPayStub_ReturnsFailed()
    {
        var id = Guid.NewGuid();

        PayStubEmailResult result = await _sut.SendPayStubEmail(id);

        Assert.False(result.Success);
        Assert.Equal(id, result.PayStubId);
        _emailService.Verify(e => e.SendCovenantEmail(It.IsAny<EmailParams>()), Times.Never);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task SendPayStubEmail_SendsPdfToWorker_AndReportsDelivery(bool delivered)
    {
        var id = Guid.NewGuid();
        _payStubsContainer.Setup(c => c.Download(It.IsAny<string>())).ReturnsAsync([1]);
        _payStubRepository.Setup(r => r.GetPayStubDetail(id)).ReturnsAsync(Detail(id));
        _payrollDocumentAdapter.Setup(a => a.MapToPayrollEmailViewModel(It.IsAny<PayStubDetailModel>()))
            .Returns(new PayrollEmailViewModel { WorkerEmail = "jane@worker.com", WorkerFullName = "Jane Doe", PayrollNumber = "PS-0001-26" });
        _renderer.Setup(r => r.RenderViewToStringAsync(It.IsAny<string>(), It.IsAny<PayrollEmailViewModel>())).ReturnsAsync("<p>body</p>");
        EmailParams sent = null;
        _emailService.Setup(e => e.SendCovenantEmail(It.IsAny<EmailParams>())).Callback<EmailParams>(p => sent = p).ReturnsAsync(delivered);

        PayStubEmailResult result = await _sut.SendPayStubEmail(id);

        Assert.Equal(delivered, result.Success);
        Assert.Equal("Jane Doe", result.WorkerFullName);
        Assert.Equal("PS-0001-26", result.PayrollNumber);
        Assert.Equal("jane@worker.com", sent.Email);
        Assert.Equal("PayStub PS-0001-26", sent.Subject);
        Assert.Equal(EmailSettingName.PayrollCovenant, sent.EmailSettingName);
        Assert.Single(sent.Attachments);
    }

    [Fact]
    public async Task DeletePayStub_RemovesRowsAndCachedBlob()
    {
        var id = Guid.NewGuid();

        await _sut.DeletePayStub(id);

        _payStubRepository.Verify(r => r.DeletePayStubs(It.Is<IEnumerable<Guid>>(ids => ids.Single() == id)), Times.Once);
        _payStubsContainer.Verify(c => c.DeleteFileIfExists(id.ToPayStubBlobName()), Times.Once);
        _payStubRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateSkipPayrollNumber_BlankValue_Fails(string value)
    {
        Result result = await _sut.CreateSkipPayrollNumber(new BaseModel<Guid> { Value = value });

        Assert.False(result);
        _skipPayrollNumberRepository.Verify(r => r.Create(It.IsAny<BaseModel<Guid>>()), Times.Never);
    }

    [Fact]
    public async Task CreateSkipPayrollNumber_NullModel_Fails()
    {
        Result result = await _sut.CreateSkipPayrollNumber(null);

        Assert.False(result);
    }

    [Fact]
    public async Task CreateSkipPayrollNumber_ValidValue_DelegatesToRepository()
    {
        var model = new BaseModel<Guid> { Value = "1234" };
        _skipPayrollNumberRepository.Setup(r => r.Create(model)).ReturnsAsync(Result.Ok());

        Result result = await _sut.CreateSkipPayrollNumber(model);

        Assert.True(result);
        _skipPayrollNumberRepository.Verify(r => r.Create(model), Times.Once);
    }

    private static PayStubDetailModel Detail(Guid id) => new()
    {
        Id = id,
        WorkerProfileId = Guid.NewGuid(),
        PayrollNumberId = 1,
        PayrollNumber = "PS-0001-26",
        EndDate = Monday.AddDays(5)
    };

    private static PayStubT4Model T4Worker() => new()
    {
        FirstName = "Jane",
        LastName = "Doe",
        Sin = "123456789",
        Address = "1 King St",
        City = "Toronto",
        ProvinceCode = "ON",
        PostalCode = "M5H 1A1",
        Phone = "4165550000",
        Items =
        [
            new PayStubT4Base
            {
                PayStubNumber = "PS-0001-26",
                DatePaid = Monday,
                TotalEarnings = 300m,
                CompanyName = "Acme",
                Employer = new PayStubT4Tax { Cpp = 10m, EI = 5.6m },
                Employee = new PayStubT4Tax { Cpp = 10m, EI = 4m, FederalTax = 20m, ProvincialTax = 5m }
            }
        ]
    };
}
