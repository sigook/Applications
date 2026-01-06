using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using Covenant.Infrastructure.Context;
using Covenant.Integration.Tests.Utils;

namespace Covenant.Integration.Tests.AgencyModule.AgencyRequestWorker
{
    public partial class AgencyRequestWorkerControllerTest
    {
        private static class Data
        {
            public static readonly Guid AgencyId = Guid.NewGuid();
            private static readonly Availability FakeAvailability = new Availability();
            public static readonly DateTime FakeNow = new DateTime(2019, 01, 01);

            private static readonly CompanyProfileJobPositionRate JobPositionRate = new CompanyProfileJobPositionRate { JobPosition = new JobPosition() };
            public static readonly Request FakeRequest = Request.AgencyCreateRequest(AgencyId, Guid.NewGuid(), FakeData.FakeLocation(), FakeNow, JobPositionRate.Id, workersQuantity: 5).Value;

            public static readonly WorkerProfile FakeWorkerForList = new WorkerProfile(new User(CvnEmail.Create("w_profile@mail.com").Value))
            {
                ApprovedToWork = true,
                AgencyId = AgencyId,
                Location = new Location { City = new City { Province = new Province() } },
            };

            static Data()
            {
                FakeWorkerForList.PatchAvailabilities(new[] { new BaseModel<Guid>(FakeAvailability.Id) });
                FakeWorkerForList.PatchProfileImage(new CovenantFile("profile.png"));
                FakeWorkerForList.PatchSinInformation(new FakeSinInfo
                {
                    DueDate = DateTime.Now.AddDays(1),
                    SocialInsurance = "A987654321B",
                    SocialInsuranceExpire = true,
                    SocialInsuranceFile = new CovenantFile("sin.pdf")
                });
                FakeWorkerToBook.PatchSinInformation(new FakeSinInfo
                {
                    DueDate = DateTime.Now.AddDays(1),
                    SocialInsurance = "A123456789B",
                    SocialInsuranceExpire = true,
                    SocialInsuranceFile = new CovenantFile("sin.pdf")
                });
                FakeWorkerToBook.PatchAvailabilities(new[] { new BaseModel<Guid>(FakeAvailability.Id) });
                FakeWorkerToBook.PatchProfileImage(new CovenantFile("profile.png"));
            }

            public static readonly WorkerProfile FakeWorkerToBook = new WorkerProfile(new User(CvnEmail.Create("w_profile@mail.com").Value))
            {
                ApprovedToWork = true,
                AgencyId = AgencyId,
                Location = new Location { City = new City { Province = new Province { Country = new Country { Code = "USA" } } } }
            };

            private static readonly WorkerProfile FakeWorkerToReject = new WorkerProfile(new User(CvnEmail.Create("w_reject@mail.com").Value))
            {
                ApprovedToWork = true,
                AgencyId = AgencyId,
                Location = new Location { City = new City { Province = new Province() } }
            };

            public static readonly WorkerProfile FakeWorkerToRejectObsolete = new WorkerProfile(new User(CvnEmail.Create("w_reject_obsolote@mail.com").Value))
            {
                ApprovedToWork = true,
                AgencyId = AgencyId,
                Location = new Location { City = new City { Province = new Province() } }
            };

            public static readonly Covenant.Common.Entities.Request.WorkerRequest FakeWorkerRequestList =
                 Covenant.Common.Entities.Request.WorkerRequest.AgencyBook(FakeWorkerForList.WorkerId, FakeRequest.Id, "recruiter@mail.com");

            public static readonly Covenant.Common.Entities.Request.WorkerRequest FakeWorkerRequestReject =
                Covenant.Common.Entities.Request.WorkerRequest.AgencyBook(FakeWorkerToReject.WorkerId, FakeRequest.Id, "recruiter@mail.com");

            public static readonly Covenant.Common.Entities.Request.WorkerRequest FakeWorkerRequestRejectObsolete =
                Covenant.Common.Entities.Request.WorkerRequest.AgencyBook(FakeWorkerToRejectObsolete.WorkerId, FakeRequest.Id, "recruiter@mail.com");

            public static readonly IEnumerable<WorkerProfile> FakeWorkers = new[] { FakeWorkerForList, FakeWorkerToBook, FakeWorkerToReject, FakeWorkerToRejectObsolete };

            public static void Seed(CovenantContext context)
            {
                context.Availability.Add(FakeAvailability);
                context.CompanyProfileJobPositionRate.Add(JobPositionRate);
                context.WorkerProfile.AddRange(FakeWorkers);
                context.Request.Add(FakeRequest);
                context.WorkerRequest.AddRange(FakeWorkerRequestList, FakeWorkerRequestReject, FakeWorkerRequestRejectObsolete);
                context.SaveChanges();
            }

            private class FakeSinInfo : ISinInformation<CovenantFile>
            {
                public string SocialInsurance { get; set; }
                public bool SocialInsuranceExpire { get; set; }
                public DateTime? DueDate { get; set; }
                public CovenantFile SocialInsuranceFile { get; set; }
            }
        }
    }
}