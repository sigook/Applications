using Covenant.Api.AgencyModule.AgencyRequestWorkerTimeSheet.Models;
using Covenant.Api.Authorization;
using Covenant.Api.CompanyModule.CompanyRequestWorkerTimeSheet.Controllers;
using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Request;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories;
using Covenant.Infrastructure.Repositories.Company;
using Covenant.Infrastructure.Repositories.Request;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Covenant.Integration.Tests.CompanyModule.CompanyRequestWorkerTimeSheet
{
    public class V2CompanyRequestWorkerTimeSheetControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<V2CompanyRequestWorkerTimeSheetControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        private static string _requestUri() =>
            V2CompanyRequestWorkerTimeSheetController.RouteName
                .Replace("{requestId}", Data.Request.Id.ToString())
                .Replace("{workerId}", Data.Worker.Id.ToString());

        public V2CompanyRequestWorkerTimeSheetControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync(_requestUri());
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<IEnumerable<TimeSheetListModel>>();
            Assert.NotEmpty(list);
            foreach (TimeSheet entity in Data.TimeSheets)
            {
                TimeSheetListModel model = list.Single(s => s.Id == entity.Id);
                AssertModelEntity(model, entity);
            }
        }

        private static void AssertModelEntity(TimeSheetListModel model, TimeSheet entity)
        {
            Assert.Equal(model.Id, entity.Id);
            Assert.Equal(model.Day, entity.Date);
            Assert.Equal(model.ClockIn, entity.ClockIn);
            Assert.Equal(model.ClockOut, entity.ClockOut);
            Assert.Equal(model.ClockInRounded, entity.ClockInRounded);
            Assert.Equal(model.ClockOutRounded, entity.ClockOutRounded);
            Assert.Equal(model.TimeIn, entity.TimeIn);
            Assert.Equal(model.TimeOut, entity.TimeOut);
            Assert.Equal(model.TimeInApproved, entity.TimeInApproved);
            Assert.Equal(model.TimeOutApproved, entity.TimeOutApproved);
            Assert.Equal(model.Comment, entity.Comment);
            Assert.Equal(model.MissingHours, entity.MissingHours);
            Assert.Equal(model.MissingHoursOvertime, entity.MissingHoursOvertime);
            Assert.Equal(model.MissingRateWorker, entity.MissingRateWorker);
            Assert.Equal(model.MissingRateAgency, entity.MissingRateAgency);
            Assert.Equal(model.DeductionsOthers, entity.DeductionsOthers);
            Assert.Equal(model.DeductionsOthersDescription, entity.DeductionsOthersDescription);
            Assert.Equal(model.BonusOrOthersDescription, entity.BonusOrOthersDescription);
            Assert.True(model.TimeOutApproved is null || model.WasApproved, nameof(model.WasApproved));
            Assert.True(model.CanUpdate);
        }

        [Fact]
        public async Task Post()
        {
            var model = new TimeSheetModel
            {
                TimeIn = Data.FakeNow.AddDays(5),
                Hours = TimeSpan.FromHours(8)
            };
            HttpResponseMessage response = await _client.PostAsJsonAsync(_requestUri(), model);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<TimeSheetListModel>();
            var context = _factory.Server.Host.Services.GetService<CovenantContext>();
            TimeSheet entity = await context.TimeSheet.SingleAsync(s => s.Id == detail.Id);
            Assert.Equal(model.TimeIn.Date, entity.Date);
            Assert.Equal(model.TimeIn.Date, entity.TimeIn);
            Assert.Equal(model.TimeIn.Date.AddHours(model.Hours.TotalHours), entity.TimeOut);
            Assert.NotNull(entity.Comment);
        }

        [Fact]
        public async Task Put()
        {
            TimeSheet timeSheet = Data.FakeTimeSheet;
            var model = new TimeSheetModel
            {
                TimeIn = timeSheet.TimeIn,
                Hours = TimeSpan.FromHours(2),
                MissingHours = TimeSpan.FromHours(1),
                MissingHoursOvertime = TimeSpan.FromHours(3),
                DeductionsOthers = 100,
                BonusOrOthers = 200,
                DeductionsOthersDescription = "Uniform",
                BonusOrOthersDescription = "Holiday",
            };
            Guid id = timeSheet.Id;
            HttpResponseMessage response = await _client.PutAsJsonAsync($"{_requestUri()}/{id}", model);
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetService<CovenantContext>();
            TimeSheet entity = await context.TimeSheet.SingleAsync(s => s.Id == id);
            Assert.Equal(model.TimeIn, entity.Date);
            Assert.Equal(model.TimeIn, entity.TimeIn);
            Assert.Equal(model.Hours.TotalHours, entity.TotalHoursApproved);
            Assert.Equal(model.MissingHours, entity.MissingHours);
            Assert.Equal(model.MissingHoursOvertime, entity.MissingHoursOvertime);
            Assert.Equal(default, entity.MissingRateAgency);
            Assert.Equal(default, entity.MissingRateWorker);
            Assert.Equal(model.DeductionsOthers, entity.DeductionsOthers);
            Assert.Equal(model.BonusOrOthers, entity.BonusOrOthers);
            Assert.Equal(model.DeductionsOthersDescription, entity.DeductionsOthersDescription);
            Assert.Equal(model.BonusOrOthersDescription, entity.BonusOrOthersDescription);
            Assert.NotEmpty(entity.Comment);
        }

        [Fact]
        public async Task ClockIn()
        {
            var model = new CompanyClockInModel
            {
                ClockIn = Data.FakeNow.TimeOfDay.Subtract(TimeSpan.FromHours(1))
            };
            HttpResponseMessage response = await _client.PostAsJsonAsync($"{_requestUri()}/ClockIn", model);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<RegisterTimeSheetResultModel>();
            var context = _factory.Server.Host.Services.GetService<CovenantContext>();
            TimeSheet entity = await context.TimeSheet.SingleAsync(s => s.Id == detail.TimeSheetId);
            Assert.Equal(model.ClockIn, entity.ClockIn.GetValueOrDefault().TimeOfDay);
        }

        [Fact]
        public async Task Delete()
        {
            var context = _factory.Server.Host.Services.GetService<CovenantContext>();
            TimeSheet timeSheet = Data.TimeSheetForDelete;
            Assert.True(await context.TimeSheet.AnyAsync(s => s.Id == timeSheet.Id));
            HttpResponseMessage response = await _client.DeleteAsync($"{_requestUri()}/{timeSheet.Id}");
            response.EnsureSuccessStatusCode();
            Assert.False(await context.TimeSheet.AnyAsync(s => s.Id == timeSheet.Id));
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o =>
                    {
                        o.AddSub(Data.CompanyId);
                        o.AddCompanyRole();
                    });
                services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<ITimeSheetRepository, TimeSheetRepository>();
                services.AddSingleton<ITimesheetService, TimesheetService>();
                services.AddSingleton<IRequestRepository, RequestRepository>();
                var timeService = new Mock<ITimeService>();
                timeService.Setup(t => t.GetCurrentDateTime()).Returns(Data.FakeNow);
                services.AddSingleton(timeService.Object);
                services.AddSingleton<ICatalogRepository, CatalogRepository>();
                services.AddSingleton<IWorkerRequestRepository, WorkerRequestRepository>();
                services.AddSingleton(TimeLimits.DefaultTimeLimits);
                services.AddSingleton(Rates.DefaultRates);
                services.AddSingleton<CompanyIdFilter>();
                services.AddSingleton<ICompanyRepository, CompanyRepository>();
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
                Data.Seed(context);
            }
        }

        private static class Data
        {
            public static readonly DateTime FakeNow = new DateTime(2019, 01, 01, 08, 00, 00);
            public static readonly Guid CompanyId = Guid.NewGuid();
            public static readonly Request Request = Request.AgencyCreateRequest(Guid.NewGuid(), CompanyId, FakeData.FakeLocation(), FakeNow, Guid.NewGuid()).Value;
            public static readonly User Worker = new User(CvnEmail.Create("w_worker@mail.com").Value);

            private static readonly Covenant.Common.Entities.Request.WorkerRequest FakeWorkerRequest =
                Covenant.Common.Entities.Request.WorkerRequest.AgencyBook(Worker.Id, Request.Id);

            public static readonly TimeSheet FakeTimeSheet = TimeSheet.CreateTimeSheet(FakeWorkerRequest, FakeNow.AddDays(1), TimeSpan.FromHours(8), now: FakeNow).Value;
            public static readonly TimeSheet TimeSheetForDelete = TimeSheet.CreateTimeSheet(FakeWorkerRequest, FakeNow.AddDays(2), TimeSpan.FromHours(8), now: FakeNow).Value;
            public static readonly TimeSheet TimeSheetReportedByWorker = TimeSheet.WorkerClockIn(FakeWorkerRequest.Id, FakeNow.AddDays(3), false, FakeNow.AddDays(3)).Value;

            public static readonly TimeSheet[] TimeSheets = { FakeTimeSheet, TimeSheetReportedByWorker };

            public static void Seed(CovenantContext context)
            {
                context.AddRange(Worker, Request, FakeWorkerRequest, FakeTimeSheet, TimeSheetForDelete, TimeSheetReportedByWorker);
                context.SaveChanges();
            }
        }
    }
}