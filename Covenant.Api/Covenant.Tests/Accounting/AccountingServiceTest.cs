using Covenant.Common.Interfaces;
using Covenant.Common.Models.Accounting.PayStubs;
using Covenant.Common.Models.Accounting.Subcontractors;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Accounting.PayStubs;
using Covenant.Common.Repositories.Accounting.Subcontractors;
using Covenant.Core.BL.Services.Accounting;
using Covenant.Documents.Services;
using MediatR;
using Moq;
using Xunit;

namespace Covenant.Tests.Accounting;

public class AccountingServiceTest
{
    private static readonly Guid AgencyId = Guid.NewGuid();

    private readonly Mock<IPayStubRepository> _payStubRepository = new();
    private readonly Mock<ISubcontractorRepository> _subcontractorRepository = new();
    private readonly Mock<IMediator> _mediator = new();
    private readonly AccountingService _sut;

    public AccountingServiceTest()
    {
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(c => c.GetAgencyId()).Returns(AgencyId);
        currentUser.Setup(c => c.GetAgencyIds()).Returns([AgencyId]);
        _sut = new AccountingService(currentUser.Object, _payStubRepository.Object, _subcontractorRepository.Object, _mediator.Object);
    }

    [Fact]
    public async Task GetWeeklyPayrollGroupByPaymentDate_IsScopedToCurrentAgencies()
    {
        var pagination = new Pagination();
        var expected = new PaginatedList<WeeklyPayrollModel>();
        _payStubRepository.Setup(r => r.GetWeeklyPayrollGroupByPaymentDate(It.Is<IEnumerable<Guid>>(ids => ids.Single() == AgencyId), pagination))
            .ReturnsAsync(expected);

        var result = await _sut.GetWeeklyPayrollGroupByPaymentDate(pagination);

        Assert.Same(expected, result);
    }

    [Fact]
    public async Task GetSubcontractors_IsScopedToCurrentAgency()
    {
        var pagination = new Pagination();
        var expected = new PaginatedList<PayrollSubContractorListModel>();
        _subcontractorRepository.Setup(r => r.GetPayrollsSubcontractor(AgencyId, pagination)).ReturnsAsync(expected);

        var result = await _sut.GetSubcontractors(pagination);

        Assert.Same(expected, result);
    }

    [Theory]
    [InlineData("not-a-date")]
    [InlineData("")]
    [InlineData(null)]
    public async Task GetWeeklyPayrollGroupByPaymentDateFile_InvalidDate_Fails(string weekEnding)
    {
        var result = await _sut.GetWeeklyPayrollGroupByPaymentDateFile(weekEnding);

        Assert.False(result);
        _payStubRepository.Verify(r => r.GetWeeklyPayrollDetailByPaymentDate(It.IsAny<DateTime>()), Times.Never);
    }

    [Fact]
    public async Task GetWeeklyPayrollGroupByPaymentDateFile_ValidDate_GeneratesReport()
    {
        var document = new ResultGenerateDocument<byte[]>([1], "payroll.xlsx", string.Empty);
        _payStubRepository.Setup(r => r.GetWeeklyPayrollDetailByPaymentDate(new DateTime(2026, 03, 07))).ReturnsAsync([]);
        _mediator.Setup(m => m.Send(It.IsAny<GeneratePaymentReport>(), It.IsAny<CancellationToken>())).ReturnsAsync(document);

        var result = await _sut.GetWeeklyPayrollGroupByPaymentDateFile("2026-03-07");

        Assert.True(result);
        Assert.Same(document, result.Value);
    }

    [Theory]
    [InlineData("not-a-date")]
    [InlineData(null)]
    public async Task GetSubcontractorFile_InvalidDate_Fails(string weekEnding)
    {
        var result = await _sut.GetSubcontractorFile(weekEnding);

        Assert.False(result);
        _subcontractorRepository.Verify(r => r.GetReportsSubcontractorSummary(It.IsAny<DateTime>()), Times.Never);
    }

    [Fact]
    public async Task GetSubcontractorFile_ValidDate_GeneratesReport()
    {
        var document = new ResultGenerateDocument<byte[]>([1], "subcontractors.xlsx", string.Empty);
        _subcontractorRepository.Setup(r => r.GetReportsSubcontractorSummary(new DateTime(2026, 03, 07))).ReturnsAsync([]);
        _mediator.Setup(m => m.Send(It.IsAny<GenerateSubcontractorReport>(), It.IsAny<CancellationToken>())).ReturnsAsync(document);

        var result = await _sut.GetSubcontractorFile("2026-03-07");

        Assert.True(result);
        Assert.Same(document, result.Value);
    }
}
