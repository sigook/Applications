using Covenant.Common.Entities;
using Covenant.Common.Functionals;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Models.Worker;
using Covenant.Common.Utils.Extensions;
using Moq;
using Xunit;

namespace Covenant.Tests.Worker
{
    public class WorkerProfileTest
    {
        [Theory]
        [InlineData((string)null, "")]
        [InlineData("", "")]
        [InlineData("1", "******1")]
        [InlineData("12", "******12")]
        [InlineData("123", "******123")]
        [InlineData("1234", "******1234")]
        [InlineData("12345", "******2345")]
        [InlineData("123456", "******3456")]
        [InlineData("1234567", "******4567")]
        [InlineData("123456789", "******6789")]
        public void MaskSINNumber(string sin, string expected)
        {
            string masked = sin.MaskSIN();
            Assert.Equal(expected, masked);
        }

        [Fact]
        public void DoNotApprovedToWorkIfItDoesNotHaveSinInfo()
        {
            var worker = new WorkerProfile 
            { 
                ApprovedToWork = false,
                Location = new Location
                {
                    City = new City
                    {
                        Province = new Province
                        {
                            Country = new Country
                            {
                                Code = "USA"
                            }
                        }
                    }
                }
            };
            var now = new DateTime(2021, 01, 01);
            Result result = worker.UpdateApprovedToWork(now);
            Assert.False(result);

            var sinInfo = new Mock<ISinInformation<CovenantFile>>();
            sinInfo.SetupGet(c => c.SocialInsurance).Returns(string.Empty);
            worker.PatchSinInformation(sinInfo.Object);
            worker.PatchProfileImage(new CovenantFile("worker.png"));
            result = worker.UpdateApprovedToWork(now);
            Assert.False(result);

            sinInfo.SetupGet(c => c.SocialInsurance).Returns("123-456-789");
            sinInfo.SetupGet(c => c.SocialInsuranceFile).Returns(new CovenantFile("sin.pdf"));
            worker.PatchSinInformation(sinInfo.Object);
            result = worker.UpdateApprovedToWork(now);
            Assert.True(result);
        }

        [Fact]
        public void DoNotApprovedToWorkIfSinHasExpired()
        {
            var worker = new WorkerProfile 
            { 
                ApprovedToWork = false,
                Location = new Location
                {
                    City = new City
                    {
                        Province = new Province
                        {
                            Country = new Country
                            {
                                Code = "USA"
                            }
                        }
                    }
                }
            };
            var now = new DateTime(2021, 01, 01);

            var sinInfo = new Mock<ISinInformation<CovenantFile>>();
            sinInfo.SetupGet(c => c.SocialInsurance).Returns("123-456-789");
            sinInfo.SetupGet(c => c.SocialInsuranceFile).Returns(new CovenantFile("sin.pdf"));
            sinInfo.SetupGet(c => c.SocialInsuranceExpire).Returns(true);
            sinInfo.SetupGet(c => c.DueDate).Returns(now.Subtract(TimeSpan.FromDays(1)));
            worker.PatchSinInformation(sinInfo.Object);
            worker.PatchProfileImage(new CovenantFile("worker.png"));
            Result result = worker.UpdateApprovedToWork(now);
            Assert.False(result);

            sinInfo.SetupGet(c => c.DueDate).Returns(now.AddDays(1));
            worker.PatchSinInformation(sinInfo.Object);
            result = worker.UpdateApprovedToWork(now);
            Assert.True(result);
        }

        [Fact]
        public void IfItIsSubcontractorItCanBeApprovedWithoutSinInfo()
        {
            var now = new DateTime(2021, 01, 01);
            var worker = new WorkerProfile { ApprovedToWork = false };
            worker.UpdateSubcontractor(now, true);
            Result result = worker.UpdateApprovedToWork(now);
            Assert.True(result);
        }

        [Fact]
        public void IfItIsContractorItCanBeApprovedWithoutSinInfo()
        {
            var now = new DateTime(2021, 01, 01);
            var worker = new WorkerProfile { ApprovedToWork = false };
            Result result = worker.UpdateApprovedToWork(now);
            Assert.False(result);

            worker.UpdateContractor(now);
            result = worker.UpdateApprovedToWork(now);
            Assert.True(result);
        }

        [Fact]
        public void IfItIsAlreadyApprovedToWorkItMustChangeToNotApproved()
        {
            var worker = new WorkerProfile { ApprovedToWork = true };
            var now = new DateTime(2021, 01, 01);
            Result result = worker.UpdateApprovedToWork(now);
            Assert.True(result);
            Assert.False(worker.ApprovedToWork);
        }

        [Fact]
        public void PatchIdentification1DoesNotTouchOtherSlots()
        {
            var worker = new WorkerProfile();
            worker.PatchIdentification2("222222222", Guid.NewGuid(), new CovenantFile("id2.pdf"));
            worker.PatchPoliceCheck(new CovenantFile("police.pdf"));
            worker.PatchResume(new CovenantFile("resume.pdf"));

            var typeId = Guid.NewGuid();
            Result result = worker.PatchIdentification1("111111111", typeId, new CovenantFile("id1.pdf"));

            Assert.True(result);
            Assert.Equal("111111111", worker.IdentificationNumber1);
            Assert.Equal(typeId, worker.IdentificationType1Id);
            Assert.Equal("id1.pdf", worker.IdentificationType1File.FileName);
            Assert.Equal("id2.pdf", worker.IdentificationType2File.FileName);
            Assert.Equal("police.pdf", worker.PoliceCheckBackGround.FileName);
            Assert.Equal("resume.pdf", worker.Resume.FileName);
        }

