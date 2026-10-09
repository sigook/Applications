using Covenant.Api.Controllers.Agency.Profile;
using Covenant.Common.Constants;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Enums;
using Covenant.Common.Models.Agency;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.Extensions.Time.Testing;
using System.Net.Http.Json;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.Agency.Profile;

public class AttendanceControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<AttendanceControllerTest.Startup>>
{
    private const string RouteName = AttendanceController.RouteName;
    private const string TimeZone = "America/Bogota";
    private const string ReportUri = $"{RouteName}/report?from=2026-09-28&to=2026-10-04";

    private readonly CustomWebApplicationFactory<Startup> _factory;

    public AttendanceControllerTest(CustomWebApplicationFactory<Startup> factory) => _factory = factory;

    private HttpClient ClientAs(string role, Guid sub)
    {
        HttpClient client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.RoleHeader, role);
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.SubHeader, sub.ToString());
        return client;
    }

    private HttpClient AsAdmin() => ClientAs(CovenantConstants.Role.Admin, Data.AdminUser.Id);

    private static async Task<UserAttendanceTodayModel> Toggle(HttpClient client, DateTime now)
    {
        Data.SetNow(now);
        HttpResponseMessage response = await client.PostAsync($"{RouteName}?timeZone={TimeZone}", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserAttendanceTodayModel>();
    }

    [Fact]
    public async Task ClocksInThenOutOncePerDay()
    {
        HttpClient client = ClientAs(CovenantConstants.Role.Recruiting, Data.PunchUser.Id);

        UserAttendanceTodayModel started = await Toggle(client, new DateTime(2026, 10, 5, 8, 0, 0));
        Assert.Equal(AttendanceStatus.ClockedIn, started.Status);
        Assert.Null(started.WorkedHours);

        UserAttendanceTodayModel stopped = await Toggle(client, new DateTime(2026, 10, 5, 18, 30, 0));
        Assert.Equal(AttendanceStatus.ClockedOut, stopped.Status);
        Assert.Equal(9.5m, stopped.WorkedHours);

        HttpResponseMessage again = await client.PostAsync($"{RouteName}?timeZone={TimeZone}", null);
        Assert.Equal(HttpStatusCode.BadRequest, again.StatusCode);

        var today = await client.GetFromJsonAsync<UserAttendanceTodayModel>($"{RouteName}/today?timeZone={TimeZone}");
        Assert.Equal(AttendanceStatus.ClockedOut, today.Status);

        Data.SetNow(new DateTime(2026, 10, 6, 8, 0, 0));
        today = await client.GetFromJsonAsync<UserAttendanceTodayModel>($"{RouteName}/today?timeZone={TimeZone}");
        Assert.Equal(AttendanceStatus.NotStarted, today.Status);
    }

    [Fact]
    public async Task ClockRejectsAnUnknownTimeZone()
    {
        HttpClient client = ClientAs(CovenantConstants.Role.Recruiting, Data.PunchUser.Id);

        HttpResponseMessage response = await client.PostAsync($"{RouteName}?timeZone=Mars/Olympus", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReportSplitsRegularAndOvertime()
    {
        var report = await AsAdmin().GetFromJsonAsync<UserAttendanceReportModel>($"{ReportUri}&userId={Data.ReportUser.Id}");

        Assert.Equal(3, report.Items.Count);

        UserAttendanceListModel monday = report.Items.Single(i => i.Date == new DateTime(2026, 9, 28));
        Assert.Equal(9.5m, monday.WorkedHours);
        Assert.Equal(1m, monday.LunchHours);
        Assert.Equal(8m, monday.RegularHours);
        Assert.Equal(1.5m, monday.OvertimeHours);
        Assert.Equal("Report User", monday.Name);

        UserAttendanceListModel saturday = report.Items.Single(i => i.Date == new DateTime(2026, 10, 3));
        Assert.True(saturday.IsWeekend);
        Assert.Equal(0m, saturday.LunchHours);
        Assert.Equal(0m, saturday.RegularHours);
        Assert.Equal(3m, saturday.OvertimeHours);

        UserAttendanceListModel thursday = report.Items.Single(i => i.Date == new DateTime(2026, 10, 1));
        Assert.True(thursday.IsMissingClockOut);
        Assert.Equal(0m, thursday.WorkedHours);

        Assert.Equal(12.5m, report.Totals.WorkedHours);
        Assert.Equal(1m, report.Totals.LunchHours);
        Assert.Equal(8m, report.Totals.RegularHours);
        Assert.Equal(4.5m, report.Totals.OvertimeHours);
    }

    [Fact]
    public async Task ReportOnlyIncludesUsersOfTheCurrentAgency()
    {
        var report = await AsAdmin().GetFromJsonAsync<UserAttendanceReportModel>(ReportUri);

        Assert.DoesNotContain(report.Items, i => i.UserId == Data.ForeignUser.Id);
    }

    [Fact]
    public async Task ReportFileIsAnExcel()
    {
        HttpResponseMessage response = await AsAdmin().GetAsync($"{RouteName}/report/file?from=2026-09-28&to=2026-10-04");

        response.EnsureSuccessStatusCode();
        Assert.Equal(CovenantConstants.ExcelMime, response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task ReportRejectsAnInvertedRange()
    {
        HttpResponseMessage response = await AsAdmin().GetAsync($"{RouteName}/report?from=2026-10-04&to=2026-09-28");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RecruitingCannotSeeTheReport()
    {
        HttpResponseMessage response = await ClientAs(CovenantConstants.Role.Recruiting, Data.PunchUser.Id).GetAsync(ReportUri);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminFixesAMissingClockOutAndTheLunch()
    {
        var model = new UpdateUserAttendanceModel
        {
            ClockIn = new DateTime(2026, 9, 29, 9, 0, 0),
            ClockOut = new DateTime(2026, 9, 29, 17, 0, 0),
            LunchMinutes = 30,
            Reason = "Forgot to clock out, short lunch."
        };

        HttpResponseMessage response = await AsAdmin().PutAsJsonAsync($"{RouteName}/{Data.ToEdit.Id}", model);
        response.EnsureSuccessStatusCode();

        var report = await AsAdmin().GetFromJsonAsync<UserAttendanceReportModel>($"{ReportUri}&userId={Data.EditUser.Id}");
        UserAttendanceListModel edited = report.Items.Single();
        Assert.True(edited.IsEdited);
        Assert.False(edited.IsMissingClockOut);
        Assert.Equal(30, edited.LunchMinutes);
        Assert.Equal(0.5m, edited.LunchHours);
        Assert.Equal(7.5m, edited.WorkedHours);
        Assert.Equal(model.Reason, edited.EditReason);
    }

    [Fact]
    public async Task EditRejectsALunchLongerThanTheShift()
    {
        var model = new UpdateUserAttendanceModel
        {
            ClockIn = new DateTime(2026, 9, 29, 9, 0, 0),
            ClockOut = new DateTime(2026, 9, 29, 10, 0, 0),
            LunchMinutes = 90,
            Reason = "Too long."
        };

        HttpResponseMessage response = await AsAdmin().PutAsJsonAsync($"{RouteName}/{Data.ToEdit.Id}", model);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AdminCannotEditAnotherAgencyAttendance()
    {
        var model = new UpdateUserAttendanceModel
        {
            ClockIn = new DateTime(2026, 9, 30, 9, 0, 0),
            ClockOut = new DateTime(2026, 9, 30, 17, 0, 0),
            Reason = "Not mine."
        };

        HttpResponseMessage response = await AsAdmin().PutAsJsonAsync($"{RouteName}/{Data.Foreign.Id}", model);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task EditRejectsAClockOutBeforeTheClockIn()
    {
        var model = new UpdateUserAttendanceModel
        {
            ClockIn = new DateTime(2026, 9, 29, 17, 0, 0),
            ClockOut = new DateTime(2026, 9, 29, 9, 0, 0),
            Reason = "Wrong order."
        };

        HttpResponseMessage response = await AsAdmin().PutAsJsonAsync($"{RouteName}/{Data.ToEdit.Id}", model);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder().AddTestAuth(o => o.AddName("admin@attendance.com"));
            services.AddTestDatabase();
            services.AddSingleton<TimeProvider>(Data.Clock);
        }

        public void Configure(IApplicationBuilder app, CovenantContext context)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
            Data.Seed(context);
        }
    }

    private static class Data
    {
        public static readonly FakeTimeProvider Clock = new(new DateTimeOffset(2026, 10, 5, 8, 0, 0, TimeSpan.FromHours(-5)));

        public static void SetNow(DateTime bogotaNow) => Clock.SetUtcNow(new DateTimeOffset(bogotaNow, TimeSpan.FromHours(-5)));

        public static readonly Covenant.Common.Entities.Agency.Agency Agency = FakeData.FakeAgency();
        public static readonly Covenant.Common.Entities.Agency.Agency OtherAgency = FakeData.FakeAgency();

        public static readonly User AdminUser = new(CvnEmail.Create("admin@attendance.com").Value);
        public static readonly User PunchUser = new(CvnEmail.Create("punch@attendance.com").Value);
        public static readonly User ReportUser = new(CvnEmail.Create("report@attendance.com").Value);
        public static readonly User EditUser = new(CvnEmail.Create("edit@attendance.com").Value);
        public static readonly User ForeignUser = new(CvnEmail.Create("foreign@attendance.com").Value);

        public static readonly UserAttendance ToEdit = UserAttendance.Start(EditUser.Id, new DateTime(2026, 9, 29, 9, 0, 0));
        public static readonly UserAttendance Foreign = UserAttendance.Start(ForeignUser.Id, new DateTime(2026, 9, 30, 8, 0, 0));

        private static UserAttendance Closed(User user, DateTime clockIn, DateTime clockOut)
        {
            var attendance = UserAttendance.Start(user.Id, clockIn);
            attendance.Stop(clockOut);
            return attendance;
        }

        public static void Seed(CovenantContext context)
        {
            context.Agencies.AddRange(Agency, OtherAgency);
            context.Users.AddRange(AdminUser, PunchUser, ReportUser, EditUser, ForeignUser);
            context.AgencyPersonnel.AddRange(
                AgencyPersonnel.CreatePrimary(Agency.Id, AdminUser.Id, "Admin"),
                AgencyPersonnel.CreatePrimary(Agency.Id, PunchUser.Id, "Punch User"),
                AgencyPersonnel.CreatePrimary(Agency.Id, ReportUser.Id, "Report User"),
                AgencyPersonnel.CreatePrimary(Agency.Id, EditUser.Id, "Edit User"),
                AgencyPersonnel.CreatePrimary(OtherAgency.Id, ForeignUser.Id, "Foreign User"));
            context.UserAttendances.AddRange(
                Closed(ReportUser, new DateTime(2026, 9, 28, 8, 0, 0), new DateTime(2026, 9, 28, 18, 30, 0)),
                UserAttendance.Start(ReportUser.Id, new DateTime(2026, 10, 1, 9, 0, 0)),
                Closed(ReportUser, new DateTime(2026, 10, 3, 10, 0, 0), new DateTime(2026, 10, 3, 13, 0, 0)),
                ToEdit,
                Foreign);
            context.SaveChanges();
        }
    }
}
