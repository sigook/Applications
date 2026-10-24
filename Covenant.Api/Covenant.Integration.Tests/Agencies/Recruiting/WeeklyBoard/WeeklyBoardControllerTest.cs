using Covenant.Api.Authorization;
using Covenant.Api.Controllers.Agencies.Recruiting.WeeklyBoard;
using Covenant.Common.Entities.Agencies;
using Covenant.Common.Entities.Requests;
using Covenant.Common.Entities.Workers;
using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Requests.WeeklyBoard;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using Moq;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.Agencies.Recruiting.WeeklyBoard;

public class WeeklyBoardControllerTest : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<WeeklyBoardControllerTest.Startup, WeeklyBoardControllerTest.Data>>
{
    private static readonly DateTime Monday = new(2026, 10, 19);

    private readonly SeededWebApplicationFactory<Startup, Data> _factory;
    private readonly Data _data;
    private readonly HttpClient _client;

    public WeeklyBoardControllerTest(SeededWebApplicationFactory<Startup, Data> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _data = factory.Data;
    }

    private CovenantContext Context => _factory.Services.GetRequiredService<CovenantContext>();

    private static string Week(DateTime from) => $"?from={from:yyyy-MM-dd}&to={from.AddDays(6):yyyy-MM-dd}";

    private Task<HttpResponseMessage> Assign(Request request, IEnumerable<AgencyPersonnel> recruiters, params DateTime[] days) =>
        _client.PostAsJsonAsync(WeeklyBoardController.RouteName, new AssignRecruitersModel
        {
            RequestId = request.Id,
            RecruiterIds = [.. recruiters.Select(r => r.Id)],
            WorkDates = days
        });

    private async Task<WeeklyBoardModel> Board(DateTime from) =>
        await _client.GetFromJsonAsync<WeeklyBoardModel>($"{WeeklyBoardController.RouteName}{Week(from)}");

    private Task<List<RequestRecruiter>> Assignments(Request request) =>
        Context.RequestRecruiters.AsNoTracking().Where(r => r.RequestId == request.Id).ToListAsync();

    [Fact]
    public async Task Assign_ShowsOneAssignmentPerRecruiterAndDay_GroupedByRecruiter()
    {
        Request request = _data.ForAssign;

        (await Assign(request, [Data.Alice, Data.Bob], Monday, Monday.AddDays(1))).EnsureSuccessStatusCode();

        Assert.Equal(4, (await Assignments(request)).Count);
        WeeklyBoardModel board = await Board(Monday);
        Assert.Equal(Monday, board.WeekStart);
        WeeklyBoardRecruiterRowModel alice = board.Recruiters.Single(r => r.RecruiterId == Data.Alice.Id);
        Assert.Equal(Data.Alice.Name, alice.RecruiterName);
        var aliceAssignments = alice.Assignments.Where(a => a.RequestId == request.Id).ToList();
        Assert.Equal([Monday, Monday.AddDays(1)], aliceAssignments.Select(a => a.WorkDate));
        Assert.Equal(request.NumberId, aliceAssignments[0].NumberId);
        Assert.Contains(board.Recruiters, r => r.RecruiterId == Data.Bob.Id);
        Assert.True(board.Recruiters.Select(r => r.RecruiterName).SequenceEqual(board.Recruiters.Select(r => r.RecruiterName).Order()));
    }

    [Fact]
    public async Task Assign_OutsideRequestedWeek_IsNotListed()
    {
        Request request = _data.ForOtherWeek;

        (await Assign(request, [Data.Alice], Monday.AddDays(14))).EnsureSuccessStatusCode();

        WeeklyBoardModel board = await Board(Monday);
        Assert.DoesNotContain(board.Recruiters.SelectMany(r => r.Assignments), a => a.RequestId == request.Id);
        WeeklyBoardModel later = await Board(Monday.AddDays(14));
        Assert.Contains(later.Recruiters.SelectMany(r => r.Assignments), a => a.RequestId == request.Id);
    }

    [Fact]
    public async Task Assign_SameRecruiterAndDayTwice_ReturnsBadRequest()
    {
        Request request = _data.ForDuplicate;
        (await Assign(request, [Data.Alice], Monday)).EnsureSuccessStatusCode();

        HttpResponseMessage response = await Assign(request, [Data.Alice], Monday);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Single(await Assignments(request));
    }

