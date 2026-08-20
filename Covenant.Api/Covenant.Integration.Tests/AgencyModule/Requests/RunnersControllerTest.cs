using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Entities.Request.Runners;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Common.Models.Request.Runners;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Request;
using Covenant.Infrastructure.Repositories.Worker;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.Requests;

public class RunnersControllerTest : IClassFixture<CustomWebApplicationFactory<RunnersControllerTest.Startup>>
{
    private readonly CustomWebApplicationFactory<Startup> _factory;
    private readonly HttpClient _client;

    public RunnersControllerTest(CustomWebApplicationFactory<Startup> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private static string RequestUri(Guid requestId = default) =>
        $"api/agency/requests/{(requestId == default ? Startup.FakeRequest.Id : requestId)}/Runners";

    private CovenantContext Context => _factory.Server.Host.Services.GetRequiredService<CovenantContext>();

    private Task<Runner> GetRunner(Guid id) => Context.Runners
        .Include(r => r.StatusHistory)
        .Include(r => r.Interviews)
        .SingleAsync(r => r.Id == id);

    [Fact]
    public async Task Search()
    {
        HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/Search?searchTerm=prospect");
        response.EnsureSuccessStatusCode();
        var results = await response.Content.ReadFromJsonAsync<List<ApplicantSearchResultModel>>();
        ApplicantSearchResultModel prospect = Assert.Single(results);
        Assert.Equal(Startup.FakeWorkerSearch.Id, prospect.WorkerProfileId);
        Assert.Equal("Ready Prospect", prospect.Name);
        Assert.Equal(Startup.FakeWorkerSearch.Worker.Email, prospect.Email);
        Assert.Null(prospect.CandidateId);
    }

    [Fact]
    public async Task Get()
    {
        HttpResponseMessage response = await _client.GetAsync(RequestUri());
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<RunnerListModel>>();
        RunnerListModel model = list.Items.Single(r => r.Id == Startup.FakeRunnerList.Id);
        Assert.Equal(Startup.FakeRequest.Id, model.RequestId);
        Assert.Equal(Startup.FakeWorkerList.Id, model.WorkerProfileId);
        Assert.Equal("Alan List", model.Name);
        Assert.Equal(Startup.FakeWorkerList.Worker.Email, model.Email);
        Assert.Equal(RunnerType.Active, model.Type);
        Assert.Equal(RunnerStatus.SentToClient, model.Status);
        Assert.Equal(Startup.UserId, model.CreatedBy);
        Assert.Equal(0, model.InterviewsCount);
        Assert.All(list.Items, i => Assert.Equal(Startup.FakeRequest.Id, i.RequestId));
    }

    [Fact]
    public async Task GetFiltersByStatusAndType()
    {
        HttpResponseMessage response = await _client.GetAsync(
            $"{RequestUri()}?statuses={(int)RunnerStatus.InterviewScheduled}&type={(int)RunnerType.Passive}");
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<RunnerListModel>>();
        Assert.NotEmpty(list.Items);
        Assert.All(list.Items, i =>
        {
            Assert.Equal(RunnerStatus.InterviewScheduled, i.Status);
            Assert.Equal(RunnerType.Passive, i.Type);
        });
        Assert.Contains(list.Items, i => i.Id == Startup.FakeRunnerFiltered.Id);
    }

    [Fact]
    public async Task GetFiltersByName()
    {
        HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}?name=alan");
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<RunnerListModel>>();
        RunnerListModel model = Assert.Single(list.Items);
        Assert.Equal(Startup.FakeRunnerList.Id, model.Id);
    }

    [Fact]
    public async Task GetById()
    {
        HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/{Startup.FakeRunnerDetail.Id}");
        response.EnsureSuccessStatusCode();
        var detail = await response.Content.ReadFromJsonAsync<RunnerDetailModel>();
        Assert.Equal(Startup.FakeRunnerDetail.Id, detail.Id);
        Assert.Equal(Startup.FakeRequest.Id, detail.RequestId);
        Assert.Equal(Startup.FakeWorkerDetail.Id, detail.WorkerProfileId);
        Assert.Equal("Diana Detail", detail.Name);
        Assert.Equal(Startup.FakeWorkerDetail.Worker.Email, detail.Email);
        Assert.Equal(RunnerStatus.InterviewScheduled, detail.Status);
        Assert.Equal(2, detail.StatusHistory.Count());
        RunnerStatusHistoryModel last = detail.StatusHistory.First();
        Assert.Equal(RunnerStatus.SentToClient, last.PreviousStatus);
        Assert.Equal(RunnerStatus.InterviewScheduled, last.NewStatus);
        Assert.Equal("Client wants to meet her", last.Comments);
        Assert.Equal(Startup.FakeRecruiterUser.Email, last.ChangedByEmail);
        RunnerInterviewModel interview = Assert.Single(detail.Interviews);
        Assert.Equal(InterviewType.Video, interview.Type);
        Assert.Equal("Mr. Client", interview.Interviewer);
        Assert.Equal(InterviewStatus.Scheduled, interview.Status);
        Assert.Equal(0, interview.RescheduleCount);
    }

