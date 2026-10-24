using Covenant.Common.Entities;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models;
using Covenant.Common.Models.Candidates;
using Covenant.Common.Repositories;
using Covenant.Core.BL.Adapters;
using Covenant.Core.BL.Interfaces.Shared;
using Covenant.Tests.Utils;
using Moq;
using Xunit;

namespace Covenant.Tests.Candidates;

public class CandidateAdapterTest
{
    private static readonly Guid Female = Guid.NewGuid();
    private static readonly Guid Referral = Guid.NewGuid();
    private static readonly Guid Other = Guid.NewGuid();
    private const string ResumeUrl = "https://teams.example.com/files/resume.pdf";

    private readonly Mock<ICatalogRepository> _catalogRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly Mock<ITeamsService> _teamsService = new();
    private readonly Mock<IFilesContainer> _filesContainer = new();
    private readonly Mock<ISkillService> _skillService = new();
    private readonly CandidateAdapter _sut;

    public CandidateAdapterTest()
    {
        _catalogRepository.Setup(c => c.GetGender()).ReturnsAsync([new BaseModel<Guid>(Female, "Female")]);
        _catalogRepository.Setup(c => c.GetSources()).ReturnsAsync([new BaseModel<Guid>(Referral, "Referral"), new BaseModel<Guid>(Other, "Other")]);
        _skillService.Setup(s => s.Canonicalize(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync((IEnumerable<string> skills) => skills.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).Distinct().ToList());
        _sut = new CandidateAdapter(_catalogRepository.Object, _currentUserService.Object, _teamsService.Object, _filesContainer.Object, _skillService.Object);
    }

    private static CandidateCsvModel Csv(string source = "referral", string urlResume = null) => new()
    {
        FullName = "Jane Candidate",
        Phone = "4165550000",
        Address = "1 Main St",
        Skills = "Forklift, Welding",
        Status = "Citizen",
        Gender = "female",
        Source = source,
        UrlResume = urlResume
    };

    [Fact]
    public async Task ConvertCandidateCsvToCandidateBulk_WithoutRequestOrResume_MapsCandidate()
    {
        var agencyId = Guid.NewGuid();

        var result = await _sut.ConvertCandidateCsvToCandidateBulk(Csv(), agencyId, null);

        Assert.Equal("Jane Candidate", result.Candidate.Name);
        Assert.Equal(Female, result.Candidate.GenderId);
        Assert.Equal(Referral, result.Candidate.SourceId);
        Assert.Equal("Citizen", result.Candidate.ResidencyStatus);
        Assert.Equal("4165550000", result.CandidatesPhone.PhoneNumber);
        Assert.Equal(["Forklift", "Welding"], result.CandidatesSkills.Select(s => s.Skill));
        Assert.Null(result.RequestApplicant);
        Assert.Null(result.CandidatesDocument);
        _teamsService.Verify(t => t.GetTeamsFile(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ConvertCandidateCsvToCandidateBulk_UnknownSource_FallsBackToOther()
    {
        var result = await _sut.ConvertCandidateCsvToCandidateBulk(Csv(source: "Billboard"), Guid.NewGuid(), null);

        Assert.Equal(Other, result.Candidate.SourceId);
    }

    [Fact]
    public async Task ConvertCandidateCsvToCandidateBulk_WithRequestAndResume_AddsApplicantAndDocument()
    {
        var request = FakeData.FakeRequest();
        _teamsService.Setup(t => t.GetTeamsFile(ResumeUrl)).ReturnsAsync(new MemoryStream([1, 2, 3]));
        _filesContainer.Setup(f => f.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>())).ReturnsAsync(Result.Ok());

        var result = await _sut.ConvertCandidateCsvToCandidateBulk(Csv(urlResume: ResumeUrl), Guid.NewGuid(), request);

        Assert.Equal(request.Id, result.RequestApplicant.RequestId);
        Assert.Equal(result.Candidate.Id, result.RequestApplicant.CandidateId);
        Assert.Equal(result.Candidate.Id, result.CandidatesDocument.CandidateId);
        Assert.StartsWith("Resume_", result.CandidatesDocument.Document.FileName);
        Assert.EndsWith(".pdf", result.CandidatesDocument.Document.FileName);
    }

    [Fact]
    public async Task ConvertCandidateCsvToCandidateBulk_ResumeUploadFails_SkipsDocument()
    {
        _teamsService.Setup(t => t.GetTeamsFile(ResumeUrl)).ReturnsAsync(new MemoryStream([1]));
        _filesContainer.Setup(f => f.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>())).ReturnsAsync(Result.Fail(new ResultError("Upload", "upload failed")));

        var result = await _sut.ConvertCandidateCsvToCandidateBulk(Csv(urlResume: ResumeUrl), Guid.NewGuid(), null);

        Assert.Null(result.CandidatesDocument);
    }

    [Fact]
    public void ConvertCandidateModelToCandidate_InvalidPostalCode_Fails()
    {
        var result = _sut.ConvertCandidateModelToCandidate(new CandidateCreateModel { Name = "Jane", PostalCode = "invalid" }, Guid.NewGuid());

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void ConvertCandidateModelToCandidate_InvalidEmail_Fails()
    {
        var result = _sut.ConvertCandidateModelToCandidate(new CandidateCreateModel { Name = "Jane", Email = "not-an-email" }, Guid.NewGuid());

        Assert.False(result.IsSuccess);
    }
}
