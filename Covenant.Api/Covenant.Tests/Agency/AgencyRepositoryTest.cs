using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Agency;
using Covenant.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using AgencyEntity = Covenant.Common.Entities.Agency.Agency;
using AgencyPersonnelEntity = Covenant.Common.Entities.Agency.AgencyPersonnel;

namespace Covenant.Tests.AgencyTests
{
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

        [Fact]
        public async Task GetAgencyIdsForUser_MasterAgency_ReturnsAllChildAgencies()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User(CvnEmail.Create("master@test.com").Value, userId);

            var masterAgencyId = Guid.NewGuid();
            var masterAgency = new AgencyEntity("Master Agency", "1234567890")
            {
                Id = masterAgencyId,
                AgencyType = AgencyType.Master
            };

            var childAgency1Id = Guid.NewGuid();
            var childAgency1 = new AgencyEntity("Child Agency 1", "1234567891")
            {
                Id = childAgency1Id,
                AgencyType = AgencyType.Regular,
                AgencyParentId = masterAgencyId
            };

            var childAgency2Id = Guid.NewGuid();
            var childAgency2 = new AgencyEntity("Child Agency 2", "1234567892")
            {
                Id = childAgency2Id,
                AgencyType = AgencyType.BusinessPartner,
                AgencyParentId = masterAgencyId
            };

            var agencyPersonnel = AgencyPersonnelEntity.CreatePrimary(masterAgencyId, user);

            await _context.Agencies.AddRangeAsync(masterAgency, childAgency1, childAgency2);
            await _context.AgencyPersonnel.AddAsync(agencyPersonnel);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.GetAgencyIdsForUser(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(masterAgencyId, result);
            Assert.Contains(childAgency1Id, result);
            Assert.Contains(childAgency2Id, result);
        }

        [Fact]
        public async Task GetAgencyIdsForUser_RegularAgency_ReturnsOnlyOwnAgency()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User(CvnEmail.Create("regular@test.com").Value, userId);

            var masterAgencyId = Guid.NewGuid();
            var masterAgency = new AgencyEntity("Master Agency", "1234567890")
            {
                Id = masterAgencyId,
                AgencyType = AgencyType.Master
            };

            var regularAgencyId = Guid.NewGuid();
            var regularAgency = new AgencyEntity("Regular Agency", "1234567891")
            {
                Id = regularAgencyId,
                AgencyType = AgencyType.Regular,
                AgencyParentId = masterAgencyId
            };

            var agencyPersonnel = AgencyPersonnelEntity.CreatePrimary(regularAgencyId, user);

            await _context.Agencies.AddRangeAsync(masterAgency, regularAgency);
            await _context.AgencyPersonnel.AddAsync(agencyPersonnel);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.GetAgencyIdsForUser(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains(regularAgencyId, result);
            Assert.DoesNotContain(masterAgencyId, result);
        }

        [Fact]
        public async Task GetAgencyIdsForUser_BusinessPartnerAgency_ReturnsOnlyOwnAgency()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User(CvnEmail.Create("partner@test.com").Value, userId);

            var masterAgencyId = Guid.NewGuid();
            var masterAgency = new AgencyEntity("Master Agency", "1234567890")
            {
                Id = masterAgencyId,
                AgencyType = AgencyType.Master
            };

            var partnerAgencyId = Guid.NewGuid();
            var partnerAgency = new AgencyEntity("Partner Agency", "1234567891")
            {
                Id = partnerAgencyId,
                AgencyType = AgencyType.BusinessPartner,
                AgencyParentId = masterAgencyId
            };

            var agencyPersonnel = AgencyPersonnelEntity.CreatePrimary(partnerAgencyId, user);

            await _context.Agencies.AddRangeAsync(masterAgency, partnerAgency);
            await _context.AgencyPersonnel.AddAsync(agencyPersonnel);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.GetAgencyIdsForUser(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains(partnerAgencyId, result);
            Assert.DoesNotContain(masterAgencyId, result);
        }

        [Fact]
        public async Task GetAgencyIdsForUser_UserWithNoAgency_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var result = await _sut.GetAgencyIdsForUser(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAgencyIdsForUser_MasterAgencyWithNoChildren_ReturnsOnlyMasterAgency()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User(CvnEmail.Create("master@test.com").Value, userId);

            var masterAgencyId = Guid.NewGuid();
            var masterAgency = new AgencyEntity("Master Agency", "1234567890")
            {
                Id = masterAgencyId,
                AgencyType = AgencyType.Master
            };

            var agencyPersonnel = AgencyPersonnelEntity.CreatePrimary(masterAgencyId, user);

            await _context.Agencies.AddAsync(masterAgency);
            await _context.AgencyPersonnel.AddAsync(agencyPersonnel);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.GetAgencyIdsForUser(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains(masterAgencyId, result);
        }
    }
}
