using Covenant.Api.Authorization;
using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Request;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Company;
using Covenant.Infrastructure.Repositories.Request;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.CompanyModule.CompanyRequestWorkerTimeSheet
{
    public class CompanyRequestWorkerTimeSheetControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<CompanyRequestWorkerTimeSheetControllerTest.Startup>>
    {
        private readonly HttpClient _client;
        private readonly string _requestUri = $"api/v2/CompanyRequest/{Data.Request.Id}/Worker/{Data.Worker.Id}/TimeSheet";

        public CompanyRequestWorkerTimeSheetControllerTest(CustomWebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

        [Fact, TestOrder(1)]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync(_requestUri);
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<IList<TimeSheetListModel>>();
            Assert.NotEmpty(list);
            var model = list[0];
            Assert.Equal(model.Id, Data.TimeSheet.Id);
            Assert.Equal(model.Day, Data.TimeSheet.Date);
            Assert.Equal(model.TimeOut, Data.TimeSheet.TimeOut);
            Assert.True(model.WasApproved, nameof(model.WasApproved));
            Assert.NotNull(model.Comment);
            Assert.Equal(model.TimeIn, Data.TimeSheet.TimeIn);
            Assert.True(model.CanUpdate);
            Assert.Equal(Data.TimeSheet.TimeInApproved, model.TimeInApproved);
            Assert.Equal(Data.TimeSheet.TimeOutApproved, model.TimeOutApproved);
        }

        [Fact, TestOrder(2)]
        public async Task Put()
        {
            var model = new TimeSheetModel
            {
                TimeIn = Data.TimeSheet.TimeIn,
                Hours = TimeSpan.FromHours(8)
            };
            HttpResponseMessage response = await _client.PutAsJsonAsync($"{_requestUri}", model);
            response.EnsureSuccessStatusCode();
            response = await _client.GetAsync(_requestUri);
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<IEnumerable<TimeSheetListModel>>();
            foreach (TimeSheetListModel item in list)
            {
                Assert.NotNull(item.TimeInApproved);
                Assert.NotNull(item.TimeOutApproved);
                Assert.NotNull(item.Comment);
                Assert.True(item.WasApproved);
                Assert.True(item.CanUpdate);
            }
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
                services.AddSingleton<CompanyIdFilter>();
                services.AddSingleton<ICompanyRepository, CompanyRepository>();
                services.AddSingleton(Rates.DefaultRates);
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
            private static readonly DateTime FakeNow = new DateTime(2019, 01, 01);
            public static readonly Guid CompanyId = Guid.NewGuid();
            public static readonly Request Request = Request.AgencyCreateRequest(Guid.NewGuid(), CompanyId, FakeData.FakeLocation(), FakeNow, Guid.NewGuid()).Value;
            public static readonly User Worker = new User(CvnEmail.Create("w_worker@mail.com").Value);

            private static readonly Covenant.Common.Entities.Request.WorkerRequest FakeWorkerRequest =
                Covenant.Common.Entities.Request.WorkerRequest.AgencyBook(Worker.Id, Request.Id);

            public static readonly TimeSheet TimeSheet = TimeSheet.CreateTimeSheet(FakeWorkerRequest, FakeNow, TimeSpan.FromHours(8), now: FakeNow).Value;

            public static void Seed(CovenantContext context)
            {
                context.AddRange(Worker, Request, FakeWorkerRequest, TimeSheet);
                context.SaveChanges();
            }
        }
    }
}