using Covenant.Common.Entities;
using Covenant.Common.Functionals;
using Covenant.Common.Entities.Worker;
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
    }
}