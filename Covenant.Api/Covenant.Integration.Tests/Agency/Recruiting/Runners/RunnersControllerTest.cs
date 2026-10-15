using Covenant.Api.Authorization;
using Covenant.Common.Constants;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Request.Runners;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Identity;
using Covenant.Common.Models.Request.Runners;
using Covenant.Common.Repositories.Requests;
using Covenant.Common.Repositories.Workers;
using Covenant.Core.BL.Interfaces.Requests;
using Covenant.Core.BL.Services.Requests;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Requests;
using Covenant.Infrastructure.Repositories.Workers;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using RecruitingRunnersController = Covenant.Api.Controllers.Agency.Recruiting.Runners.RunnersController;

namespace Covenant.Integration.Tests.Agency.Recruiting.Runners;

public class RunnersControllerTest : IClassFixture<CustomWebApplicationFactory<RunnersControllerTest.Startup>>
{
    private readonly CustomWebApplicationFactory<Startup> _factory;
    private readonly HttpClient _client;

    public RunnersControllerTest(CustomWebApplicationFactory<Startup> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<AgencyRunnersPagedResponse> Get(string query = "")
    {
        var response = await _client.GetAsync($"{RecruitingRunnersController.RouteName}{query}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AgencyRunnersPagedResponse>();
    }

    private Task<Runner> GetRunner(Guid id) => _factory.Services.GetRequiredService<CovenantContext>().Runners
        .Include(r => r.StatusHistory)
        .SingleAsync(r => r.Id == id);

    [Fact]
    public async Task GetPaginatesByRequestSortedByNumberId()
    {
        AgencyRunnersPagedResponse descending = await Get("?isDescending=true");
        Assert.Equal(3, descending.TotalItems);
        Assert.Equal(8, descending.TotalRunners);
        Assert.True(descending.Items.First().NumberId > descending.Items.Last().NumberId);

        AgencyRunnersPagedResponse ascending = await Get("?isDescending=false");
        Assert.True(ascending.Items.First().NumberId < ascending.Items.Last().NumberId);
    }

    [Fact]
    public async Task GetKeepsEveryRunnerOfTheRequestInTheSamePage()
    {
        AgencyRunnersPagedResponse first = await Get("?pageSize=1&isDescending=false");
        Assert.Equal(3, first.TotalItems);
        Assert.Equal(3, first.TotalPages);
        Assert.Single(first.Items);

        AgencyRunnersPagedResponse second = await Get("?pageSize=1&pageIndex=2&isDescending=false");
        AgencyRunnersPagedResponse third = await Get("?pageSize=1&pageIndex=3&isDescending=false");
        List<AgencyRequestRunnersModel> requests = [.. first.Items, .. second.Items, .. third.Items];
        AgencyRequestRunnersModel later = requests.Single(r => r.RequestId == Startup.FakeLaterRequest.Id);
        Assert.Equal(3, later.Runners.Count);
    }

    [Fact]
    public async Task GetReturnsRequestSummaryWithHiredCount()
    {
        AgencyRunnersPagedResponse page = await Get();
        AgencyRequestRunnersModel request = page.Items.Single(r => r.RequestId == Startup.FakeLaterRequest.Id);
        Assert.Equal(2, request.WorkersQuantity);
        Assert.Equal(1, request.HiredRunners);
        Assert.Equal(3, request.TotalRunners);
        Assert.True(request.IsDirectHiring);
        Assert.True(request.UsesRunners);
        Assert.Equal(Startup.FakeLaterRequest.StartAt, request.StartAt);

        AgencyRequestRunnersModel readOnly = page.Items.Single(r => r.RequestId == Startup.FakeRequestWithoutRunners.Id);
        Assert.False(readOnly.UsesRunners);
    }

    [Fact]
    public async Task GetReturnsRunnerDetails()
    {
        AgencyRunnersPagedResponse page = await Get();
        List<AgencyRunnerListModel> runners = page.Items.SelectMany(r => r.Runners).ToList();

        AgencyRunnerListModel interviewed = runners.Single(r => r.Id == Startup.FakeRunnerInterviewed.Id);
        Assert.Equal("Ivy Interviewed", interviewed.Name);
        Assert.Equal(Startup.FakeWorkerInterviewed.Worker.Email, interviewed.Email);
        Assert.Equal(RunnerStatus.InterviewScheduled, interviewed.Status);
        Assert.Equal(1, interviewed.InterviewsCount);
        Assert.Equal(Startup.FakeInterviewDate, interviewed.LastInterviewAt);
        Assert.Equal(Startup.FakeRecruiterUser.Email, interviewed.CreatedByEmail);
        Assert.Equal(Startup.FakeLaterRequest.NumberId, interviewed.RequestNumberId);

        AgencyRunnerListModel hired = runners.Single(r => r.Id == Startup.FakeRunnerHired.Id);
        Assert.Equal(RunnerStatus.Hired, hired.Status);
        Assert.Equal(Startup.FakeHiredStartDate, hired.StartDate);
        Assert.Null(hired.LastInterviewAt);
    }

    [Fact]
    public async Task GetFiltersByStatusAndType()
    {
        AgencyRunnersPagedResponse page = await Get($"?statuses={(int)RunnerStatus.SentToClient}&type={(int)RunnerType.Passive}");
        List<AgencyRunnerListModel> runners = page.Items.SelectMany(r => r.Runners).ToList();
        Assert.NotEmpty(runners);
        Assert.All(runners, r =>
        {
            Assert.Equal(RunnerStatus.SentToClient, r.Status);
            Assert.Equal(RunnerType.Passive, r.Type);
        });
        Assert.Contains(runners, r => r.Id == Startup.FakeRunnerPassive.Id);
        Assert.DoesNotContain(page.Items, r => r.RequestId == Startup.FakeLaterRequest.Id);
    }

    [Fact]
    public async Task GetFiltersByName()
    {
        AgencyRunnersPagedResponse page = await Get("?name=ivy");
        AgencyRequestRunnersModel request = Assert.Single(page.Items);
        AgencyRunnerListModel runner = Assert.Single(request.Runners);
        Assert.Equal(Startup.FakeRunnerInterviewed.Id, runner.Id);
        Assert.Equal(3, request.TotalRunners);
    }

    [Fact]
    public async Task GetFileReturnsTheExcelOfEveryRunnerOfTheFilter()
    {
        var response = await _client.GetAsync($"{RecruitingRunnersController.RouteName}/File");
        response.EnsureSuccessStatusCode();
        Assert.Equal(CovenantConstants.ExcelMime, response.Content.Headers.ContentType?.MediaType);
        Assert.NotEmpty(await response.Content.ReadAsByteArrayAsync());
    }

    [Fact]
    public async Task ChangeStatusUpdatesTheValidRunnersAndReportsTheRest()
    {
        Guid missingId = Guid.NewGuid();
        var model = new ChangeRunnersStatusModel
        {
            RunnerIds = [Startup.FakeBulkSent.Id, Startup.FakeBulkHired.Id, Startup.FakeBulkWithoutRunners.Id, missingId],
            Status = RunnerStatus.WaitingForFinalDecision,
            Comments = "Client is deciding"
        };
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{RecruitingRunnersController.RouteName}/Status", model);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ChangeRunnersStatusResultModel>();
        Assert.Equal(1, result.Updated);
        Assert.Equal(3, result.Skipped.Count);
        Assert.Contains(result.Skipped, s => s.RunnerId == Startup.FakeBulkHired.Id && s.Reason.Contains("hired"));
        Assert.Contains(result.Skipped, s => s.RunnerId == Startup.FakeBulkWithoutRunners.Id && s.Reason.Contains("runners"));
        Assert.Contains(result.Skipped, s => s.RunnerId == missingId);

        Runner updated = await GetRunner(Startup.FakeBulkSent.Id);
        Assert.Equal(RunnerStatus.WaitingForFinalDecision, updated.Status);
        Assert.Equal(Startup.UserId, updated.UpdatedBy);
        RunnerStatusHistory history = updated.StatusHistory.Last();
        Assert.Equal(RunnerStatus.WaitingForFinalDecision, history.NewStatus);
        Assert.Equal(model.Comments, history.Comments);
    }

    [Fact]
    public async Task ChangeStatusToHiredSetsTheStartDate()
    {
        var startDate = new DateTime(2026, 11, 2);
        var model = new ChangeRunnersStatusModel
        {
            RunnerIds = [Startup.FakeBulkHire.Id],
            Status = RunnerStatus.Hired,
            StartDate = startDate
        };
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{RecruitingRunnersController.RouteName}/Status", model);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ChangeRunnersStatusResultModel>();
        Assert.Equal(1, result.Updated);
        Assert.Empty(result.Skipped);

        Runner hired = await GetRunner(Startup.FakeBulkHire.Id);
        Assert.Equal(RunnerStatus.Hired, hired.Status);
        Assert.Equal(startDate, hired.StartDate);
    }

    [Fact]
    public async Task ChangeStatusToHiredWithoutStartDateReturnsBadRequest()
    {
        var model = new ChangeRunnersStatusModel { RunnerIds = [Startup.FakeBulkSent.Id], Status = RunnerStatus.Hired };
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{RecruitingRunnersController.RouteName}/Status", model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ChangeStatusToInterviewRescheduledReturnsBadRequest()
    {
        var model = new ChangeRunnersStatusModel { RunnerIds = [Startup.FakeBulkSent.Id], Status = RunnerStatus.InterviewRescheduled };
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{RecruitingRunnersController.RouteName}/Status", model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder()
                .AddTestAuth(o =>
                {
                    o.AddSub(UserId);
                    o.AddAgencyPersonnelRole(AgencyId);
                    o.AddName(FakeRecruiterUser.Email);
                });
            services.AddTestDatabase();
            services.AddSingleton<IUserAccountService, UserAccountService>();
            services.AddSingleton<IRequestRepository, RequestRepository>();
            services.AddSingleton<IRunnerRepository, RunnerRepository>();
            services.AddSingleton<IWorkerRepository, WorkerRepository>();
            services.AddSingleton<IRunnerService, RunnerService>();
            services.AddSingleton<AgencyIdFilter>();
        }

        public static readonly User FakeRecruiterUser = new(CvnEmail.Create("runners.recruiter@mail.com").Value);
        public static readonly Guid UserId = FakeRecruiterUser.Id;

        public static readonly Request FakeSoonRequest = FakeData.FakeRequest(startAt: DateTime.Today.AddDays(2), workersQuantity: 1);

        private static readonly Guid AgencyId = FakeSoonRequest.CompanyProfile.AgencyId;

        public static readonly AgencyPersonnel FakeRecruiter =
            AgencyPersonnel.CreatePrimary(AgencyId, FakeRecruiterUser.Id, FakeRecruiterUser.Email);

        public static readonly Request FakeLaterRequest = FakeData.FakeRequest(agencyId: AgencyId, startAt: DateTime.Today.AddDays(20), workersQuantity: 2);
        public static readonly Request FakeRequestWithoutRunners = FakeData.FakeRequest(agencyId: AgencyId, startAt: DateTime.Today.AddDays(40));

        public static readonly DateTime FakeInterviewDate = new(2026, 10, 20, 10, 0, 0);
        public static readonly DateTime FakeHiredStartDate = new(2026, 11, 1);

        public static readonly WorkerProfile FakeWorkerInterviewed = FakeWorker("Ivy", "Interviewed", "ivy.interviewed@mail.com");
        public static readonly WorkerProfile FakeWorkerHired = FakeWorker("Hal", "Hired", "hal.hired@mail.com");
        public static readonly WorkerProfile FakeWorkerLater = FakeWorker("Lena", "Later", "lena.later@mail.com");
        public static readonly WorkerProfile FakeWorkerPassive = FakeWorker("Pam", "Passive", "pam.passive@mail.com");
        public static readonly WorkerProfile FakeWorkerBulkSent = FakeWorker("Sid", "Sent", "sid.sent@mail.com");
        public static readonly WorkerProfile FakeWorkerBulkHired = FakeWorker("Bea", "Hired", "bea.hired@mail.com");
        public static readonly WorkerProfile FakeWorkerBulkHire = FakeWorker("Hugo", "Hire", "hugo.hire@mail.com");
        public static readonly WorkerProfile FakeWorkerBulkWithoutRunners = FakeWorker("Nora", "Norunners", "nora.norunners@mail.com");

        public static readonly Runner FakeRunnerInterviewed = FakeRunner(FakeLaterRequest, FakeWorkerInterviewed);
        public static readonly Runner FakeRunnerHired = FakeRunner(FakeLaterRequest, FakeWorkerHired);
        public static readonly Runner FakeRunnerLater = FakeRunner(FakeLaterRequest, FakeWorkerLater);
        public static readonly Runner FakeRunnerPassive = FakeRunner(FakeSoonRequest, FakeWorkerPassive, RunnerType.Passive);
        public static readonly Runner FakeBulkSent = FakeRunner(FakeSoonRequest, FakeWorkerBulkSent);
        public static readonly Runner FakeBulkHired = FakeRunner(FakeSoonRequest, FakeWorkerBulkHired);
        public static readonly Runner FakeBulkHire = FakeRunner(FakeSoonRequest, FakeWorkerBulkHire);
        public static readonly Runner FakeBulkWithoutRunners = FakeRunner(FakeRequestWithoutRunners, FakeWorkerBulkWithoutRunners);

        static Startup()
        {
            FakeLaterRequest.WorkerSalary = 55000;
            FakeRequestWithoutRunners.UsesRunners = false;
            FakeRunnerInterviewed.ChangeStatus(RunnerStatus.InterviewScheduled, UserId, "Client wants to meet her");
            FakeRunnerInterviewed.AddInterview(FakeInterviewDate, InterviewType.Video, "Mr. Client", null, UserId);
            FakeRunnerHired.ChangeStatus(RunnerStatus.Hired, UserId, startDate: FakeHiredStartDate);
            FakeBulkHired.ChangeStatus(RunnerStatus.Hired, UserId, startDate: FakeHiredStartDate);
        }

        private static WorkerProfile FakeWorker(string firstName, string lastName, string email) =>
            new(new User(CvnEmail.Create(email).Value), AgencyId)
            {
                FirstName = firstName,
                LastName = lastName,
                ApprovedToWork = true,
                Location = FakeData.FakeLocation()
            };

        private static Runner FakeRunner(Request request, WorkerProfile worker, RunnerType type = RunnerType.Active) =>
            Runner.CreateFromWorker(request.Id, worker.Id, type, UserId).Value;

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
            context.Users.Add(FakeRecruiterUser);
            context.AgencyPersonnel.Add(FakeRecruiter);
            context.Requests.AddRange(FakeSoonRequest, FakeLaterRequest, FakeRequestWithoutRunners);
            context.WorkerProfiles.AddRange(FakeWorkerInterviewed, FakeWorkerHired, FakeWorkerLater, FakeWorkerPassive,
                FakeWorkerBulkSent, FakeWorkerBulkHired, FakeWorkerBulkHire, FakeWorkerBulkWithoutRunners);
            context.Runners.AddRange(FakeRunnerInterviewed, FakeRunnerHired, FakeRunnerLater, FakeRunnerPassive,
                FakeBulkSent, FakeBulkHired, FakeBulkHire, FakeBulkWithoutRunners);
            context.SaveChanges();
        }
    }
}
