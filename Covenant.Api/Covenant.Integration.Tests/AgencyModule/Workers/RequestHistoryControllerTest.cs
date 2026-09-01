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
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Request;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static Covenant.Api.Controllers.Sigook.Agency.Workers.RequestHistoryController;
using System.Net.Http.Json;

namespace Covenant.Integration.Tests.AgencyModule.Workers
{
    public class RequestHistoryControllerTest : IClassFixture<CustomWebApplicationFactory<RequestHistoryControllerTest.Startup>>
    {
        private readonly HttpClient _client;

        public RequestHistoryControllerTest(CustomWebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

        [Fact]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync(
                RouteName.Replace("{workerProfileId}", Startup.FakeWorkerProfile.Id.ToString()));
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadFromJsonAsync<PaginatedList<RequestListModel>>();
            Assert.NotEmpty(list.Items);
        }
        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddTestDatabase();
                services.AddSingleton<IRequestRepository, RequestRepository>();
                services.AddSingleton<ITimeService, TimeService>();
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o =>
                    o.AddAgencyPersonnelRole());
                services.AddSingleton<AgencyIdFilter>();
            }

            public static readonly Covenant.Common.Entities.Agency.Agency FakeAgency = new Covenant.Common.Entities.Agency.Agency() { User = FakeData.FakeUser() };
            public static readonly WorkerProfile FakeWorkerProfile = new WorkerProfile(new User(CvnEmail.Create("w@mail").Value))
            {
                Agency = FakeAgency
            , Location = FakeData.FakeLocation(),};

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
                    FakeAgency, "A", "6479807865", new CompanyProfileIndustry());
                var request = new Request(cp, FakeData.FakeJobPositionRate(cp)) 
                { 
                    JobLocation = new Location { City = new City { Province = new Province { Country = FakeData.FakeCountry("CA") } } } 
                };
                context.Agencies.Add(FakeAgency);
                context.CompanyProfiles.Add(cp);
                context.WorkerProfiles.Add(FakeWorkerProfile);
                context.Requests.Add(request);
                context.WorkerRequests.Add(Covenant.Common.Entities.Request.WorkerRequest.AgencyBook(FakeWorkerProfile.Id, request.Id));
                context.SaveChanges();
            }
        }
    }
}