        [Fact]
        public void PatchIdentification1UpdatesExistingFileKeepingId()
        {
            var worker = new WorkerProfile();
            worker.PatchIdentification1("111111111", Guid.NewGuid(), new CovenantFile("id1.pdf"));
            var fileId = worker.IdentificationType1FileId;

            worker.PatchIdentification1("111111111", Guid.NewGuid(), new CovenantFile("id1-v2.pdf"));

            Assert.Equal(fileId, worker.IdentificationType1FileId);
            Assert.Equal("id1-v2.pdf", worker.IdentificationType1File.FileName);
        }

        [Fact]
        public void PatchPoliceCheckSetsFlagOnUpload()
        {
            var worker = new WorkerProfile();
            Result result = worker.PatchPoliceCheck(new CovenantFile("police.pdf"));
            Assert.True(result);
            Assert.True(worker.HavePoliceCheckBackground);
            Assert.Equal("police.pdf", worker.PoliceCheckBackGround.FileName);
        }

        [Fact]
        public void PatchSocialInsuranceDocumentKeepsExpireAndDueDate()
        {
            var dueDate = new DateTime(2030, 01, 01);
            var worker = new WorkerProfile { SocialInsuranceExpire = true, DueDate = dueDate };
            Result result = worker.PatchSocialInsuranceDocument("123-456-789", new CovenantFile("sin.pdf"));
            Assert.True(result);
            Assert.Equal("123-456-789", worker.SocialInsurance);
            Assert.Equal("sin.pdf", worker.SocialInsuranceFile.FileName);
            Assert.True(worker.SocialInsuranceExpire);
            Assert.Equal(dueDate, worker.DueDate);
        }

        [Fact]
        public void PatchSocialInsuranceDocumentFailsWithInvalidLength()
        {
            var worker = new WorkerProfile();
            Result result = worker.PatchSocialInsuranceDocument("123", new CovenantFile("sin.pdf"));
            Assert.False(result);
            Assert.Null(worker.SocialInsuranceFile);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("123")]
        public void PatchSocialInsuranceFromIdentificationFailsWithInvalidNumber(string socialInsurance)
        {
            var worker = new WorkerProfile();
            Result result = worker.PatchSocialInsuranceFromIdentification(socialInsurance, new CovenantFile("id.pdf"));
            Assert.False(result);
            Assert.Null(worker.SocialInsuranceFile);
        }

        [Fact]
        public void PatchSocialInsuranceFromIdentificationReplacesDifferentSin()
        {
            var worker = new WorkerProfile { SocialInsurance = "111-222-333" };
            Result<bool> result = worker.PatchSocialInsuranceFromIdentification("999-888-777", new CovenantFile("id.pdf"));
            Assert.True(result);
            Assert.True(result.Value);
            Assert.Equal("999-888-777", worker.SocialInsurance);
            Assert.Equal("id.pdf", worker.SocialInsuranceFile.FileName);
        }

        [Fact]
        public void PatchSocialInsuranceFromIdentificationUpdatesFileWhenSinMatches()
        {
            var worker = new WorkerProfile { SocialInsurance = "111-222-333" };
            worker.PatchSocialInsuranceDocument("111-222-333", new CovenantFile("old.pdf"));
            Result<bool> result = worker.PatchSocialInsuranceFromIdentification("111-222-333", new CovenantFile("id.pdf"));
            Assert.True(result);
            Assert.False(result.Value);
            Assert.Equal("id.pdf", worker.SocialInsuranceFile.FileName);
        }

        [Fact]
        public void PatchSocialInsuranceFromIdentificationFillsEmptySin()
        {
            var dueDate = new DateTime(2030, 01, 01);
            var worker = new WorkerProfile { SocialInsuranceExpire = true, DueDate = dueDate };
            Result result = worker.PatchSocialInsuranceFromIdentification("123-456-789", new CovenantFile("id.pdf"));
            Assert.True(result);
            Assert.Equal("123-456-789", worker.SocialInsurance);
            Assert.Equal("id.pdf", worker.SocialInsuranceFile.FileName);
            Assert.Equal(worker.SocialInsuranceFile.Id, worker.SocialInsuranceFileId);
            Assert.True(worker.SocialInsuranceExpire);
            Assert.Equal(dueDate, worker.DueDate);
        }

        [Fact]
        public void PatchDocumentsClearsSlotsWhenFileNameIsEmpty()
        {
            var worker = new WorkerProfile();
            worker.PatchIdentification1("111111111", Guid.NewGuid(), new CovenantFile("id1.pdf"));
            worker.PatchPoliceCheck(new CovenantFile("police.pdf"));
            worker.PatchResume(new CovenantFile("resume.pdf"));

            var documentsInformation = new Mock<IWorkerDocumentsInformation<IdentificationType, CovenantFile>>();
            documentsInformation.SetupGet(d => d.IdentificationNumber1).Returns("111111111");
            documentsInformation.SetupGet(d => d.IdentificationType1File).Returns((CovenantFile)null);
            documentsInformation.SetupGet(d => d.IdentificationType2File).Returns((CovenantFile)null);
            documentsInformation.SetupGet(d => d.PoliceCheckBackGround).Returns((CovenantFile)null);
            documentsInformation.SetupGet(d => d.HavePoliceCheckBackground).Returns(false);
            documentsInformation.SetupGet(d => d.Resume).Returns((CovenantFile)null);

            Result result = worker.PatchDocuments(documentsInformation.Object);

            Assert.True(result);
            Assert.Null(worker.IdentificationType1File);
            Assert.Null(worker.IdentificationType1FileId);
            Assert.Null(worker.PoliceCheckBackGround);
            Assert.Null(worker.PoliceCheckBackGroundId);
            Assert.False(worker.HavePoliceCheckBackground);
            Assert.Equal("resume.pdf", worker.Resume.FileName);
        }
    }
}