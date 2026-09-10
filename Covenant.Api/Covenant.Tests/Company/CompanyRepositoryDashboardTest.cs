using Covenant.Common.Configuration;
using Covenant.Common.Entities.Company;
using Covenant.Common.Enums;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Company;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Covenant.Tests.Company;

public class CompanyRepositoryDashboardTest
{
    private readonly CompanyRepository _sut;
    private readonly CovenantContext _context;

    private readonly Guid _agencyId = Guid.NewGuid();
    private readonly Guid _otherAgencyId = Guid.NewGuid();
    private readonly Guid _companyProfileId = Guid.NewGuid();
    private readonly Guid _otherCompanyProfileId = Guid.NewGuid();
    private readonly Guid _ownerId = Guid.NewGuid();
    private readonly Guid _otherOwnerId = Guid.NewGuid();

    // Sunday 2026-09-06 through Saturday 2026-09-12, as UTC instants.
    private static readonly DateTime FromUtc = new(2026, 9, 6, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime ToUtcExclusive = new(2026, 9, 13, 0, 0, 0, DateTimeKind.Utc);

    public CompanyRepositoryDashboardTest()
    {
        DbContextOptions<CovenantContext> options = new DbContextOptionsBuilder<CovenantContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _context = new CovenantContext(options);
        var mockFilesConfiguration = new Mock<IOptions<FilesConfiguration>>();
        mockFilesConfiguration.Setup(m => m.Value).Returns(new FilesConfiguration());
        _sut = new CompanyRepository(_context, mockFilesConfiguration.Object);

        _context.CompanyProfiles.AddRange(
            new CompanyProfile { Id = _companyProfileId, AgencyId = _agencyId, FullName = "Acme" },
            new CompanyProfile { Id = _otherCompanyProfileId, AgencyId = _otherAgencyId, FullName = "Other agency client" });
        _context.SaveChanges();
    }

    private void AddDeal(DateTime date, DealStatus status, decimal value, Guid? ownerId = null, Guid? companyProfileId = null) =>
        _context.Deals.Add(new Deal("Deal", ownerId ?? _ownerId, companyProfileId ?? _companyProfileId, date, value, DealType.Temporal, status, null));

    private void AddInteraction(DateTime createdAt, InteractionType type, Guid? ownerId = null, Guid? companyProfileId = null) =>
        _context.CompanyInteractions.Add(new CompanyInteraction("Note", ownerId ?? _ownerId, companyProfileId ?? _companyProfileId,
            InteractionPurpose.Intro, type, InteractionStatus.Completed)
        {
            CreatedAt = createdAt
        });

    [Fact]
    public async Task GetDealsByStatusGroupsAndSumsInsideTheWindow()
    {
        AddDeal(new DateTime(2026, 9, 7, 12, 0, 0, DateTimeKind.Utc), DealStatus.Accepted, 1000m);
        AddDeal(new DateTime(2026, 9, 8, 12, 0, 0, DateTimeKind.Utc), DealStatus.Accepted, 500m);
        AddDeal(new DateTime(2026, 9, 9, 12, 0, 0, DateTimeKind.Utc), DealStatus.ToSend, 200m);
        await _context.SaveChangesAsync();

        var result = await _sut.GetDealsByStatus(_agencyId, null, FromUtc, ToUtcExclusive, []);

        Assert.Equal(2, result.Count);
        var accepted = result.Single(r => r.Status == DealStatus.Accepted);
        Assert.Equal(2, accepted.Count);
        Assert.Equal(1500m, accepted.TotalValue);
        Assert.Equal(200m, result.Single(r => r.Status == DealStatus.ToSend).TotalValue);
    }

    [Fact]
    public async Task GetDealsByStatusIsLeftInclusiveAndRightExclusive()
    {
        AddDeal(FromUtc, DealStatus.Sent, 10m);
        AddDeal(ToUtcExclusive, DealStatus.Sent, 20m);
        AddDeal(FromUtc.AddSeconds(-1), DealStatus.Sent, 40m);
        await _context.SaveChangesAsync();

        var result = await _sut.GetDealsByStatus(_agencyId, null, FromUtc, ToUtcExclusive, []);

        Assert.Equal(10m, Assert.Single(result).TotalValue);
    }

    [Fact]
    public async Task GetDealsByStatusExcludesOtherAgencies()
    {
        AddDeal(new DateTime(2026, 9, 7, 12, 0, 0, DateTimeKind.Utc), DealStatus.Sent, 10m);
        AddDeal(new DateTime(2026, 9, 7, 12, 0, 0, DateTimeKind.Utc), DealStatus.Sent, 99m, companyProfileId: _otherCompanyProfileId);
        await _context.SaveChangesAsync();

        var result = await _sut.GetDealsByStatus(_agencyId, null, FromUtc, ToUtcExclusive, []);

        Assert.Equal(10m, Assert.Single(result).TotalValue);
    }

    [Fact]
    public async Task GetDealsByStatusFiltersByOwnerAndStatuses()
    {
        AddDeal(new DateTime(2026, 9, 7, 12, 0, 0, DateTimeKind.Utc), DealStatus.Accepted, 10m);
        AddDeal(new DateTime(2026, 9, 7, 12, 0, 0, DateTimeKind.Utc), DealStatus.Rejected, 20m);
        AddDeal(new DateTime(2026, 9, 7, 12, 0, 0, DateTimeKind.Utc), DealStatus.Accepted, 99m, ownerId: _otherOwnerId);
        await _context.SaveChangesAsync();

        var result = await _sut.GetDealsByStatus(_agencyId, _ownerId, FromUtc, ToUtcExclusive, [DealStatus.Accepted]);

        var row = Assert.Single(result);
        Assert.Equal(DealStatus.Accepted, row.Status);
        Assert.Equal(1, row.Count);
        Assert.Equal(10m, row.TotalValue);
    }

    [Fact]
    public async Task GetInteractionsByTypeGroupsInsideTheWindow()
    {
        AddInteraction(new DateTime(2026, 9, 7, 12, 0, 0, DateTimeKind.Utc), InteractionType.Call);
        AddInteraction(new DateTime(2026, 9, 8, 12, 0, 0, DateTimeKind.Utc), InteractionType.Call);
        AddInteraction(new DateTime(2026, 9, 9, 12, 0, 0, DateTimeKind.Utc), InteractionType.Mail);
        AddInteraction(ToUtcExclusive, InteractionType.Mail);
        AddInteraction(new DateTime(2026, 9, 9, 12, 0, 0, DateTimeKind.Utc), InteractionType.Sms, companyProfileId: _otherCompanyProfileId);
        await _context.SaveChangesAsync();

        var result = await _sut.GetInteractionsByType(_agencyId, null, FromUtc, ToUtcExclusive);

        Assert.Equal(2, result.Count);
        Assert.Equal(2, result.Single(r => r.Type == InteractionType.Call).Count);
        Assert.Equal(1, result.Single(r => r.Type == InteractionType.Mail).Count);
    }

    [Fact]
    public async Task GetInteractionsByTypeFiltersByOwner()
    {
        AddInteraction(new DateTime(2026, 9, 7, 12, 0, 0, DateTimeKind.Utc), InteractionType.Call);
        AddInteraction(new DateTime(2026, 9, 7, 12, 0, 0, DateTimeKind.Utc), InteractionType.Call, ownerId: _otherOwnerId);
        await _context.SaveChangesAsync();

        var result = await _sut.GetInteractionsByType(_agencyId, _ownerId, FromUtc, ToUtcExclusive);

        Assert.Equal(1, Assert.Single(result).Count);
    }
}
