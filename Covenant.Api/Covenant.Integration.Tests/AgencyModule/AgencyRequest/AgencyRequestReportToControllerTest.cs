using Covenant.Api.AgencyModule.AgencyRequestReportTo.Controllers;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Enums;
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
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.AgencyRequest
{
    public class AgencyRequestReportToControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<AgencyRequestReportToControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public AgencyRequestReportToControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => AgencyRequestReportToController.RouteName.Replace("{requestId}", Startup.FakeRequest.Id.ToString());

        [Fact]
        public async Task Post()
        {
            Guid id = Startup.FakeContactPersonPost.Id;
            var requestUri = $"{RequestUri()}/{id}";
            HttpResponseMessage response = await _client.PostAsJsonAsync(requestUri, new { });
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            RequestReportTo entity = await context.RequestReportTo.SingleAsync(c => c.RequestId == Startup.FakeRequest.Id && c.ContactPersonId == id);
            Assert.NotNull(entity);
        }

        [Fact]
        public async Task Get()
        {
            CompanyProfileContactPerson entity = Startup.FakeContactPerson;
            var requestUri = $"{RequestUri()}";
            HttpResponseMessage response = await _client.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<PaginatedList<RequestContactPersonModel>>();
            var model = list.Items.Single(c => c.Id == entity.Id);
            Assert.Equal(entity.Id, model.Id);
            Assert.Equal(entity.Title, model.Title);
            Assert.Equal(entity.FirstName, model.FirstName);
            Assert.Equal(entity.MiddleName, model.MiddleName);
            Assert.Equal(entity.LastName, model.LastName);
        }

        [Fact]
        public async Task GetById()
        {
            CompanyProfileContactPerson entity = Startup.FakeContactPerson;
            var requestUri = $"{RequestUri()}/{entity.Id}";
            HttpResponseMessage response = await _client.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadAsJsonAsync<RequestContactPersonDetailModel>();
            Assert.Equal(entity.Id, model.Id);
            Assert.Equal(entity.Title, model.Title);
            Assert.Equal(entity.FirstName, model.FirstName);
            Assert.Equal(entity.MiddleName, model.MiddleName);
            Assert.Equal(entity.LastName, model.LastName);
            Assert.Equal(entity.Position, model.Position);
            Assert.Equal(entity.Email, model.Email);
            Assert.Equal(entity.MobileNumber, model.MobileNumber);
            Assert.Equal(entity.OfficeNumber, model.OfficeNumber);
            Assert.Equal(entity.OfficeNumberExt, model.OfficeNumberExt);
        }

        [Fact]
        public async Task Delete()
        {
            CompanyProfileContactPerson entity = Startup.FakeContactPersonDelete;
            var requestUri = $"{RequestUri()}/{entity.Id}";
            HttpResponseMessage response = await _client.DeleteAsync(requestUri);
            response.EnsureSuccessStatusCode();
            Assert.False(_factory.Server.Host.Services.GetRequiredService<CovenantContext>()
                .RequestReportTo.Any(c => c.RequestId == Startup.FakeRequest.Id && c.ContactPersonId == entity.Id));
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o =>
                    {
                        o.AddSub(AgencyId);
                        o.AddAgencyPersonnelRole();
                    });
                services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<IRequestRepository, RequestRepository>();
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<AgencyIdFilter>();
            }

            public static readonly Guid AgencyId = Guid.NewGuid();
            private static readonly CompanyName FakeCompanyName = CompanyName.Create("ABC").Value;
            public static readonly CompanyProfile FakeCompany = CompanyProfile.AgencyCreateCompany(
                new User(CvnEmail.Create("CompanyProfile@mail.com").Value),
                AgencyId,
                FakeCompanyName,
                FakeCompanyName,
                "647897654",
                default,
                default,
                default,
                default,
                CompanyProfileIndustry.Create("Labour").Value,
                default,
                default,
                default,
                default,
                "juan@covenantgroupl.com",
                CompanyStatus.Lead,
                null).Value;
            private static readonly Guid JobPositionRateId = Guid.NewGuid();
            public static readonly Request FakeRequest = FakeData.FakeRequest(AgencyId, FakeCompany.Company.Id, JobPositionRateId);
            public static readonly CompanyProfileContactPerson FakeContactPersonPost = new CompanyProfileContactPerson(FakeCompany.Id) { Title = "Sr.", FirstName = "Post" };
            public static readonly CompanyProfileContactPerson FakeContactPersonDelete = new CompanyProfileContactPerson(FakeCompany.Id) { Title = "Sr.", FirstName = "Delete" };
            public static readonly CompanyProfileContactPerson FakeContactPerson = new CompanyProfileContactPerson(FakeCompany.Id)
            {
                Title = "Sr.",
                FirstName = "Jhon",
                MiddleName = "Arvy",
                LastName = "Mun",
                Email = "con@mail.com",
                MobileNumber = "647897654",
                OfficeNumber = "89765432",
                OfficeNumberExt = 777,
                Position = "Supervisor"
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
                FakeCompany.ContactPersons.Add(FakeContactPersonPost);
                FakeCompany.ContactPersons.Add(FakeContactPerson);
                FakeCompany.ContactPersons.Add(FakeContactPersonDelete);
                context.RequestReportTo.Add(new RequestReportTo(FakeRequest.Id, FakeContactPerson.Id));
                context.RequestReportTo.Add(new RequestReportTo(FakeRequest.Id, FakeContactPersonDelete.Id));
                context.AddRange(FakeCompany, FakeRequest);
                context.SaveChanges();
            }
        }
    }
}