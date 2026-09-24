using Covenant.Api.Validators.Company;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Company.SalesDashboard;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Request;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using Moq;
using Xunit;

namespace Covenant.Tests.Sales;

public class SalesServiceDashboardTest
{
    private readonly Mock<ICompanyRepository> _companyRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly Mock<ITimeService> _timeService = new();
    private readonly ISalesService _sut;
    private readonly Guid _agencyId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();

    // Wednesday 2026-09-09, 15:00 UTC. Week = Sun 2026-09-06 to Sat 2026-09-12, quarter = Q3.
    private static readonly DateTimeOffset Now = new(2026, 9, 9, 15, 0, 0, TimeSpan.Zero);
    private static readonly DateTime WeekFromUtc = new(2026, 9, 6, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime WeekToUtc = new(2026, 9, 13, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime QuarterFromUtc = new(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime QuarterToUtc = new(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc);

    public SalesServiceDashboardTest()
    {
        _currentUserService.Setup(i => i.GetAgencyId()).Returns(_agencyId);
        _currentUserService.Setup(i => i.GetUserId()).Returns(_userId);
        _timeService.Setup(t => t.GetCurrentDateTimeOffset()).Returns(Now);
        _sut = new SalesService(
            Mock.Of<IRequestService>(),
            Mock.Of<IRequestRepository>(),
            _companyRepository.Object,
            _currentUserService.Object,
            Mock.Of<IUploadedFilesService>(),
            Mock.Of<IDocumentService>(),
            new CreateCompanyInteractionModelValidator(),
            new UpdateCompanyInteractionModelValidator(),
            new CreateDealModelValidator(),
            new UpdateDealModelValidator(),
            _timeService.Object,
            new GetDealsByStatusFilterValidator());
    }

    private void SetupDeals(params DealStatusSummaryModel[] rows) =>
        _companyRepository
            .Setup(r => r.GetDealsByStatus(_agencyId, It.IsAny<Guid?>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<List<DealStatus>>()))
            .ReturnsAsync(rows.ToList());

    private void SetupInteractions(params InteractionTypeSummaryModel[] rows) =>
        _companyRepository
            .Setup(r => r.GetInteractionsByType(_agencyId, It.IsAny<Guid?>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(rows.ToList());

    [Fact]
    public async Task GetDealsByStatusScopesToOwnerForSalesUser()
    {
        Guid? capturedOwner = null;
        _companyRepository
            .Setup(r => r.GetDealsByStatus(_agencyId, It.IsAny<Guid?>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<List<DealStatus>>()))
            .Callback<Guid, Guid?, DateTime, DateTime, List<DealStatus>>((_, owner, _, _, _) => capturedOwner = owner)
            .ReturnsAsync([]);

        await _sut.GetDealsByStatus(new GetDealsByStatusFilter { OwnerId = Guid.NewGuid() });

        Assert.Equal(_userId, capturedOwner);
    }

    [Fact]
    public async Task GetDealsByStatusIsNotScopedForAdmin()
    {
        _currentUserService.Setup(i => i.IsAdmin()).Returns(true);
        Guid? capturedOwner = Guid.NewGuid();
        _companyRepository
            .Setup(r => r.GetDealsByStatus(_agencyId, It.IsAny<Guid?>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<List<DealStatus>>()))
            .Callback<Guid, Guid?, DateTime, DateTime, List<DealStatus>>((_, owner, _, _, _) => capturedOwner = owner)
            .ReturnsAsync([]);

        await _sut.GetDealsByStatus(new GetDealsByStatusFilter());

        Assert.Null(capturedOwner);
    }

    [Fact]
    public async Task GetDealsByStatusKeepsRequestedOwnerForAdmin()
    {
        _currentUserService.Setup(i => i.IsAdmin()).Returns(true);
        var requestedOwner = Guid.NewGuid();
        Guid? capturedOwner = null;
        _companyRepository
            .Setup(r => r.GetDealsByStatus(_agencyId, It.IsAny<Guid?>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<List<DealStatus>>()))
            .Callback<Guid, Guid?, DateTime, DateTime, List<DealStatus>>((_, owner, _, _, _) => capturedOwner = owner)
            .ReturnsAsync([]);

        await _sut.GetDealsByStatus(new GetDealsByStatusFilter { OwnerId = requestedOwner });

        Assert.Equal(requestedOwner, capturedOwner);
    }

    [Fact]
    public async Task GetDealsByStatusQueriesTheUtcWeekWindow()
    {
        DateTime capturedFrom = default;
        DateTime capturedTo = default;
        _companyRepository
            .Setup(r => r.GetDealsByStatus(_agencyId, It.IsAny<Guid?>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<List<DealStatus>>()))
            .Callback<Guid, Guid?, DateTime, DateTime, List<DealStatus>>((_, _, from, to, _) =>
            {
                capturedFrom = from;
                capturedTo = to;
            })
            .ReturnsAsync([]);

        var result = await _sut.GetDealsByStatus(new GetDealsByStatusFilter { Period = SalesPeriod.Week });

        Assert.Equal(WeekFromUtc, capturedFrom);
        Assert.Equal(WeekToUtc, capturedTo);
        Assert.Equal(new DateTime(2026, 9, 6), result.Value.Period.From);
        Assert.Equal(new DateTime(2026, 9, 12), result.Value.Period.To);
    }

    [Fact]
    public async Task GetDealsByStatusZeroFillsEveryStatusInEnumOrder()
    {
        SetupDeals(new DealStatusSummaryModel { Status = DealStatus.Accepted, Count = 6, TotalValue = 29500m });

        var result = await _sut.GetDealsByStatus(new GetDealsByStatusFilter());

        Assert.Equal(7, result.Value.Items.Count);
        Assert.Equal(Enum.GetValues<DealStatus>().OrderBy(s => s), result.Value.Items.Select(i => i.Status));
        Assert.Equal(0, result.Value.Items.Single(i => i.Status == DealStatus.ToSend).Count);
        Assert.Equal(6, result.Value.Items.Single(i => i.Status == DealStatus.Accepted).Count);
    }

    [Fact]
    public async Task GetDealsByStatusReturnsOnlyTheRequestedStatusesDeduplicated()
    {
        SetupDeals(
            new DealStatusSummaryModel { Status = DealStatus.ToSend, Count = 3, TotalValue = 12000m },
            new DealStatusSummaryModel { Status = DealStatus.Accepted, Count = 6, TotalValue = 29500m });

        var result = await _sut.GetDealsByStatus(new GetDealsByStatusFilter
        {
            Statuses = [DealStatus.Accepted, DealStatus.ToSend, DealStatus.Accepted]
        });

        Assert.Equal([DealStatus.ToSend, DealStatus.Accepted], result.Value.Items.Select(i => i.Status));
        Assert.Equal(9, result.Value.TotalCount);
        Assert.Equal(41500m, result.Value.TotalValue);
    }

    [Fact]
    public async Task GetDealsByStatusRejectsAPeriodOutsideTheEnum()
    {
        var result = await _sut.GetDealsByStatus(new GetDealsByStatusFilter { Period = (SalesPeriod)99 });

        Assert.False(result);
        _companyRepository.Verify(
            r => r.GetDealsByStatus(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<List<DealStatus>>()),
            Times.Never);
    }

    [Fact]
    public async Task GetDealsByStatusRejectsAStatusOutsideTheEnum()
    {
        var result = await _sut.GetDealsByStatus(new GetDealsByStatusFilter { Statuses = [(DealStatus)42] });

        Assert.False(result);
        _companyRepository.Verify(
            r => r.GetDealsByStatus(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<List<DealStatus>>()),
            Times.Never);
    }

    [Fact]
    public async Task GetDashboardSummaryUsesTheQuarterForPipelineAndTheWeekForActivity()
    {
        DateTime dealsFrom = default;
        DateTime dealsTo = default;
        DateTime interactionsFrom = default;
        DateTime interactionsTo = default;
        _companyRepository
            .Setup(r => r.GetDealsByStatus(_agencyId, It.IsAny<Guid?>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<List<DealStatus>>()))
            .Callback<Guid, Guid?, DateTime, DateTime, List<DealStatus>>((_, _, from, to, _) =>
            {
                dealsFrom = from;
                dealsTo = to;
            })
            .ReturnsAsync([]);
        _companyRepository
            .Setup(r => r.GetInteractionsByType(_agencyId, It.IsAny<Guid?>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Callback<Guid, Guid?, DateTime, DateTime>((_, _, from, to) =>
            {
                interactionsFrom = from;
                interactionsTo = to;
            })
            .ReturnsAsync([]);

        var result = await _sut.GetDashboardSummary(new GetSalesDashboardSummaryFilter());

        Assert.Equal(QuarterFromUtc, dealsFrom);
        Assert.Equal(QuarterToUtc, dealsTo);
        Assert.Equal(WeekFromUtc, interactionsFrom);
        Assert.Equal(WeekToUtc, interactionsTo);
        Assert.Equal("Q3 2026", result.Quarter.Label);
        Assert.Equal("Sep 6 - Sep 12, 2026", result.Week.Label);
        Assert.Equal(Now.UtcDateTime, result.AsOf);
    }

    [Fact]
    public async Task GetDashboardSummaryZeroFillsStatusesAndInteractionTypes()
    {
        SetupDeals(new DealStatusSummaryModel { Status = DealStatus.Sent, Count = 16, TotalValue = 100m });
        SetupInteractions(new InteractionTypeSummaryModel { Type = InteractionType.Mail, Count = 68 });

        var result = await _sut.GetDashboardSummary(new GetSalesDashboardSummaryFilter());

        Assert.Equal(7, result.Pipeline.Count);
        Assert.Equal(4, result.Activity.Count);
        Assert.Equal(Enum.GetValues<InteractionType>().OrderBy(t => t), result.Activity.Select(a => a.Type));
        Assert.Equal(68, result.Activity.Single(a => a.Type == InteractionType.Mail).Count);
        Assert.Equal(0, result.Activity.Single(a => a.Type == InteractionType.Call).Count);
    }

    [Fact]
    public async Task GetDashboardSummaryScopesToOwnerForSalesUser()
    {
        Guid? capturedDealsOwner = null;
        Guid? capturedInteractionsOwner = null;
        _companyRepository
            .Setup(r => r.GetDealsByStatus(_agencyId, It.IsAny<Guid?>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<List<DealStatus>>()))
            .Callback<Guid, Guid?, DateTime, DateTime, List<DealStatus>>((_, owner, _, _, _) => capturedDealsOwner = owner)
            .ReturnsAsync([]);
        _companyRepository
            .Setup(r => r.GetInteractionsByType(_agencyId, It.IsAny<Guid?>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Callback<Guid, Guid?, DateTime, DateTime>((_, owner, _, _) => capturedInteractionsOwner = owner)
            .ReturnsAsync([]);

        await _sut.GetDashboardSummary(new GetSalesDashboardSummaryFilter { OwnerId = Guid.NewGuid() });

        Assert.Equal(_userId, capturedDealsOwner);
        Assert.Equal(_userId, capturedInteractionsOwner);
    }
}
