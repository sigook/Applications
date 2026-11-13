using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Agencies;
using Covenant.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

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

    private static User StaffUser(string email, string name) =>
        new User(CvnEmail.Create(email).Value, Guid.NewGuid()).With(u => u.UpdateName(name));

    [Fact]
    public async Task GetPersonnel_ReturnsOnlyRequestedUsersOrderedByName()
    {
        var zoe = StaffUser("zoe@test.com", "Zoe");
        var ann = StaffUser("ann@test.com", "Ann");
        var other = StaffUser("other@test.com", "Bob");
        await _context.Users.AddRangeAsync(zoe, ann, other);
        await _context.SaveChangesAsync();

        var result = await _sut.GetPersonnel([zoe.Id, ann.Id]);

        Assert.Equal(["Ann", "Zoe"], result.Select(p => p.Name));
        Assert.All(result, p => Assert.Equal(p.Id, p.UserId));
    }

    [Fact]
    public async Task GetPersonnel_NoUsers_ReturnsEmpty()
    {
        Assert.Empty(await _sut.GetPersonnel([]));
    }
}
