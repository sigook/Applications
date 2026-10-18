using Covenant.Common.Entities;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using Covenant.Core.BL.Services.Workers;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Xunit;

namespace Covenant.Tests.Workers;

public class WorkerRequirementServiceTest
{
    private static readonly DateTime Now = new(2021, 01, 01);
    private readonly WorkerRequirementService _sut = new(new FakeTimeProvider(new DateTimeOffset(Now)));

    private static WorkerProfile UsaWorker() => new()
    {
        Location = new Location { City = new City { Province = new Province { Country = new Country { Code = "USA" } } } }
    };

    private static void PatchSin(WorkerProfile worker, string sin = "123-456-789", bool expires = false, DateTime? dueDate = null)
    {
        var sinInfo = new Mock<ISinInformation<CovenantFile>>();
        sinInfo.SetupGet(c => c.SocialInsurance).Returns(sin);
        sinInfo.SetupGet(c => c.SocialInsuranceFile).Returns(new CovenantFile("sin.pdf"));
        sinInfo.SetupGet(c => c.SocialInsuranceExpire).Returns(expires);
        sinInfo.SetupGet(c => c.DueDate).Returns(dueDate);
        worker.PatchSinInformation(sinInfo.Object);
    }

    private static WorkerProfile CompleteUsaWorker()
    {
        var worker = UsaWorker();
        worker.PatchProfileImage(new CovenantFile("worker.png"));
        PatchSin(worker);
        worker.PatchIdentification1("111111111", Guid.NewGuid(), new CovenantFile("id1.pdf"));
        worker.PatchIdentification2("222222222", Guid.NewGuid(), new CovenantFile("id2.pdf"));
        worker.PatchResume(new CovenantFile("resume.pdf"));
        return worker;
    }

    [Fact]
    public void EmptyProfileListsEveryRequirement()
    {
        var missing = _sut.GetMissing(new WorkerProfile());

        Assert.Equal(
        [
            WorkerRequirement.ProfileImage,
            WorkerRequirement.TaxCategory,
            WorkerRequirement.SocialInsuranceNumber,
            WorkerRequirement.SocialInsuranceFile,
            WorkerRequirement.Identification,
        ], missing);
    }

    [Fact]
    public void ResumeIsNotRequired()
    {
        var worker = CompleteUsaWorker();
        worker.RemoveResume();
        Assert.Empty(_sut.GetMissing(worker));
    }

    [Fact]
    public void UsaWorkerDoesNotNeedTaxCategory()
    {
        Assert.DoesNotContain(WorkerRequirement.TaxCategory, _sut.GetMissing(UsaWorker()));
    }

    [Fact]
    public void CanadianWorkerWithTaxCategoryDoesNotNeedIt()
    {
        var worker = new WorkerProfile
        {
            Location = new Location { City = new City { Province = new Province { Country = new Country { Code = "CA" } } } },
            WorkerProfileTaxCategory = new WorkerProfileTaxCategory { FederalCategory = TaxCategory.Cc0 }
        };
        Assert.DoesNotContain(WorkerRequirement.TaxCategory, _sut.GetMissing(worker));
    }

    [Fact]
    public void CompleteProfileHasNothingMissing()
    {
        Assert.Empty(_sut.GetMissing(CompleteUsaWorker()));
    }

    [Fact]
    public void OneCompleteIdentificationIsEnough()
    {
        var worker = CompleteUsaWorker();
        worker.PatchIdentification2(string.Empty, null, null);
        Assert.Empty(_sut.GetMissing(worker));

        worker.PatchIdentification1(string.Empty, null, null);
        worker.PatchIdentification2("222222222", Guid.NewGuid(), new CovenantFile("id2.pdf"));
        Assert.Empty(_sut.GetMissing(worker));
    }

    [Fact]
    public void IdentificationNeedsNumberAndFileOnTheSameSlot()
    {
        var worker = UsaWorker();
        worker.PatchIdentification1(string.Empty, Guid.NewGuid(), new CovenantFile("id1.pdf"));
        worker.PatchIdentification2("222222222", Guid.NewGuid(), null);
        Assert.Contains(WorkerRequirement.Identification, _sut.GetMissing(worker));
    }

    [Fact]
    public void ExpiredSinIsReportedOnlyWhenExpiryIsEnabled()
    {
        var worker = CompleteUsaWorker();
        PatchSin(worker, expires: true, dueDate: Now.AddDays(-1));
        Assert.Equal([WorkerRequirement.SocialInsuranceExpired], _sut.GetMissing(worker));

        PatchSin(worker, expires: true, dueDate: Now.AddDays(1));
        Assert.Empty(_sut.GetMissing(worker));

        PatchSin(worker, expires: false, dueDate: Now.AddDays(-1));
        Assert.Empty(_sut.GetMissing(worker));
    }

    [Fact]
    public void SubcontractorAndContractorHaveNoRequirementsButStillHaveMissingDocuments()
    {
        var worker = new WorkerProfile { IsContractor = true };
        Assert.Empty(_sut.GetMissing(worker));
        Assert.NotEmpty(_sut.GetMissingDocuments(worker));
    }

    [Fact]
    public void DetailModelUsesTheSameRule()
    {
        var model = new WorkerProfileDetailModel
        {
            ProfileImage = new CovenantFileModel { Id = Guid.NewGuid(), FileName = "worker.png" },
            FederalTaxCategory = TaxCategory.Cc0,
            SocialInsurance = "123-456-789",
            SocialInsuranceFile = new CovenantFileModel("sin.pdf"),
            IdentificationNumber1 = "111111111",
            IdentificationNumber2 = "222222222",
        };

        Assert.Equal([WorkerRequirement.Identification], _sut.GetMissing(model));
    }

    [Fact]
    public void DetailModelPlaceholderImageCountsAsMissing()
    {
        var model = new WorkerProfileDetailModel { ProfileImage = new CovenantFileModel { FileName = "worker.png" } };
        Assert.Contains(WorkerRequirement.ProfileImage, _sut.GetMissing(model));
    }
}
