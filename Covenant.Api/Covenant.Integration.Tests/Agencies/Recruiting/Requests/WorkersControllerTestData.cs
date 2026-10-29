using Covenant.Common.Entities.Agencies;
using Covenant.Common.Entities.Companies;
using Covenant.Common.Entities.Requests;
using Covenant.Common.Entities.Workers;
using Covenant.Common.Entities;
using Covenant.Common.Models.Workers;
using Covenant.Common.Models;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;

namespace Covenant.Integration.Tests.Agencies.Recruiting.Requests;

public partial class WorkersControllerTest
{
    public class Data : ITestData
    {
        public static readonly Guid AgencyId = Guid.NewGuid();
        public static readonly DateTime FakeNow = new(2019, 01, 01);

        private readonly Availability availability = new();

        public Agency Agency { get; }
        public CompanyProfile CompanyProfile { get; }
        public CompanyProfileJobPositionRate JobPositionRate { get; }
        public Request Request { get; }
        public Request RequestToFinishAll { get; }
        public WorkerProfile WorkerForList { get; }
        public WorkerProfile WorkerToBook { get; }
        public WorkerProfile WorkerToReject { get; }
        public WorkerProfile WorkerToRejectObsolete { get; }
        public WorkerProfile WorkerToFinish { get; }
        public WorkerProfile WorkerToFinishAll1 { get; }
        public WorkerProfile WorkerToFinishAll2 { get; }
        public WorkerRequest WorkerRequestList { get; }
        public WorkerRequest WorkerRequestReject { get; }
        public WorkerRequest WorkerRequestRejectObsolete { get; }
        public WorkerRequest WorkerRequestFinish { get; }
        public WorkerRequest WorkerRequestFinishAll1 { get; }
        public WorkerRequest WorkerRequestFinishAll2 { get; }

        public Data()
        {
            Agency = FakeData.FakeAgency(AgencyId);
            CompanyProfile = FakeData.FakeCompanyProfile(Agency);
            JobPositionRate = FakeData.FakeJobPositionRate(CompanyProfile);
            Request = Request.AgencyCreateRequest(CompanyProfile.Id, FakeData.FakeLocation(), FakeNow,
                JobPositionRate.Id, workerRate: 15M, agencyRate: 20M, workersQuantity: 5).Value;

            WorkerForList = FakeData.FakeWorkerProfile(Agency, "w_profile@mail.com");
            WorkerToBook = FakeData.FakeWorkerProfile(Agency, "w_book@mail.com",
                FakeData.FakeCity(FakeData.FakeProvince(FakeData.FakeCountry("USA"))));
            WorkerToReject = FakeData.FakeWorkerProfile(Agency, "w_reject@mail.com");
            WorkerToRejectObsolete = FakeData.FakeWorkerProfile(Agency, "w_reject_obsolote@mail.com");
            WorkerToFinish = FakeData.FakeWorkerProfile(Agency, "w_finish@mail.com");
            WorkerToFinishAll1 = FakeData.FakeWorkerProfile(Agency, "w_finish_all1@mail.com");
            WorkerToFinishAll2 = FakeData.FakeWorkerProfile(Agency, "w_finish_all2@mail.com");
            RequestToFinishAll = Request.AgencyCreateRequest(CompanyProfile.Id, FakeData.FakeLocation(), FakeNow,
                JobPositionRate.Id, workerRate: 15M, agencyRate: 20M, workersQuantity: 2).Value;

            PatchWorker(WorkerForList, "A987654321B");
            PatchWorker(WorkerToBook, "A123456789B");

            WorkerRequestList = WorkerRequest.AgencyBook(WorkerForList.Id, Request.Id, "recruiter@mail.com");
            WorkerRequestReject = WorkerRequest.AgencyBook(WorkerToReject.Id, Request.Id, "recruiter@mail.com");
            WorkerRequestRejectObsolete = WorkerRequest.AgencyBook(WorkerToRejectObsolete.Id, Request.Id, "recruiter@mail.com");
            WorkerRequestFinish = WorkerRequest.AgencyBook(WorkerToFinish.Id, Request.Id, "recruiter@mail.com");
            WorkerRequestFinishAll1 = WorkerRequest.AgencyBook(WorkerToFinishAll1.Id, RequestToFinishAll.Id, "recruiter@mail.com");
            WorkerRequestFinishAll2 = WorkerRequest.AgencyBook(WorkerToFinishAll2.Id, RequestToFinishAll.Id, "recruiter@mail.com");
        }

        public IEnumerable<WorkerProfile> Workers =>
            [WorkerForList, WorkerToBook, WorkerToReject, WorkerToRejectObsolete, WorkerToFinish, WorkerToFinishAll1, WorkerToFinishAll2];

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
            context.Requests.AddRange(Request, RequestToFinishAll);
            context.WorkerRequests.AddRange(WorkerRequestList, WorkerRequestReject, WorkerRequestRejectObsolete,
                WorkerRequestFinish, WorkerRequestFinishAll1, WorkerRequestFinishAll2);
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