    [Fact]
    public async Task Assign_UnknownRecruiter_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync(WeeklyBoardController.RouteName, new AssignRecruitersModel
        {
            RequestId = _data.ForUnknownRecruiter.Id,
            RecruiterIds = [Guid.NewGuid()],
            WorkDates = [Monday]
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Empty(await Assignments(_data.ForUnknownRecruiter));
    }

    [Fact]
    public async Task Unassign_RemovesOnlyThatDay()
    {
        Request request = _data.ForUnassign;
        (await Assign(request, [Data.Alice], Monday, Monday.AddDays(2))).EnsureSuccessStatusCode();

        HttpResponseMessage response = await _client.DeleteAsync(
            $"{WeeklyBoardController.RouteName}?requestId={request.Id}&recruiterId={Data.Alice.Id}&workDate={Monday:yyyy-MM-dd}");

        response.EnsureSuccessStatusCode();
        RequestRecruiter remaining = Assert.Single(await Assignments(request));
        Assert.Equal(Monday.AddDays(2), remaining.WorkDate);
    }

    [Fact]
    public async Task Move_ToAnotherRecruiterAndDay_UpdatesAssignment()
    {
        Request request = _data.ForMove;
        (await Assign(request, [Data.Alice], Monday)).EnsureSuccessStatusCode();

        HttpResponseMessage response = await _client.PostAsJsonAsync($"{WeeklyBoardController.RouteName}/move", new MoveAssignmentModel
        {
            RequestId = request.Id,
            FromRecruiterId = Data.Alice.Id,
            FromWorkDate = Monday,
            ToRecruiterId = Data.Bob.Id,
            ToWorkDate = Monday.AddDays(3)
        });

        response.EnsureSuccessStatusCode();
        RequestRecruiter moved = Assert.Single(await Assignments(request));
        Assert.Equal(Data.Bob.Id, moved.RecruiterId);
        Assert.Equal(Monday.AddDays(3), moved.WorkDate);
    }

    [Fact]
    public async Task Move_OntoExistingAssignment_ReturnsBadRequest()
    {
        Request request = _data.ForMoveConflict;
        (await Assign(request, [Data.Alice, Data.Bob], Monday)).EnsureSuccessStatusCode();

        HttpResponseMessage response = await _client.PostAsJsonAsync($"{WeeklyBoardController.RouteName}/move", new MoveAssignmentModel
        {
            RequestId = request.Id,
            FromRecruiterId = Data.Alice.Id,
            FromWorkDate = Monday,
            ToRecruiterId = Data.Bob.Id,
            ToWorkDate = Monday
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(await Assignments(request), a => a.RecruiterId == Data.Alice.Id);
    }

    [Fact]
    public async Task AddRunner_LinksRunnerToAssignment_AndAddsNote()
    {
        Request request = _data.ForRunner;
        (await Assign(request, [Data.Alice], Monday)).EnsureSuccessStatusCode();

        HttpResponseMessage response = await _client.PostAsJsonAsync($"{WeeklyBoardController.RouteName}/runner", new AddRunnerModel
        {
            RequestId = request.Id,
            WorkDate = Monday,
            WorkerProfileId = Data.Runner.Id,
            Type = RunnerType.Active
        });

        response.EnsureSuccessStatusCode();
        var mine = await _client.GetFromJsonAsync<RecruiterWeeklyBoardModel>($"{WeeklyBoardController.RouteName}/mine{Week(Monday)}");
        Assert.Equal(Data.Alice.Name, mine.RecruiterName);
        WeeklyBoardAssignmentModel assignment = mine.Assignments.Single(a => a.RequestId == request.Id);
        Assert.Equal(1, assignment.RunnersSent);
        WeeklyBoardRunnerModel runner = Assert.Single(assignment.Runners);
        Assert.Equal(Data.Runner.Id, runner.WorkerProfileId);
        Assert.Equal("Rita Runner", runner.FullName);

        var orderRunners = await _client.GetFromJsonAsync<List<WeeklyBoardRunnerModel>>($"{WeeklyBoardController.RouteName}/{request.Id}/runners");
        Assert.Equal(runner.RunnerId, Assert.Single(orderRunners).RunnerId);

        Assert.True(await Context.RequestNotes.AnyAsync(n => n.RequestId == request.Id && n.Note.Note == "Rita Runner was sent"));
    }

    [Fact]
    public async Task AddRunner_WithoutOwnAssignment_ReturnsBadRequest()
    {
        Request request = _data.ForRunnerWithoutAssignment;
        (await Assign(request, [Data.Bob], Monday)).EnsureSuccessStatusCode();

        HttpResponseMessage response = await _client.PostAsJsonAsync($"{WeeklyBoardController.RouteName}/runner", new AddRunnerModel
        {
            RequestId = request.Id,
            WorkDate = Monday,
            WorkerProfileId = Data.Runner.Id,
            Type = RunnerType.Active
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(await Context.Runners.AnyAsync(r => r.RequestId == request.Id));
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder().AddTestAuth(o =>
            {
                o.AddSub(Data.AliceUser.Id);
                o.AddAgencyPersonnelRole(Data.AgencyId);
            });
            services.AddTestDatabase();
            services.AddSingleton<AgencyIdFilter>();
            services.AddSingleton<TimeProvider>(new FakeTimeProvider(new DateTimeOffset(Monday, TimeSpan.Zero)));
            var currentUserService = new Mock<ICurrentUserService>();
            currentUserService.Setup(s => s.GetAgencyId()).Returns(Data.AgencyId);
            currentUserService.Setup(s => s.GetAgencyIds()).Returns([Data.AgencyId]);
            currentUserService.Setup(s => s.GetUserId()).Returns(Data.AliceUser.Id);
            services.AddSingleton(currentUserService.Object);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllerRoute("default", "{controller}/{action=Index}/{id?}"));
        }
    }

    public class Data : ITestData
    {
        private static readonly Request Anchor = FakeData.FakeRequest(startAt: Monday);
        public static readonly Guid AgencyId = Anchor.CompanyProfile.AgencyId;

        public static readonly User AliceUser = FakeData.FakeUser("alice.weeklyboard@mail.com");
        public static readonly User BobUser = FakeData.FakeUser("bob.weeklyboard@mail.com");
        public static readonly AgencyPersonnel Alice = AgencyPersonnel.CreatePrimary(AgencyId, AliceUser.Id, "Alice Recruiter");
        public static readonly AgencyPersonnel Bob = AgencyPersonnel.CreatePrimary(AgencyId, BobUser.Id, "Bob Recruiter");


        public static readonly WorkerProfile Runner = new(FakeData.FakeUser("rita.runner@mail.com"), AgencyId)
        {
            FirstName = "Rita",
            LastName = "Runner",
            ApprovedToWork = true,
            Location = FakeData.FakeLocation()
        };

        public Request ForAssign { get; } = Order();
        public Request ForOtherWeek { get; } = Order();
        public Request ForDuplicate { get; } = Order();
        public Request ForUnknownRecruiter { get; } = Order();
        public Request ForUnassign { get; } = Order();
        public Request ForMove { get; } = Order();
        public Request ForMoveConflict { get; } = Order();
        public Request ForRunner { get; } = Order();
        public Request ForRunnerWithoutAssignment { get; } = Order();

        private static Request Order() =>
            FakeData.FakeRequest(agencyId: AgencyId, companyProfileId: Anchor.CompanyProfileId, jobPositionRateId: Anchor.JobPositionRateId.GetValueOrDefault(), startAt: Monday);

        public void Seed(CovenantContext context)
        {
            context.Users.AddRange(AliceUser, BobUser);
            context.Requests.Add(Anchor);
            context.AgencyPersonnel.AddRange(Alice, Bob);
            context.WorkerProfiles.Add(Runner);
            context.Requests.AddRange(ForAssign, ForOtherWeek, ForDuplicate, ForUnknownRecruiter,
                ForUnassign, ForMove, ForMoveConflict, ForRunner, ForRunnerWithoutAssignment);
            context.SaveChanges();
        }
    }
}
