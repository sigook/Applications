using Covenant.Common.Entities.Workers;
using Covenant.Common.Entities;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Workers;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Workers;
using Covenant.Common.Resources;
using Covenant.Core.BL.Interfaces.Shared;
using Covenant.Core.BL.Services.Workers;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Covenant.Tests.Workers;

public class WorkerProfileUpdateServiceTest
{
    private readonly Mock<IWorkerRepository> _workerRepository = new();
    private readonly Mock<IDocumentService> _documentService = new();
    private readonly Mock<ISkillService> _skillService = new();
    private readonly WorkerProfileUpdateService _sut;
    private readonly WorkerProfile _profile = new(new User(CvnEmail.Create("worker@mail.com").Value), Guid.NewGuid());

    public WorkerProfileUpdateServiceTest()
    {
        _workerRepository.Setup(r => r.GetProfile(It.IsAny<Expression<Func<WorkerProfile, bool>>>()))
            .ReturnsAsync((Expression<Func<WorkerProfile, bool>> condition) => condition.Compile()(_profile) ? _profile : null);
        _sut = new WorkerProfileUpdateService(_workerRepository.Object, _documentService.Object, _skillService.Object);
    }

    private void VerifySaved(Times times)
    {
        _workerRepository.Verify(r => r.Update(_profile), times);
        _workerRepository.Verify(r => r.SaveChangesAsync(), times);
    }

    private static WorkerProfileJobExperienceModel Job(string company = "Acme") => new()
    {
        Company = company,
        Supervisor = "Sam",
        Duties = "Picking",
        StartDate = new DateTime(2024, 01, 01),
        EndDate = new DateTime(2025, 01, 01)
    };

