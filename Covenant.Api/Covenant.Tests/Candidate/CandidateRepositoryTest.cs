using Covenant.Common.Configuration;
using Covenant.Common.Entities.Request;
using Covenant.Common.Enums;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Candidate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using CandidateEntity = Covenant.Common.Entities.Candidate.Candidate;

namespace Covenant.Tests.Candidate;

public class CandidateRepositoryTest
{
    private readonly CandidateRepository _sut;
    private readonly CovenantContext _context;
    private readonly Guid _agencyId = Guid.NewGuid();
    private readonly Guid _requestId = Guid.NewGuid();

    public CandidateRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<CovenantContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _context = new CovenantContext(options);
        var mockFilesConfiguration = new Mock<IOptions<FilesConfiguration>>();
        mockFilesConfiguration.Setup(m => m.Value).Returns(new FilesConfiguration());
        _sut = new CandidateRepository(_context, mockFilesConfiguration.Object);
    }

    private async Task<CandidateEntity> SeedCandidate(
        string email = "candidate@test.com",
        string address = "123 Main St, Toronto, ON",
        bool dnu = false,
        Guid? agencyId = null)
    {
        var candidate = new CandidateEntity(agencyId ?? _agencyId, "Test Candidate")
        {
            Email = email,
            Address = address,
            Dnu = dnu
        };
        _context.Candidates.Add(candidate);
        await _context.SaveChangesAsync();
        return candidate;
    }

    [Fact]
    public async Task ReturnsCandidateWhoseAddressContainsTheCity()
    {
        var candidate = await SeedCandidate();
        var result = await _sut.GetCandidatesAvailableToInvite(_agencyId, _requestId, "Toronto");
        var invited = Assert.Single(result);
        Assert.Equal(candidate.Id, invited.Id);
        Assert.Equal(candidate.Email, invited.Email);
    }

    [Fact]
    public async Task MatchesCityIgnoringCaseAndDiacritics()
    {
        await SeedCandidate(address: "12 Rue Sainte-Catherine, MONTRÉAL");
        var result = await _sut.GetCandidatesAvailableToInvite(_agencyId, _requestId, "Montreal");
        Assert.Single(result);
    }

    [Fact]
    public async Task ExcludesCandidateFromAnotherAgency()
    {
        await SeedCandidate(agencyId: Guid.NewGuid());
        var result = await _sut.GetCandidatesAvailableToInvite(_agencyId, _requestId, "Toronto");
        Assert.Empty(result);
    }

    [Fact]
    public async Task ExcludesDnuCandidate()
    {
        await SeedCandidate(dnu: true);
        var result = await _sut.GetCandidatesAvailableToInvite(_agencyId, _requestId, "Toronto");
        Assert.Empty(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-an-email")]
    public async Task ExcludesCandidateWithInvalidEmail(string email)
    {
        await SeedCandidate(email: email);
        var result = await _sut.GetCandidatesAvailableToInvite(_agencyId, _requestId, "Toronto");
        Assert.Empty(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("456 King St, Hamilton, ON")]
    public async Task ExcludesCandidateWhoseAddressDoesNotContainTheCity(string address)
    {
        await SeedCandidate(address: address);
        var result = await _sut.GetCandidatesAvailableToInvite(_agencyId, _requestId, "Toronto");
        Assert.Empty(result);
    }

    [Fact]
    public async Task ExcludesCandidateAlreadyApplicantOfTheRequest()
    {
        var candidate = await SeedCandidate();
        _context.Add(RequestApplicant.CreateWithCandidate(_requestId, candidate.Id, "Sigook", string.Empty, RequestApplicantStatus.Pending).Value);
        await _context.SaveChangesAsync();
        var result = await _sut.GetCandidatesAvailableToInvite(_agencyId, _requestId, "Toronto");
        Assert.Empty(result);
    }

    [Fact]
    public async Task IncludesCandidateApplicantOfAnotherRequest()
    {
        var candidate = await SeedCandidate();
        _context.Add(RequestApplicant.CreateWithCandidate(Guid.NewGuid(), candidate.Id, "Sigook", string.Empty, RequestApplicantStatus.Pending).Value);
        await _context.SaveChangesAsync();
        var result = await _sut.GetCandidatesAvailableToInvite(_agencyId, _requestId, "Toronto");
        Assert.Single(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ReturnsEmptyWhenTheCityIsBlank(string city)
    {
        await SeedCandidate();
        var result = await _sut.GetCandidatesAvailableToInvite(_agencyId, _requestId, city);
        Assert.Empty(result);
    }
}
