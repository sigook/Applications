using Covenant.Common.Constants;
using Covenant.Common.Entities.Accounting.PayStubs;
using Covenant.Common.Entities.Agencies;
using Covenant.Common.Entities.Companies;
using Covenant.Common.Entities.Requests;
using Covenant.Common.Entities.Workers;
using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces.Storage;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Moq;

namespace Covenant.Integration.Tests.Workers.Activity;

public class ActivityTestData : ITestData
{
    public static readonly DateTime WeekStart = new(2026, 10, 18);

    private readonly List<object> _entities = [];

    public City City { get; } = FakeData.FakeCity();
    public Agency Agency { get; }
    public WorkerProfile Profile { get; }
    public WorkerProfile OtherProfile { get; }
    public Request ActiveRequest { get; }
    public Request CompletedRequest { get; }
    public Request EndedRequest { get; }
    public Request RejectedRequest { get; }
    public WorkerRequest ActiveAssignment { get; }
    public TimeSheet OpenTimeSheet { get; }
    public TimeSheet PendingTimeSheet { get; }
    public TimeSheet NextWeekTimeSheet { get; }
    public PayStub OldPayStub { get; }
    public PayStub PreviousPayStub { get; }
    public PayStub LastPayStub { get; }
    public PayStub OtherWorkerPayStub { get; }

    public ActivityTestData()
    {
        Agency = FakeData.FakeAgency(city: City);
        Profile = FakeData.FakeWorkerProfile(Agency, $"worker.{Guid.NewGuid():N}@activity.com", City);
        OtherProfile = FakeData.FakeWorkerProfile(Agency, $"other.{Guid.NewGuid():N}@activity.com", City);

        ActiveRequest = RequestFor("Acme Foods", TimeSpan.FromMinutes(30));
        CompletedRequest = RequestFor("Beta Logistics", TimeSpan.Zero);
        EndedRequest = RequestFor("Cedar Retail", TimeSpan.Zero);
        RejectedRequest = RequestFor("Delta Parts", TimeSpan.Zero);

        ActiveAssignment = Assignment(ActiveRequest, new DateTime(2026, 10, 1));
        var completed = Assignment(CompletedRequest, new DateTime(2026, 9, 1));
        completed.Finish(new DateTime(2026, 10, 24), WorkerRequestFinishReason.ContractCompleted);
        var ended = Assignment(EndedRequest, new DateTime(2026, 8, 1));
        ended.Finish(new DateTime(2026, 8, 20), WorkerRequestFinishReason.WorkerResigned);
        var rejected = Assignment(RejectedRequest, new DateTime(2026, 7, 1));
        rejected.Reject("No show", new DateTime(2026, 7, 3));

        OpenTimeSheet = TimeSheet.WorkerClockIn(ActiveAssignment.Id, WeekStart.AddHours(8)).Value;
        _entities.Add(OpenTimeSheet);
        for (int day = 1; day <= 5; day++)
            _entities.Add(TimeSheet.CreateTimeSheet(ActiveAssignment.Id, WeekStart.AddDays(day), TimeSpan.FromHours(10), now: WeekStart).Value);
        PendingTimeSheet = TimeSheet.WorkerClockIn(completed.Id, WeekStart.AddDays(6).AddHours(8)).Value;
        PendingTimeSheet.AddClockOut(WeekStart.AddDays(6).AddHours(12));
        PendingTimeSheet.UpdateMissing(TimeSpan.FromHours(1), TimeSpan.FromMinutes(30), 20, 25);
        PendingTimeSheet.AddBonusOrOthers(50, "Safety bonus");
        PendingTimeSheet.AddDeductionsOthers(15, "Boots");
        PendingTimeSheet.AddReimbursements(12.5m, "Parking");
        _entities.Add(PendingTimeSheet);
        NextWeekTimeSheet = TimeSheet.CreateTimeSheet(ActiveAssignment.Id, WeekStart.AddDays(8), TimeSpan.FromHours(8), now: WeekStart).Value;
        _entities.Add(NextWeekTimeSheet);

        var paidTimeSheet = TimeSheet.CreateTimeSheet(ActiveAssignment.Id, WeekStart.AddDays(-6), TimeSpan.FromHours(8), now: WeekStart).Value;
        _entities.Add(paidTimeSheet);
        var paidTotal = TimeSheetTotalPayroll.CreateTotal(paidTimeSheet.Id, TimeSpan.FromHours(8), TimeSpan.FromHours(8), TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromHours(8));

        OldPayStub = PayStubFor(Profile, 9101, new DateTime(2025, 12, 27), 400m);
        PreviousPayStub = PayStubFor(Profile, 9102, new DateTime(2026, 10, 10), 500m);
        LastPayStub = PayStubFor(Profile, 9103, new DateTime(2026, 10, 17), 700m,
            [new PayStubWageDetail(1, 8, 0, 0, 0, 0, 0, 0, paidTotal.Id).AttachTimeSheetTotal(paidTotal)]);
        OtherWorkerPayStub = PayStubFor(OtherProfile, 9104, new DateTime(2026, 10, 17), 900m);
    }

