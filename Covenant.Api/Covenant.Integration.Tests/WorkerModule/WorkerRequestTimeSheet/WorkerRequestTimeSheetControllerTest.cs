using Covenant.Api.WorkerModule.WorkerRequestTimeSheet.Controllers;
using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories;
using Covenant.Infrastructure.Repositories.Request;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Covenant.Integration.Tests.WorkerModule.WorkerRequestTimeSheet
{
    public class WorkerRequestTimeSheetControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<WorkerRequestTimeSheetControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        private readonly string _url = WorkerRequestTimeSheetController.RouteName.Replace("{requestId}", Data.Request.Id.ToString());

        public WorkerRequestTimeSheetControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Post_ClockIn()
        {
            var payload = new WorkerLocationModel
            {
                Latitude = 43.60210640960763,
                Longitude = -79.73347051568783
            };
            HttpResponseMessage response = await _client.PostAsJsonAsync(_url, payload);
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadAsJsonAsync<RegisterTimeSheetResultModel>();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            Assert.NotNull(context.TimeSheet.Single(s => s.Id == model.TimeSheetId).ClockIn);
        }

        [Fact]
        public async Task Get_After_ClockIn()
        {
            HttpResponseMessage response = await _client.GetAsync(_url);
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<PaginatedList<TimeSheetListModel>>();
            TimeSheetListModel model = list.Items.Single(s => s.Id == Data.Day1.Id);

            Assert.Equal(model.Id, Data.Day1.Id);
            Assert.Equal(model.Day, Data.Day1.Date);
            Assert.Equal(model.TimeIn, Data.Day1.TimeIn);
            Assert.Equal(model.TimeOut, Data.Day1.TimeOut);
            Assert.Equal(model.ClockIn, Data.Day1.ClockIn);
            Assert.Equal(model.ClockOut, Data.Day1.ClockOut);
            Assert.Equal(model.ClockInRounded, Data.Day1.ClockInRounded);
            Assert.Equal(model.ClockOutRounded, Data.Day1.ClockOutRounded);
            Assert.Equal(model.TimeInApproved, Data.Day1.TimeInApproved);
            Assert.Equal(model.TimeOutApproved, Data.Day1.TimeOutApproved);
            Assert.Equal(model.Comment, Data.Day1.Comment);
            Assert.False(model.WasApproved);
            Assert.Equal(0, model.TotalHours);
        }

        [Fact]
        public async Task Get_After_ClockOut()
        {
            HttpResponseMessage response = await _client.GetAsync(_url);
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<PaginatedList<TimeSheetListModel>>();
            TimeSheetListModel model = list.Items.Single(s => s.Id == Data.Day2.Id);

            Assert.Equal(model.Id, Data.Day2.Id);
            Assert.Equal(model.Day, Data.Day2.Date);
            Assert.Equal(model.TimeIn, Data.Day2.TimeIn);
            Assert.Equal(model.TimeOut, Data.Day2.TimeOut);
            Assert.Equal(model.ClockIn, Data.Day2.ClockIn);
            Assert.Equal(model.ClockOut, Data.Day2.ClockOut);
            Assert.Equal(model.ClockInRounded, Data.Day2.ClockInRounded);
            Assert.Equal(model.ClockOutRounded, Data.Day2.ClockOutRounded);
            Assert.Equal(model.TimeInApproved, Data.Day2.TimeInApproved);
            Assert.Equal(model.TimeOutApproved, Data.Day2.TimeOutApproved);
            Assert.Equal(model.Comment, Data.Day2.Comment);
            Assert.False(model.WasApproved);
            Assert.Equal(Data.HoursDay2, model.TotalHours);
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o =>
                    {
                        o.AddSub(Data.FakeUser.Id);
                        o.AddWorkerRole();
                    });
                services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<IRequestRepository, RequestRepository>();
                services.AddSingleton<IWorkerRequestRepository, WorkerRequestRepository>();
                services.AddSingleton<ICatalogRepository, CatalogRepository>();
                services.AddSingleton<ITimeSheetRepository, TimeSheetRepository>();
                services.AddSingleton<ITimesheetService, TimesheetService>();
                services.AddSingleton(TimeLimits.DefaultTimeLimits);
                services.AddSingleton(Rates.DefaultRates);
                var timeService = new Mock<ITimeService>();
                timeService.Setup(s => s.GetCurrentDateTime()).Returns(() => Data.Now);
                timeService.Setup(s => s.GetCurrentDateTimeOffset()).Returns(() => new DateTimeOffset(Data.Now));
                services.AddSingleton(timeService.Object);
                services.AddSingleton<IIdentityServerService, IdentityServerService>();
            }

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
                context.User.Add(Data.FakeUser);
                context.WorkerProfile.Add(Data.FakeWorker);
                context.Location.Add(Data.FakeLocation);
                context.Request.Add(Data.Request);
                context.WorkerRequest.Add(Data.FakeWorkerRequest);
                context.SaveChanges();
            }
        }

        private static class Data
        {
            public static DateTime Now = DateTime.Now;
            private static readonly Guid AgencyId = Guid.NewGuid();
            public static readonly User FakeUser = new User(CvnEmail.Create("user_user@mail.com").Value);
            internal static readonly WorkerProfile FakeWorker = new WorkerProfile(FakeUser, AgencyId);
            public static Location FakeLocation = FakeData.FakeLocation();
            public static readonly Request Request = Request.AgencyCreateRequest(AgencyId, Guid.NewGuid(), FakeLocation, Now, Guid.NewGuid()).Value;
            internal static readonly double HoursDay2 = 2;

            public static readonly Covenant.Common.Entities.Request.WorkerRequest FakeWorkerRequest = Covenant.Common.Entities.Request.WorkerRequest.AgencyBook(FakeUser.Id, Request.Id);
            public static readonly TimeSheet Day1 = TimeSheet.WorkerClockIn(FakeWorkerRequest.Id, Now.AddDays(-2).Date).Value;
            public static readonly TimeSheet Day2 = TimeSheet.WorkerClockIn(FakeWorkerRequest.Id, Now.AddDays(-1).Date).Value;

            static Data()
            {
                FakeWorkerRequest.UpdateStartWorking(Now.AddDays(-2));
                Day2.AddClockOut(Day2.ClockIn.GetValueOrDefault().AddHours(HoursDay2));
                FakeWorkerRequest.AddTimeSheet(Day1);
                FakeWorkerRequest.AddTimeSheet(Day2);
            }
        }
    }
}