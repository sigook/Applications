using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Agencies;
using Covenant.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using AgencyEntity = Covenant.Common.Entities.Agency.Agency;
using AgencyPersonnelEntity = Covenant.Common.Entities.Agency.AgencyPersonnel;

namespace Covenant.Tests.Agencies;

public class AgencyRepositoryTest
{
    private readonly AgencyRepository _sut;
    private readonly CovenantContext _context;

    public AgencyRepositoryTest()
    {
        DbContextOptions<CovenantContext> options = new DbContextOptionsBuilder<CovenantContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _context = new CovenantContext(options);
        var mockFilesConfiguration = new Mock<IOptions<FilesConfiguration>>();
        mockFilesConfiguration.Setup(m => m.Value).Returns(new FilesConfiguration());
        _sut = new AgencyRepository(_context, mockFilesConfiguration.Object);
    }

    private static AgencyEntity Agency(string name, string phone, AgencyType type, Guid? parentId = null) =>
        new(name, phone) { Id = Guid.NewGuid(), AgencyType = type, AgencyParentId = parentId };

    private async Task<AgencyPersonnelEntity> SeedPersonnel(Guid agencyId, string email, params AgencyEntity[] agencies)
    {
        var user = new User(CvnEmail.Create(email).Value, Guid.NewGuid());
        var personnel = AgencyPersonnelEntity.CreatePrimary(agencyId, user);
        await _context.Agencies.AddRangeAsync(agencies);
        await _context.AgencyPersonnel.AddAsync(personnel);
        await _context.SaveChangesAsync();
        return personnel;
    }

    [Fact]
    public async Task GetAgencyContextForUser_MasterAgency_ReturnsAllChildAgencies()
    {
        var master = Agency("Master Agency", "1234567890", AgencyType.Master);
        var child1 = Agency("Child Agency 1", "1234567891", AgencyType.Regular, master.Id);
        var child2 = Agency("Child Agency 2", "1234567892", AgencyType.BusinessPartner, master.Id);
        var personnel = await SeedPersonnel(master.Id, "master@test.com", master, child1, child2);

        var result = await _sut.GetAgencyContextForUser(personnel.UserId);

        Assert.Equal(master.Id, result.AgencyId);
        Assert.Equal(personnel.Id, result.AgencyPersonnelId);
        Assert.Equal(3, result.AgencyIds.Count);
        Assert.Equal(master.Id, result.AgencyIds[0]);
        Assert.Contains(child1.Id, result.AgencyIds);
        Assert.Contains(child2.Id, result.AgencyIds);
    }

    [Theory]
    [InlineData(AgencyType.Regular)]
    [InlineData(AgencyType.BusinessPartner)]
    public async Task GetAgencyContextForUser_ChildAgency_ReturnsOnlyOwnAgency(AgencyType type)
    {
        var master = Agency("Master Agency", "1234567890", AgencyType.Master);
        var child = Agency("Child Agency", "1234567891", type, master.Id);
        var personnel = await SeedPersonnel(child.Id, "child@test.com", master, child);

        var result = await _sut.GetAgencyContextForUser(personnel.UserId);

        Assert.Equal(child.Id, result.AgencyId);
        Assert.Equal(personnel.Id, result.AgencyPersonnelId);
        Assert.Equal([child.Id], result.AgencyIds);
    }

    [Fact]
    public async Task GetAgencyContextForUser_UserWithNoAgency_ReturnsNull()
    {
        var result = await _sut.GetAgencyContextForUser(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAgencyContextForUser_MasterAgencyWithNoChildren_ReturnsOnlyMasterAgency()
    {
        var master = Agency("Master Agency", "1234567890", AgencyType.Master);
        var personnel = await SeedPersonnel(master.Id, "master@test.com", master);

        var result = await _sut.GetAgencyContextForUser(personnel.UserId);

        Assert.Equal([master.Id], result.AgencyIds);
    }
}