    public HttpClient AsWorker(Func<HttpClient> create)
    {
        HttpClient client = create();
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.RoleHeader, CovenantConstants.Role.Worker);
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.SubHeader, Profile.WorkerId.ToString());
        return client;
    }

    public void Seed(CovenantContext context)
    {
        context.Agencies.Add(Agency);
        context.WorkerProfiles.AddRange(Profile, OtherProfile);
        context.AddRange(_entities);
        context.PayStubs.AddRange(OldPayStub, PreviousPayStub, LastPayStub, OtherWorkerPayStub);
        context.SaveChanges();
    }

    private Request RequestFor(string client, TimeSpan durationBreak)
    {
        var companyProfile = FakeData.FakeCompanyProfileForAgency(Agency.Id, client, City);
        var rate = FakeData.FakeJobPositionRate(companyProfile);
        var request = FakeData.FakeRequest(Agency.Id, companyProfile.Id, rate.Id, startAt: new DateTime(2026, 1, 5));
        request.UpdateDurationBreak(durationBreak);
        request.UpdateBreakIsPaid(false);
        _entities.AddRange([companyProfile, rate, request]);
        return request;
    }

    private WorkerRequest Assignment(Request request, DateTime createdAt)
    {
        var assignment = WorkerRequest.AgencyBook(Profile.Id, request.Id).With(wr => wr.CreatedAt = createdAt);
        _entities.Add(assignment);
        return assignment;
    }

    private static PayStub PayStubFor(WorkerProfile worker, long number, DateTime weekEnding, decimal earnings, List<PayStubWageDetail> wageDetails = null) =>
        PayStub.Create(
            workerProfileId: worker.Id,
            payStubNumberId: number,
            payStubNumber: $"PS-{number}",
            position: "General Labour",
            dateWorkBegins: weekEnding.AddDays(-6),
            dateWorkEnd: weekEnding,
            paymentDate: weekEnding.AddDays(6),
            weekEnding: weekEnding,
            createdAt: weekEnding.AddDays(6),
            regularWage: earnings,
            grossPayment: earnings,
            vacations: 0,
            totalEarnings: earnings,
            cpp: 20m,
            ei: 8m,
            federalTax: 40m,
            provincialTax: 10m,
            totalDeductions: 78m,
            totalPaid: earnings - 78m,
            items: [PayStubItem.CreateRegular(40, earnings / 42).Value, PayStubItem.CreateItem("Overtime Hours", 2, earnings / 42, PayStubItemType.Overtime).Value],
            otherDeductions: [],
            wageDetails: wageDetails).With(p => p.NumberId = number);
}

public class ActivityTestStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDefaultTestConfiguration();
        services.AddTestAuthenticationBuilder().AddTestAuth(_ => { });
        services.AddTestDatabase();
        services.AddSingleton(Mock.Of<IPayStubsContainer>());
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