    [Fact]
    public async Task Update_UnknownProfile_FailsWithoutSaving()
    {
        Result result = await _sut.UpdateLanguages(Guid.NewGuid(), []);

        Assert.False(result);
        Assert.Contains(result.Errors, e => e.Message == ApiResources.WorkerNotFound);
        _workerRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateBasicInformation_Valid_PatchesAndSaves()
    {
        var model = new WorkerProfileBasicInformationModel { FirstName = "Ana", LastName = "Lee", BirthDay = new DateTime(1990, 05, 05), HasVehicle = true };

        Result result = await _sut.UpdateBasicInformation(_profile.Id, model);

        Assert.True(result);
        Assert.Equal("Ana", _profile.FirstName);
        Assert.Equal("Lee", _profile.LastName);
        Assert.True(_profile.HasVehicle);
        VerifySaved(Times.Once());
    }

    [Fact]
    public async Task UpdateBasicInformation_Invalid_FailsWithoutSaving()
    {
        var model = new WorkerProfileBasicInformationModel { FirstName = "", LastName = "Lee" };

        Result result = await _sut.UpdateBasicInformation(_profile.Id, model);

        Assert.False(result);
        VerifySaved(Times.Never());
    }

    [Fact]
    public async Task UpdateLanguages_ReplacesSelection_IgnoringEmptyIds()
    {
        Guid english = Guid.NewGuid(), french = Guid.NewGuid();
        await _sut.UpdateLanguages(_profile.Id, [new BaseModel<Guid>(english), new BaseModel<Guid>(Guid.Empty)]);

        Result result = await _sut.UpdateLanguages(_profile.Id, [new BaseModel<Guid>(french)]);

        Assert.True(result);
        Assert.Equal([french], _profile.Languages.Select(l => l.LanguageId));
    }

    [Fact]
    public async Task UpdateSkills_StoresCanonicalizedSkills()
    {
        _skillService.Setup(s => s.Canonicalize(It.Is<IEnumerable<string>>(skills => skills.SequenceEqual(new[] { "forklift ", "picker" }))))
            .ReturnsAsync(["Forklift Operator", "Picker"]);

        Result result = await _sut.UpdateSkills(_profile.Id, ["forklift ", "picker"]);

        Assert.True(result);
        Assert.Equal(["Forklift Operator", "Picker"], _profile.Skills.Select(s => s.Skill));
        VerifySaved(Times.Once());
    }

    [Fact]
    public async Task JobExperience_CanBeAddedUpdatedAndDeleted()
    {
        Assert.True(await _sut.AddJobExperience(_profile.Id, Job()));
        WorkerProfileJobExperience job = Assert.Single(_profile.JobExperiences);

        Assert.True(await _sut.UpdateJobExperience(_profile.Id, job.Id, Job("Globex")));
        Assert.Equal("Globex", Assert.Single(_profile.JobExperiences).Company);

        Assert.True(await _sut.DeleteJobExperience(_profile.Id, job.Id));
        Assert.Empty(_profile.JobExperiences);
        VerifySaved(Times.Exactly(3));
    }

    [Fact]
    public async Task UpdateJobExperience_Unknown_FailsWithoutSaving()
    {
        Result result = await _sut.UpdateJobExperience(_profile.Id, Guid.NewGuid(), Job());

        Assert.False(result);
        Assert.Contains(result.Errors, e => e.Message == "Job experience not found");
        VerifySaved(Times.Never());
    }

    [Fact]
    public async Task DeleteJobExperience_Unknown_FailsWithoutSaving()
    {
        Result result = await _sut.DeleteJobExperience(_profile.Id, Guid.NewGuid());

        Assert.False(result);
        VerifySaved(Times.Never());
    }

    [Fact]
    public async Task AddJobExperience_Invalid_FailsWithoutSaving()
    {
        Result result = await _sut.AddJobExperience(_profile.Id, Job(company: ""));

        Assert.False(result);
        Assert.Empty(_profile.JobExperiences);
        VerifySaved(Times.Never());
    }

    [Fact]
    public async Task DeleteCertificate_RemovesRowAndBlob()
    {
        var certificate = new WorkerProfileCertificate { CertificateId = Guid.NewGuid() };
        _workerRepository.Setup(r => r.GetCertificate(certificate.Id)).ReturnsAsync(certificate);

        Result result = await _sut.DeleteCertificate(certificate.Id);

        Assert.True(result);
        _workerRepository.Verify(r => r.Delete(certificate), Times.Once);
        _workerRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _documentService.Verify(d => d.DeleteFile(certificate.CertificateId), Times.Once);
    }

    [Fact]
    public async Task DeleteLicense_RemovesRowAndBlob()
    {
        var license = new WorkerProfileLicense { LicenseId = Guid.NewGuid() };
        _workerRepository.Setup(r => r.GetLicense(license.Id)).ReturnsAsync(license);

        Result result = await _sut.DeleteLicense(license.Id);

        Assert.True(result);
        _workerRepository.Verify(r => r.Delete(license), Times.Once);
        _documentService.Verify(d => d.DeleteFile(license.LicenseId), Times.Once);
    }

    [Fact]
    public async Task DeleteOtherDocument_RemovesRowAndBlob()
    {
        var document = WorkerProfileOtherDocument.Create(_profile.Id, new CovenantFile("doc.pdf", "Doc")).Value;
        _workerRepository.Setup(r => r.GetOtherDocument(document.Id)).ReturnsAsync(document);

        Result result = await _sut.DeleteOtherDocument(document.Id);

        Assert.True(result);
        _workerRepository.Verify(r => r.Delete(document), Times.Once);
        _documentService.Verify(d => d.DeleteFile(document.DocumentId), Times.Once);
    }

    [Fact]
    public async Task DeleteDocuments_Unknown_FailWithoutTouchingStorage()
    {
        Assert.False(await _sut.DeleteCertificate(Guid.NewGuid()));
        Assert.False(await _sut.DeleteLicense(Guid.NewGuid()));
        Assert.False(await _sut.DeleteOtherDocument(Guid.NewGuid()));

        _workerRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _documentService.Verify(d => d.DeleteFile(It.IsAny<Guid>()), Times.Never);
    }
}
