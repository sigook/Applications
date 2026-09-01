using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;

namespace Covenant.Integration.Tests.AgencyModule.Requests
{
    public partial class WorkersControllerTest
    {
        public class Data : ITestData
        {
            public static readonly Guid AgencyId = Guid.NewGuid();
            public static readonly DateTime FakeNow = new(2019, 01, 01);

            private readonly Availability availability = new();

            public Covenant.Common.Entities.Agency.Agency Agency { get; }
            public CompanyProfile CompanyProfile { get; }
            public CompanyProfileJobPositionRate JobPositionRate { get; }
            public Request Request { get; }
            public WorkerProfile WorkerForList { get; }
            public WorkerProfile WorkerToBook { get; }
            public WorkerProfile WorkerToReject { get; }
            public WorkerProfile WorkerToRejectObsolete { get; }
            public WorkerRequest WorkerRequestList { get; }
            public WorkerRequest WorkerRequestReject { get; }
            public WorkerRequest WorkerRequestRejectObsolete { get; }

            public Data()
            {
                Agency = FakeData.FakeAgency(AgencyId);
                CompanyProfile = FakeData.FakeCompanyProfile(Agency);
                JobPositionRate = FakeData.FakeJobPositionRate(CompanyProfile);
                Request = Request.AgencyCreateRequest(CompanyProfile.Id, FakeData.FakeLocation(), FakeNow,
                    JobPositionRate.Id, workersQuantity: 5).Value;

                WorkerForList = FakeData.FakeWorkerProfile(Agency, "w_profile@mail.com");
                WorkerToBook = FakeData.FakeWorkerProfile(Agency, "w_book@mail.com",
                    FakeData.FakeCity(FakeData.FakeProvince(FakeData.FakeCountry("USA"))));
                WorkerToReject = FakeData.FakeWorkerProfile(Agency, "w_reject@mail.com");
                WorkerToRejectObsolete = FakeData.FakeWorkerProfile(Agency, "w_reject_obsolote@mail.com");

                PatchWorker(WorkerForList, "A987654321B");
                PatchWorker(WorkerToBook, "A123456789B");

                WorkerRequestList = WorkerRequest.AgencyBook(WorkerForList.Id, Request.Id, "recruiter@mail.com");
                WorkerRequestReject = WorkerRequest.AgencyBook(WorkerToReject.Id, Request.Id, "recruiter@mail.com");
                WorkerRequestRejectObsolete = WorkerRequest.AgencyBook(WorkerToRejectObsolete.Id, Request.Id, "recruiter@mail.com");
            }

            public IEnumerable<WorkerProfile> Workers =>
                [WorkerForList, WorkerToBook, WorkerToReject, WorkerToRejectObsolete];

            private void PatchWorker(WorkerProfile worker, string socialInsurance)
            {
                worker.PatchAvailabilities([new BaseModel<Guid>(availability.Id)]);
                worker.PatchProfileImage(new CovenantFile("profile.png"));
                worker.PatchSinInformation(new FakeSinInfo
                {
                    DueDate = DateTime.Now.AddDays(1),
                    SocialInsurance = socialInsurance,
                    SocialInsuranceExpire = true,
                    SocialInsuranceFile = new CovenantFile("sin.pdf")
                });
            }

            public void Seed(CovenantContext context)
            {
                context.Agencies.Add(Agency);
                context.Availabilities.Add(availability);
                context.CompanyProfiles.Add(CompanyProfile);
                context.CompanyProfileJobPositionRates.Add(JobPositionRate);
                context.WorkerProfiles.AddRange(Workers);
                context.Requests.Add(Request);
                context.WorkerRequests.AddRange(WorkerRequestList, WorkerRequestReject, WorkerRequestRejectObsolete);
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