    [Fact]
    public async Task GetByIdReturnsNotFound()
    {
        HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post()
    {
        var model = new RunnerCreateModel { WorkerProfileId = Startup.FakeWorkerPost.Id, Type = RunnerType.Passive };
        HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(_client, RequestUri(), model);
        response.EnsureSuccessStatusCode();
        var id = await response.Content.ReadFromJsonAsync<Guid>();
        Runner entity = await GetRunner(id);
        Assert.Equal(Startup.FakeRequest.Id, entity.RequestId);
        Assert.Equal(model.WorkerProfileId, entity.WorkerProfileId);
        Assert.Equal(RunnerType.Passive, entity.Type);
        Assert.Equal(RunnerStatus.SentToClient, entity.Status);
        Assert.Equal(Startup.UserId, entity.CreatedBy);
        Assert.NotEqual(default, entity.CreatedAt);
        RunnerStatusHistory history = Assert.Single(entity.StatusHistory);
        Assert.Null(history.PreviousStatus);
        Assert.Equal(RunnerStatus.SentToClient, history.NewStatus);
    }

    [Fact]
    public async Task PostReturnsBadRequestWhenWorkerIsAlreadyRunner()
    {
        var model = new RunnerCreateModel { WorkerProfileId = Startup.FakeWorkerDuplicated.Id, Type = RunnerType.Active };
        HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(_client, RequestUri(), model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostReturnsBadRequestWhenRequestDoesNotUseRunners()
    {
        var model = new RunnerCreateModel { WorkerProfileId = Startup.FakeWorkerPost.Id, Type = RunnerType.Active };
        HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(_client,
            RequestUri(Startup.FakeRequestWithoutRunners.Id), model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostReturnsBadRequestWhenWorkerBelongsToAnotherAgency()
    {
        var model = new RunnerCreateModel { WorkerProfileId = Startup.FakeWorkerOtherAgency.Id, Type = RunnerType.Active };
        HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(_client, RequestUri(), model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ChangeStatus()
    {
        Guid id = Startup.FakeRunnerStatus.Id;
        var model = new ChangeRunnerStatusModel { Status = RunnerStatus.WaitingForFinalDecision, Comments = "Waiting for the client" };
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{RequestUri()}/{id}/Status", model);
        response.EnsureSuccessStatusCode();
        Runner entity = await GetRunner(id);
        Assert.Equal(RunnerStatus.WaitingForFinalDecision, entity.Status);
        Assert.Equal(Startup.UserId, entity.UpdatedBy);
        Assert.Equal(2, entity.StatusHistory.Count());
        RunnerStatusHistory history = entity.StatusHistory.Last();
        Assert.Equal(RunnerStatus.SentToClient, history.PreviousStatus);
        Assert.Equal(RunnerStatus.WaitingForFinalDecision, history.NewStatus);
        Assert.Equal(model.Comments, history.Comments);
    }

    [Fact]
    public async Task ChangeStatusToHiredSetsStartDate()
    {
        Guid id = Startup.FakeRunnerHire.Id;
        var startDate = new DateTime(2026, 9, 1);
        var model = new ChangeRunnerStatusModel { Status = RunnerStatus.Hired, StartDate = startDate };
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{RequestUri()}/{id}/Status", model);
        response.EnsureSuccessStatusCode();
        Runner entity = await GetRunner(id);
        Assert.Equal(RunnerStatus.Hired, entity.Status);
        Assert.Equal(startDate, entity.StartDate);
    }

    [Fact]
    public async Task ChangeStatusToHiredWithoutStartDateReturnsBadRequest()
    {
        var model = new ChangeRunnerStatusModel { Status = RunnerStatus.Hired };
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{RequestUri()}/{Startup.FakeRunnerHireNoDate.Id}/Status", model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ChangeStatusOfHiredRunnerReturnsBadRequest()
    {
        var model = new ChangeRunnerStatusModel { Status = RunnerStatus.Rejected };
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{RequestUri()}/{Startup.FakeRunnerHired.Id}/Status", model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ChangeStatusReturnsBadRequestWhenRequestDoesNotUseRunners()
    {
        var model = new ChangeRunnerStatusModel { Status = RunnerStatus.Rejected };
        HttpResponseMessage response = await _client.PutAsJsonAsync(
            $"{RequestUri(Startup.FakeRequestWithoutRunners.Id)}/{Startup.FakeRunnerWithoutRunners.Id}/Status", model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddInterview()
    {
        Guid id = Startup.FakeRunnerInterview.Id;
        var model = new RunnerInterviewCreateModel
        {
            ScheduledDate = new DateTime(2026, 8, 20, 10, 0, 0),
            Type = InterviewType.Onsite,
            Interviewer = "Plant manager",
            Notes = "Bring safety shoes"
        };
        HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(_client, $"{RequestUri()}/{id}/Interview", model);
        response.EnsureSuccessStatusCode();
        var interviewId = await response.Content.ReadFromJsonAsync<Guid>();
        RunnerInterview entity = await Context.RunnerInterviews.SingleAsync(i => i.Id == interviewId);
        Assert.Equal(id, entity.RunnerId);
        Assert.Equal(model.ScheduledDate, entity.ScheduledDate);
        Assert.Equal(model.Type, entity.Type);
        Assert.Equal(model.Interviewer, entity.Interviewer);
        Assert.Equal(model.Notes, entity.Notes);
        Assert.Equal(InterviewStatus.Scheduled, entity.Status);
        Assert.Equal(0, entity.RescheduleCount);
        Assert.Equal(Startup.UserId, entity.CreatedBy);
    }

    [Fact]
    public async Task AddInterviewReturnsBadRequestWhenStatusDoesNotAllowIt()
    {
        var model = new RunnerInterviewCreateModel
        {
            ScheduledDate = new DateTime(2026, 8, 20),
            Type = InterviewType.Phone,
            Interviewer = "Recruiter"
        };
        HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(_client,
            $"{RequestUri()}/{Startup.FakeRunnerNoInterview.Id}/Interview", model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RescheduleInterview()
    {
        Guid id = Startup.FakeRunnerReschedule.Id;
        Guid interviewId = Startup.FakeRescheduleInterview.Id;
        var model = new RunnerInterviewRescheduleModel { NewDate = new DateTime(2026, 8, 25, 15, 0, 0) };
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{RequestUri()}/{id}/Interview/{interviewId}/Reschedule", model);
        response.EnsureSuccessStatusCode();
        Runner entity = await GetRunner(id);
        RunnerInterview interview = entity.Interviews.Single(i => i.Id == interviewId);
        Assert.Equal(model.NewDate, interview.ScheduledDate);
        Assert.Equal(InterviewStatus.Rescheduled, interview.Status);
        Assert.Equal(1, interview.RescheduleCount);
        Assert.Equal(Startup.UserId, interview.RescheduledBy);
        Assert.Equal(RunnerStatus.InterviewRescheduled, entity.Status);
        Assert.Contains(entity.StatusHistory, h => h.NewStatus == RunnerStatus.InterviewRescheduled);
    }

    [Fact]
    public async Task RescheduleInterviewReturnsBadRequestWhenInterviewDoesNotExist()
    {
        var model = new RunnerInterviewRescheduleModel { NewDate = new DateTime(2026, 8, 25) };
        HttpResponseMessage response = await _client.PutAsJsonAsync(
            $"{RequestUri()}/{Startup.FakeRunnerInterview.Id}/Interview/{Guid.NewGuid()}/Reschedule", model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete()
    {
        Guid id = Startup.FakeRunnerDelete.Id;
        CovenantContext context = Context;
        Assert.True(await context.Runners.AnyAsync(r => r.Id == id));
        HttpResponseMessage response = await _client.DeleteAsync($"{RequestUri()}/{id}");
        response.EnsureSuccessStatusCode();
        Assert.False(await context.Runners.AnyAsync(r => r.Id == id));
        Assert.False(await context.RunnerStatusHistories.AnyAsync(h => h.RunnerId == id));
    }

    [Fact]
    public async Task DeleteReturnsBadRequestWhenRunnerDoesNotExist()
    {
        HttpResponseMessage response = await _client.DeleteAsync($"{RequestUri()}/{Guid.NewGuid()}");
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
            services.AddDbContext<CovenantContext>(b
                => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
            services.AddSingleton<ITimeService, TimeService>();
            services.AddSingleton<IIdentityServerService, IdentityServerService>();
            services.AddSingleton<IRequestRepository, RequestRepository>();
            services.AddSingleton<IRunnerRepository, RunnerRepository>();
            services.AddSingleton<IWorkerRepository, WorkerRepository>();
            services.AddSingleton<IRunnerService, RunnerService>();
            services.AddSingleton<AgencyIdFilter>();
        }

        private static readonly Guid AgencyId = Guid.NewGuid();
        private static readonly Guid OtherAgencyId = Guid.NewGuid();

        public static readonly User FakeRecruiterUser = new(CvnEmail.Create("recruiter@mail.com").Value);
        public static readonly Guid UserId = FakeRecruiterUser.Id;
        public static readonly AgencyPersonnel FakeRecruiter =
            AgencyPersonnel.CreatePrimary(AgencyId, FakeRecruiterUser.Id, FakeRecruiterUser.Email);

        public static readonly Covenant.Common.Entities.Request.Request FakeRequest = FakeData.FakeRequest(AgencyId);
        public static readonly Covenant.Common.Entities.Request.Request FakeRequestWithoutRunners = FakeData.FakeRequest(AgencyId);

        public static readonly WorkerProfile FakeWorkerSearch = FakeWorker("Ready", "Prospect", "ready.prospect@mail.com");
        public static readonly WorkerProfile FakeWorkerSearchTaken = FakeWorker("Taken", "Prospect", "taken.prospect@mail.com");
        public static readonly WorkerProfile FakeWorkerPost = FakeWorker("Peter", "Post", "peter.post@mail.com");
        public static readonly WorkerProfile FakeWorkerOtherAgency = FakeWorker("Otto", "Other", "otto.other@mail.com", OtherAgencyId);
        public static readonly WorkerProfile FakeWorkerDuplicated = FakeWorker("Dana", "Duplicated", "dana.dup@mail.com");
        public static readonly WorkerProfile FakeWorkerList = FakeWorker("Alan", "List", "alan.list@mail.com");
        public static readonly WorkerProfile FakeWorkerFiltered = FakeWorker("Bella", "Filtered", "bella.filtered@mail.com");
        public static readonly WorkerProfile FakeWorkerDetail = FakeWorker("Diana", "Detail", "diana.detail@mail.com");
        public static readonly WorkerProfile FakeWorkerStatus = FakeWorker("Sam", "Status", "sam.status@mail.com");
        public static readonly WorkerProfile FakeWorkerHire = FakeWorker("Hank", "Hire", "hank.hire@mail.com");
        public static readonly WorkerProfile FakeWorkerHireNoDate = FakeWorker("Hilda", "Nodate", "hilda.nodate@mail.com");
        public static readonly WorkerProfile FakeWorkerHired = FakeWorker("Harry", "Hired", "harry.hired@mail.com");
        public static readonly WorkerProfile FakeWorkerInterview = FakeWorker("Ivan", "Interview", "ivan.interview@mail.com");
        public static readonly WorkerProfile FakeWorkerNoInterview = FakeWorker("Nina", "Nointerview", "nina.nointerview@mail.com");
        public static readonly WorkerProfile FakeWorkerReschedule = FakeWorker("Rita", "Reschedule", "rita.reschedule@mail.com");
        public static readonly WorkerProfile FakeWorkerDelete = FakeWorker("Derek", "Delete", "derek.delete@mail.com");
        public static readonly WorkerProfile FakeWorkerWithoutRunners = FakeWorker("Wanda", "Norunners", "wanda.norunners@mail.com");

        public static readonly Runner FakeRunnerSearchTaken = FakeRunner(FakeWorkerSearchTaken);
        public static readonly Runner FakeRunnerDuplicated = FakeRunner(FakeWorkerDuplicated);
        public static readonly Runner FakeRunnerList = FakeRunner(FakeWorkerList);
        public static readonly Runner FakeRunnerFiltered = FakeRunner(FakeWorkerFiltered, RunnerType.Passive);
        public static readonly Runner FakeRunnerDetail = FakeRunner(FakeWorkerDetail);
        public static readonly Runner FakeRunnerStatus = FakeRunner(FakeWorkerStatus);
        public static readonly Runner FakeRunnerHire = FakeRunner(FakeWorkerHire);
        public static readonly Runner FakeRunnerHireNoDate = FakeRunner(FakeWorkerHireNoDate);
        public static readonly Runner FakeRunnerHired = FakeRunner(FakeWorkerHired);
        public static readonly Runner FakeRunnerInterview = FakeRunner(FakeWorkerInterview);
        public static readonly Runner FakeRunnerNoInterview = FakeRunner(FakeWorkerNoInterview);
        public static readonly Runner FakeRunnerReschedule = FakeRunner(FakeWorkerReschedule);
        public static readonly Runner FakeRunnerDelete = FakeRunner(FakeWorkerDelete);
        public static readonly Runner FakeRunnerWithoutRunners =
            Runner.CreateFromWorker(FakeRequestWithoutRunners.Id, FakeWorkerWithoutRunners.Id, RunnerType.Active, UserId).Value;

        public static readonly RunnerInterview FakeRescheduleInterview;

        static Startup()
        {
            FakeRequestWithoutRunners.UsesRunners = false;
            FakeRunnerFiltered.ChangeStatus(RunnerStatus.InterviewScheduled, UserId);
            FakeRunnerDetail.ChangeStatus(RunnerStatus.InterviewScheduled, UserId, "Client wants to meet her");
            FakeRunnerDetail.AddInterview(new DateTime(2026, 8, 18, 9, 0, 0), InterviewType.Video, "Mr. Client", "Second round", UserId);
            FakeRunnerHired.ChangeStatus(RunnerStatus.Hired, UserId, startDate: new DateTime(2026, 8, 1));
            FakeRunnerInterview.ChangeStatus(RunnerStatus.InterviewScheduled, UserId);
            FakeRunnerReschedule.ChangeStatus(RunnerStatus.InterviewScheduled, UserId);
            FakeRescheduleInterview = FakeRunnerReschedule
                .AddInterview(new DateTime(2026, 8, 19, 11, 0, 0), InterviewType.Phone, "Recruiter", null, UserId).Value;
        }

        private static WorkerProfile FakeWorker(string firstName, string lastName, string email, Guid agencyId = default) =>
            new(new User(CvnEmail.Create(email).Value), agencyId == default ? AgencyId : agencyId)
            {
                FirstName = firstName,
                LastName = lastName,
                ApprovedToWork = true,
                Location = FakeData.FakeLocation()
            };

        private static Runner FakeRunner(WorkerProfile worker, RunnerType type = RunnerType.Active) =>
            Runner.CreateFromWorker(FakeRequest.Id, worker.Id, type, UserId).Value;

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
            context.Agencies.Add(FakeData.FakeAgency(AgencyId));
            context.Users.Add(FakeRecruiterUser);
            context.AgencyPersonnel.Add(FakeRecruiter);
            context.Requests.AddRange(FakeRequest, FakeRequestWithoutRunners);
            context.WorkerProfiles.AddRange(FakeWorkerSearch, FakeWorkerSearchTaken, FakeWorkerPost, FakeWorkerOtherAgency,
                FakeWorkerDuplicated, FakeWorkerList, FakeWorkerFiltered, FakeWorkerDetail, FakeWorkerStatus, FakeWorkerHire,
                FakeWorkerHireNoDate, FakeWorkerHired, FakeWorkerInterview, FakeWorkerNoInterview, FakeWorkerReschedule,
                FakeWorkerDelete, FakeWorkerWithoutRunners);
            context.Runners.AddRange(FakeRunnerSearchTaken, FakeRunnerDuplicated, FakeRunnerList, FakeRunnerFiltered,
                FakeRunnerDetail, FakeRunnerStatus, FakeRunnerHire, FakeRunnerHireNoDate, FakeRunnerHired, FakeRunnerInterview,
                FakeRunnerNoInterview, FakeRunnerReschedule, FakeRunnerDelete, FakeRunnerWithoutRunners);
            context.SaveChanges();
        }
    }
}
