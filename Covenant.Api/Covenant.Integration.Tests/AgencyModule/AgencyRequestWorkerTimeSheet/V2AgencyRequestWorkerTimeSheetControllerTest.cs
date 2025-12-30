using Covenant.Api.AgencyModule.AgencyRequestWorkerTimeSheet.Controllers;
using Covenant.Api.Authorization;
using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Request;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Request.TimeSheet;
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

namespace Covenant.Integration.Tests.AgencyModule.AgencyRequestWorkerTimeSheet
{
    public class V2AgencyRequestWorkerTimeSheetControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<V2AgencyRequestWorkerTimeSheetControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public V2AgencyRequestWorkerTimeSheetControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => V2AgencyRequestWorkerTimeSheetController.RouteName.Replace("{requestId}",
            Data.Request.Id.ToString()).Replace("{workerId}", Data.Worker.Id.ToString());

        [Fact]
        public async Task Post()
        {
            var model = new TimeSheetModel
            {
                TimeIn = DateTime.Now.Date,
                Hours = TimeSpan.FromHours(12),
                MissingHours = TimeSpan.FromHours(2),
                MissingHoursOvertime = TimeSpan.FromHours(1),
                MissingRateWorker = 15,
                MissingRateAgency = 18,
                DeductionsOthers = 5.4m,
                BonusOrOthers = 6.5m,
                DeductionsOthersDescription = "Insurance Deduction",
                BonusOrOthersDescription = "Bonus"
            };
            HttpResponseMessage response = await _client.PostAsJsonAsync(RequestUri(), model);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<TimeSheetListModel>();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            TimeSheet entity = await context.TimeSheet.SingleAsync(c => c.Id == detail.Id);
            var totalHours = model.Hours;
            Assert.Equal(detail.Id, entity.Id);
            Assert.Equal(model.TimeIn, entity.TimeIn);
            Assert.Equal(totalHours.TotalHours, entity.TotalHours);
            Assert.Equal(model.MissingHours, entity.MissingHours);
            Assert.Equal(model.MissingHoursOvertime, entity.MissingHoursOvertime);
            Assert.Equal(model.MissingRateWorker, entity.MissingRateWorker);
            Assert.Equal(model.MissingRateAgency, entity.MissingRateAgency);
            Assert.Equal(model.DeductionsOthers, entity.DeductionsOthers);
            Assert.Equal(model.BonusOrOthers, entity.BonusOrOthers);
            Assert.Equal(model.DeductionsOthersDescription, entity.DeductionsOthersDescription);
            Assert.Equal(model.BonusOrOthersDescription, entity.BonusOrOthersDescription);
        }

        [Fact]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync(RequestUri());
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

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o =>
                    {
                        o.AddAgencyPersonnelRole(Data.AgencyId);
                    });
                services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);

                services.AddSingleton<ITimesheetService, TimesheetService>();
                services.AddSingleton<ITimeSheetRepository, TimeSheetRepository>();
                services.AddSingleton<IRequestRepository, RequestRepository>();
                services.AddSingleton<ICatalogRepository, CatalogRepository>();
                services.AddSingleton<IWorkerRequestRepository, WorkerRequestRepository>();
                services.AddSingleton<IIdentityServerService, IdentityServerService>();
                services.AddSingleton(TimeLimits.DefaultTimeLimits);
                services.AddSingleton(Rates.DefaultRates);
                var timeService = new Mock<ITimeService>();
                timeService.Setup(s => s.GetCurrentDateTime()).Returns(new DateTime(2019, 01, 01));
                services.AddSingleton(timeService.Object);
                services.AddSingleton<AgencyIdFilter>();
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
            public static readonly Guid AgencyId = Guid.NewGuid();
            private static readonly DateTime FakeNow = new DateTime(2019, 01, 01);
            public static readonly Request Request = Request.AgencyCreateRequest(AgencyId, Guid.NewGuid(), FakeData.FakeLocation(), FakeNow, Guid.NewGuid()).Value;
            public static readonly User Worker = new User(CvnEmail.Create("w_worker@mail.com").Value);
            private static readonly Covenant.Common.Entities.Request.WorkerRequest FakeWorkerRequest = Covenant.Common.Entities.Request.WorkerRequest.AgencyBook(Worker.Id, Request.Id);
            public static readonly TimeSheet FakeTimeSheet = TimeSheet.CreateTimeSheet(FakeWorkerRequest, FakeNow, TimeSpan.FromHours(8), now: FakeNow).Value;
            public static readonly TimeSheet TimeSheetReportedByWorker = TimeSheet.WorkerClockIn(FakeWorkerRequest.Id,
                FakeNow.AddDays(1), false, FakeNow.AddDays(1)).Value;
            public static readonly TimeSheet[] TimeSheets = { FakeTimeSheet, TimeSheetReportedByWorker };

            public static void Seed(CovenantContext context)
            {
                TimeSheetReportedByWorker.AddClockOut(TimeSheetReportedByWorker.ClockIn.GetValueOrDefault().AddHours(1), TimeSheetReportedByWorker.ClockIn.GetValueOrDefault().AddHours(1));
                context.User.Add(Worker);
                context.Request.Add(Request);
                context.WorkerRequest.Add(FakeWorkerRequest);
                context.AddRange(FakeTimeSheet, TimeSheetReportedByWorker);
                context.SaveChanges();
            }
        }
    }
}