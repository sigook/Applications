using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Request;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static Covenant.Api.AgencyModule.AgencyWorkerProfileRequestHistory.Controllers.AgencyWorkerProfileRequestHistoryController;

namespace Covenant.Integration.Tests.AgencyModule.AgencyWorkerProfileRequestHistory
{
    public class AgencyWorkerProfileRequestHistoryControllerTest : IClassFixture<CustomWebApplicationFactory<AgencyWorkerProfileRequestHistoryControllerTest.Startup>>
    {
        private readonly HttpClient _client;

        public AgencyWorkerProfileRequestHistoryControllerTest(CustomWebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

        [Fact]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync(
                RouteUrl.Replace("{workerProfileId}", Startup.FakeWorkerProfile.Id.ToString()));
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<PaginatedList<RequestListModel>>();
            Assert.NotEmpty(list.Items);
        }
        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDbContext<CovenantContext>(b
                    => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<IRequestRepository, RequestRepository>();
                services.AddSingleton<ITimeService, TimeService>();
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o =>
                    o.AddAgencyPersonnelRole());
                services.AddSingleton<AgencyIdFilter>();
            }

            public static readonly Covenant.Common.Entities.Agency.Agency FakeAgency = new Covenant.Common.Entities.Agency.Agency();
            public static readonly WorkerProfile FakeWorkerProfile = new WorkerProfile(new User(CvnEmail.Create("w@mail").Value))
            {
                Agency = FakeAgency
            };

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

                var cp = new CompanyProfile(new User(CvnEmail.Create("c@maol.com").Value),
                    FakeAgency, "A", "B", "6479807865", new CompanyProfileIndustry());
                var request = new Request(cp.Company, FakeAgency, new CompanyProfileJobPositionRate()) 
                { 
                    JobLocation = new Location { City = new City { Province = new Province { Country = new Country() } } } 
                };
                context.Agencies.Add(FakeAgency);
                context.CompanyProfile.Add(cp);
                context.WorkerProfile.Add(FakeWorkerProfile);
                context.Request.Add(request);
                context.WorkerRequest.Add(Covenant.Common.Entities.Request.WorkerRequest.AgencyBook(FakeWorkerProfile.WorkerId, request.Id));
                context.SaveChanges();
            }
        }
    }
}