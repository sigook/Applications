using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Repositories.Accounting;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services.Accounting;
using MediatR;
using Moq;
using Xunit;

namespace Covenant.Tests.Accounting;

public class DeleteSubcontractorReportTest
{
    private readonly Mock<ISubcontractorRepository> _subcontractorRepository = new();
    private readonly Mock<IIdentityServerService> _identityServerService = new();
    private readonly IAccountingService _sut;
    private readonly Guid _agencyId = Guid.NewGuid();
    private static readonly DateTime WeekEnding = new(2026, 9, 5);

    public DeleteSubcontractorReportTest()
    {
        _identityServerService.Setup(i => i.GetAgencyId()).Returns(_agencyId);
        _sut = new AccountingService(
            _identityServerService.Object,
            Mock.Of<IPayStubRepository>(),
            _subcontractorRepository.Object,
            Mock.Of<IMediator>());
    }

    [Fact]
    public async Task DeletesTheWeekScopedToTheCallerAgency()
    {
        _subcontractorRepository
            .Setup(r => r.DeleteReportsByWeekEnding(_agencyId, WeekEnding))
            .ReturnsAsync(3);

        Result result = await _sut.DeleteSubcontractorReport("2026-09-05");

        Assert.True(result, result.StringErrors);
        Assert.Empty(result.Errors);
        _subcontractorRepository.Verify(r => r.DeleteReportsByWeekEnding(_agencyId, WeekEnding), Times.Once);
    }

    [Fact]
    public async Task PassesTheParsedDateThroughToTheRepository()
    {
        _subcontractorRepository
            .Setup(r => r.DeleteReportsByWeekEnding(_agencyId, It.IsAny<DateTime>()))
            .ReturnsAsync(1);

        Result result = await _sut.DeleteSubcontractorReport("2026-09-05T13:45:00");

        Assert.True(result, result.StringErrors);
        _subcontractorRepository.Verify(r => r.DeleteReportsByWeekEnding(_agencyId, WeekEnding.AddHours(13).AddMinutes(45)), Times.Once);
    }

    [Theory]
    [InlineData("not-a-date")]
    [InlineData("")]
    [InlineData(null)]
    public async Task FailsWithoutTouchingTheRepositoryWhenTheDateIsInvalid(string weekEnding)
    {
        Result result = await _sut.DeleteSubcontractorReport(weekEnding);

        Assert.False(result);
        Assert.Contains("Invalid date format", result.StringErrors);
        _subcontractorRepository.Verify(r => r.DeleteReportsByWeekEnding(It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Never);
    }

    [Fact]
    public async Task FailsWhenTheWeekHasNoReports()
    {
        _subcontractorRepository
            .Setup(r => r.DeleteReportsByWeekEnding(_agencyId, WeekEnding))
            .ReturnsAsync(0);

        Result result = await _sut.DeleteSubcontractorReport("2026-09-05");

        Assert.False(result);
        Assert.Contains("No subcontractor reports found", result.StringErrors);
    }
}
