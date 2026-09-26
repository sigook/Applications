using Covenant.Api.Authorization;
using Covenant.Common.Constants;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Candidate;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Request;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Net.Http.Json;
using RecruitingApplicantsController = Covenant.Api.Controllers.Sigook.Agency.Recruiting.ApplicantsController;

namespace Covenant.Integration.Tests.AgencyModule.Recruiting;

public class ApplicantsControllerTest : IClassFixture<CustomWebApplicationFactory<ApplicantsControllerTest.Startup>>
{
    private readonly CustomWebApplicationFactory<Startup> _factory;
    private readonly HttpClient _client;

    public ApplicantsControllerTest(CustomWebApplicationFactory<Startup> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<AgencyApplicantsPagedResponse> Get(string query = "")
    {
        var response = await _client.GetAsync($"{RecruitingApplicantsController.RouteName}{query}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AgencyApplicantsPagedResponse>();
    }

    [Fact]
    public async Task GetPaginatesByRequestSortedByNumberId()
    {
        AgencyApplicantsPagedResponse descending = await Get("?isDescending=true");
        Assert.Equal(3, descending.TotalItems);
        Assert.Equal(8, descending.TotalApplicants);
        Assert.True(descending.Items.First().NumberId > descending.Items.Last().NumberId);

        AgencyApplicantsPagedResponse ascending = await Get("?isDescending=false");
        Assert.True(ascending.Items.First().NumberId < ascending.Items.Last().NumberId);
    }

    [Fact]
    public async Task GetKeepsEveryApplicantOfTheRequestInTheSamePage()
    {
        AgencyApplicantsPagedResponse first = await Get("?pageSize=1&isDescending=false");
        Assert.Equal(3, first.TotalItems);
        Assert.Equal(3, first.TotalPages);
        Assert.Single(first.Items);

        AgencyApplicantsPagedResponse second = await Get("?pageSize=1&pageIndex=2&isDescending=false");
        Assert.Single(second.Items);
        Assert.NotEqual(first.Items.Single().RequestId, second.Items.Single().RequestId);

        AgencyApplicantsPagedResponse third = await Get("?pageSize=1&pageIndex=3&isDescending=false");
        List<AgencyRequestApplicantsModel> requests = [.. first.Items, .. second.Items, .. third.Items];
        AgencyRequestApplicantsModel later = requests.Single(r => r.RequestId == Startup.FakeLaterRequest.Id);
        Assert.Equal(2, later.Applicants.Count);
    }

    [Fact]
    public async Task GetReturnsRequestSummaryWithConfirmedCount()
    {
        AgencyApplicantsPagedResponse page = await Get();
        AgencyRequestApplicantsModel request = page.Items.Single(r => r.RequestId == Startup.FakeLaterRequest.Id);
        Assert.Equal(2, request.WorkersQuantity);
        Assert.Equal(1, request.ConfirmedApplicants);
        Assert.Equal(2, request.TotalApplicants);
        Assert.True(request.IsDirectHiring);
        Assert.Equal(Startup.FakeLaterRequest.StartAt, request.StartAt);
    }

    [Fact]
    public async Task GetReturnsComplianceProgress()
    {
        AgencyApplicantsPagedResponse page = await Get();
        List<AgencyApplicantListModel> applicants = page.Items.SelectMany(r => r.Applicants).ToList();

        AgencyApplicantListModel confirmed = applicants.Single(i => i.Id == Startup.FakeApplicantConfirmed.Id);
        Assert.Equal(2, confirmed.ComplianceTotal);
        Assert.Equal(1, confirmed.ComplianceCompleted);
        Assert.Equal(0, confirmed.MandatoryPending);

        AgencyApplicantListModel pending = applicants.Single(i => i.Id == Startup.FakeApplicantPending.Id);
        Assert.Equal(0, pending.ComplianceCompleted);
        Assert.Equal(1, pending.MandatoryPending);
        Assert.NotNull(pending.CandidateId);
    }

    [Fact]
    public async Task GetFileReturnsTheExcelOfEveryApplicantOfTheFilter()
    {
        var response = await _client.GetAsync($"{RecruitingApplicantsController.RouteName}/File");
        response.EnsureSuccessStatusCode();
        Assert.Equal(CovenantConstants.ExcelMime, response.Content.Headers.ContentType?.MediaType);
        Assert.NotEmpty(await response.Content.ReadAsByteArrayAsync());
    }

    [Fact]
    public async Task ChangeStatusStartsTheValidApplicantsAndReportsTheRest()
    {
        Guid missingId = Guid.NewGuid();
        var model = new ChangeApplicantsStatusModel
        {
            ApplicantIds = [Startup.FakeBulkPending.Id, Startup.FakeBulkConfirmed.Id, missingId],
            Status = RequestApplicantStatus.InProgress
        };
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{RecruitingApplicantsController.RouteName}/Status", model);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ChangeApplicantsStatusResultModel>();
        Assert.Equal(1, result.Updated);
        Assert.Equal(2, result.Skipped.Count);
        Assert.Contains(result.Skipped, s => s.ApplicantId == Startup.FakeBulkConfirmed.Id && s.Reason.Contains("pending or cancelled"));
        Assert.Contains(result.Skipped, s => s.ApplicantId == missingId);

        var context = _factory.Services.GetRequiredService<CovenantContext>();
        RequestApplicant started = await context.RequestApplicants.SingleAsync(a => a.Id == Startup.FakeBulkPending.Id);
        Assert.Equal(RequestApplicantStatus.InProgress, started.Status);
    }

    [Fact]
    public async Task ChangeStatusConfirmsOnlyTheApplicantsWithMandatoryItemsCompleted()
    {
        var model = new ChangeApplicantsStatusModel
        {
            ApplicantIds = [Startup.FakeBulkReady.Id, Startup.FakeBulkBlocked.Id, Startup.FakeBulkCandidate.Id],
            Status = RequestApplicantStatus.Confirmed
        };
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{RecruitingApplicantsController.RouteName}/Status", model);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ChangeApplicantsStatusResultModel>();
        Assert.Equal(1, result.Updated);
        Assert.Equal(2, result.Skipped.Count);
        Assert.Contains(result.Skipped, s => s.ApplicantId == Startup.FakeBulkBlocked.Id && s.Reason.Contains("mandatory"));
        Assert.Contains(result.Skipped, s => s.ApplicantId == Startup.FakeBulkCandidate.Id && s.Reason.Contains("candidate"));

        var context = _factory.Services.GetRequiredService<CovenantContext>();
        RequestApplicant confirmed = await context.RequestApplicants.SingleAsync(a => a.Id == Startup.FakeBulkReady.Id);
        Assert.Equal(RequestApplicantStatus.Confirmed, confirmed.Status);
        RequestApplicant blocked = await context.RequestApplicants.SingleAsync(a => a.Id == Startup.FakeBulkBlocked.Id);
        Assert.Equal(RequestApplicantStatus.InProgress, blocked.Status);
    }

    [Fact]
    public async Task GetFiltersByStatus()
    {
        AgencyApplicantsPagedResponse page = await Get($"?statuses={(int)RequestApplicantStatus.InProgress}");
        List<AgencyApplicantListModel> applicants = page.Items.SelectMany(r => r.Applicants).ToList();
        Assert.All(applicants, i => Assert.Equal(RequestApplicantStatus.InProgress, i.Status));
        Assert.Contains(applicants, i => i.Id == Startup.FakeApplicantInProgress.Id);
        Assert.DoesNotContain(page.Items, r => r.RequestId == Startup.FakeSoonRequest.Id);
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder()
                .AddTestAuth(o =>
                {
                    o.AddAgencyPersonnelRole(AgencyId);
                    o.AddName("r@mail.com");
                });
            services.AddTestDatabase();
            services.AddSingleton<IRequestRepository, RequestRepository>();
            services.AddSingleton<ITimeService, TimeService>();
            services.AddSingleton<IWorkerRequestRepository, WorkerRequestRepository>();
            services.AddSingleton<AgencyIdFilter>();
        }

        public static readonly Request FakeSoonRequest = FakeData.FakeRequest(startAt: DateTime.Today.AddDays(2), workersQuantity: 1);

        private static readonly Guid AgencyId = FakeSoonRequest.CompanyProfile.AgencyId;

        public static readonly Request FakeLaterRequest = FakeData.FakeRequest(agencyId: AgencyId, startAt: DateTime.Today.AddDays(20), workersQuantity: 2);

        public static readonly RequestComplianceItem FakeMandatoryItem = RequestComplianceItem.Create(FakeLaterRequest.Id, "W4", true, ComplianceDocumentTarget.OtherDocument).Value;
        public static readonly RequestComplianceItem FakeOptionalItem = RequestComplianceItem.Create(FakeLaterRequest.Id, "WP", false, ComplianceDocumentTarget.None).Value;
        public static readonly RequestComplianceItem FakeSoonMandatoryItem = RequestComplianceItem.Create(FakeSoonRequest.Id, "ID", true, ComplianceDocumentTarget.Identification1).Value;

        public static readonly WorkerProfile FakeWorkerConfirmed = new(new User(CvnEmail.Create("w.confirmed@mail.com").Value), AgencyId) { Location = FakeData.FakeLocation() };
        public static readonly RequestApplicant FakeApplicantConfirmed =
            RequestApplicant.CreateWithWorker(FakeLaterRequest.Id, FakeWorkerConfirmed.Id, "me@mail.com", null, RequestApplicantStatus.Confirmed).Value;
        public static readonly RequestApplicantComplianceItem FakeConfirmedCompletion =
            RequestApplicantComplianceItem.Create(FakeApplicantConfirmed.Id, FakeMandatoryItem.Id, "me@mail.com").Value;

        public static readonly WorkerProfile FakeWorkerInProgress = new(new User(CvnEmail.Create("w.progress@mail.com").Value), AgencyId) { Location = FakeData.FakeLocation() };
        public static readonly RequestApplicant FakeApplicantInProgress =
            RequestApplicant.CreateWithWorker(FakeLaterRequest.Id, FakeWorkerInProgress.Id, "me@mail.com", null, RequestApplicantStatus.InProgress).Value;

        public static readonly Candidate FakeCandidatePending = new(AgencyId, "Pending", CvnEmail.Create("c.pending@mail.com").Value);
        public static readonly RequestApplicant FakeApplicantPending =
            RequestApplicant.CreateWithCandidate(FakeSoonRequest.Id, FakeCandidatePending.Id, "me@mail.com", null, RequestApplicantStatus.Pending).Value;

        // The bulk tests own this request so they can move statuses around without
        // touching the applicants the listing tests assert on.
        public static readonly Request FakeBulkRequest = FakeData.FakeRequest(agencyId: AgencyId, startAt: DateTime.Today.AddDays(30), workersQuantity: 5);
        public static readonly RequestComplianceItem FakeBulkMandatoryItem = RequestComplianceItem.Create(FakeBulkRequest.Id, "W4", true, ComplianceDocumentTarget.OtherDocument).Value;

        public static readonly WorkerProfile FakeWorkerBulkPending = new(new User(CvnEmail.Create("w.bulkpending@mail.com").Value), AgencyId) { Location = FakeData.FakeLocation() };
        public static readonly RequestApplicant FakeBulkPending =
            RequestApplicant.CreateWithWorker(FakeBulkRequest.Id, FakeWorkerBulkPending.Id, "me@mail.com", null, RequestApplicantStatus.Pending).Value;

        public static readonly WorkerProfile FakeWorkerBulkConfirmed = new(new User(CvnEmail.Create("w.bulkconfirmed@mail.com").Value), AgencyId) { Location = FakeData.FakeLocation() };
        public static readonly RequestApplicant FakeBulkConfirmed =
            RequestApplicant.CreateWithWorker(FakeBulkRequest.Id, FakeWorkerBulkConfirmed.Id, "me@mail.com", null, RequestApplicantStatus.Confirmed).Value;

        public static readonly WorkerProfile FakeWorkerBulkReady = new(new User(CvnEmail.Create("w.bulkready@mail.com").Value), AgencyId) { Location = FakeData.FakeLocation() };
        public static readonly RequestApplicant FakeBulkReady =
            RequestApplicant.CreateWithWorker(FakeBulkRequest.Id, FakeWorkerBulkReady.Id, "me@mail.com", null, RequestApplicantStatus.InProgress).Value;
        public static readonly RequestApplicantComplianceItem FakeBulkReadyCompletion =
            RequestApplicantComplianceItem.Create(FakeBulkReady.Id, FakeBulkMandatoryItem.Id, "me@mail.com").Value;

        public static readonly WorkerProfile FakeWorkerBulkBlocked = new(new User(CvnEmail.Create("w.bulkblocked@mail.com").Value), AgencyId) { Location = FakeData.FakeLocation() };
        public static readonly RequestApplicant FakeBulkBlocked =
            RequestApplicant.CreateWithWorker(FakeBulkRequest.Id, FakeWorkerBulkBlocked.Id, "me@mail.com", null, RequestApplicantStatus.InProgress).Value;

        public static readonly Candidate FakeCandidateBulk = new(AgencyId, "Bulk", CvnEmail.Create("c.bulk@mail.com").Value);
        public static readonly RequestApplicant FakeBulkCandidate =
            RequestApplicant.CreateWithCandidate(FakeBulkRequest.Id, FakeCandidateBulk.Id, "me@mail.com", null, RequestApplicantStatus.InProgress).Value;
        public static readonly RequestApplicantComplianceItem FakeBulkCandidateCompletion =
            RequestApplicantComplianceItem.Create(FakeBulkCandidate.Id, FakeBulkMandatoryItem.Id, "me@mail.com").Value;

        public void Configure(IApplicationBuilder app, CovenantContext context)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCaching();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
            FakeLaterRequest.WorkerSalary = 55000;
            context.Requests.AddRange(FakeSoonRequest, FakeLaterRequest, FakeBulkRequest);
            context.WorkerProfiles.AddRange(FakeWorkerConfirmed, FakeWorkerInProgress,
                FakeWorkerBulkPending, FakeWorkerBulkConfirmed, FakeWorkerBulkReady, FakeWorkerBulkBlocked);
            context.Candidates.AddRange(FakeCandidatePending, FakeCandidateBulk);
            context.RequestApplicants.AddRange(FakeApplicantConfirmed, FakeApplicantInProgress, FakeApplicantPending,
                FakeBulkPending, FakeBulkConfirmed, FakeBulkReady, FakeBulkBlocked, FakeBulkCandidate);
            context.RequestComplianceItems.AddRange(FakeMandatoryItem, FakeOptionalItem, FakeSoonMandatoryItem, FakeBulkMandatoryItem);
            context.RequestApplicantComplianceItems.AddRange(FakeConfirmedCompletion, FakeBulkReadyCompletion, FakeBulkCandidateCompletion);
            context.SaveChanges();
        }
    }
}
