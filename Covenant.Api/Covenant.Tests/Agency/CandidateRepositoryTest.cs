using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Candidate;
using Covenant.Common.Entities.Request;
using Covenant.Common.Enums;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Candidate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Covenant.Tests.Agency;

public class CandidateRepositoryTest
{
    private readonly CandidateRepository _sut;
    private readonly CovenantContext _context;
    private readonly Guid _agencyId = Guid.NewGuid();
    private readonly Guid _requestId = Guid.NewGuid();

    public CandidateRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<CovenantContext>()
            .UseInMemoryDatabase($"CandidateRepository_{Guid.NewGuid():N}").Options;
        _context = new CovenantContext(options);
        var filesOptions = new Mock<IOptions<FilesConfiguration>>();
        filesOptions.Setup(m => m.Value).Returns(new FilesConfiguration());
        _sut = new CandidateRepository(_context, filesOptions.Object);
    }

    private Candidate AddCandidate(string name, string email, string address, Guid? agencyId = null, bool dnu = false)
    {
        var candidate = new Candidate(agencyId ?? _agencyId, name) { Address = address, Dnu = dnu };
        if (email != null) candidate.AddEmail(CvnEmail.Create(email).Value);
        _context.Candidates.Add(candidate);
        return candidate;
    }

    [Fact]
    public async Task GetCandidatesAvailableToInviteFiltersIneligibleCandidates()
    {
        var eligible = AddCandidate("Eligible", "eligible@mail.com", "25 Bay St, Toronto ON");
        AddCandidate("Dnu", "dnu@mail.com", "25 Bay St, Toronto ON", dnu: true);
        AddCandidate("No Email", null, "25 Bay St, Toronto ON");
        AddCandidate("No Address", "noaddress@mail.com", null);
        AddCandidate("Other Agency", "otheragency@mail.com", "25 Bay St, Toronto ON", agencyId: Guid.NewGuid());
        var applied = AddCandidate("Applied", "applied@mail.com", "25 Bay St, Toronto ON");
        _context.RequestApplicants.Add(RequestApplicant.CreateWithCandidate(_requestId, applied.Id, "Sigook", null, RequestApplicantStatus.Pending).Value);
        await _context.SaveChangesAsync();

        var result = await _sut.GetCandidatesAvailableToInvite(_agencyId, _requestId);

        var single = Assert.Single(result);
        Assert.Equal(eligible.Id, single.Id);
        Assert.Equal(eligible.Name, single.Name);
        Assert.Equal(eligible.Email, single.Email);
        Assert.Equal(eligible.Address, single.Address);
    }

    [Fact]
    public async Task GetCandidatesAvailableToInviteKeepsCandidatesAppliedToOtherRequests()
    {
        var candidate = AddCandidate("Other Request", "otherrequest@mail.com", "25 Bay St, Toronto ON");
        _context.RequestApplicants.Add(RequestApplicant.CreateWithCandidate(Guid.NewGuid(), candidate.Id, "Sigook", null, RequestApplicantStatus.Pending).Value);
        await _context.SaveChangesAsync();

        var result = await _sut.GetCandidatesAvailableToInvite(_agencyId, _requestId);

        Assert.Contains(result, c => c.Id == candidate.Id);
    }
}
