using Covenant.Api.AgencyModule.AgencyCompanyProfileContactPerson.Controllers;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Company;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.AgencyCompanyProfileContactPerson
{
    public class AgencyCompanyProfileContactPersonControllerTest : IClassFixture<CustomWebApplicationFactory<AgencyCompanyProfileContactPersonControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public AgencyCompanyProfileContactPersonControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => AgencyCompanyProfileContactPersonController.RouteName.Replace("{profileId}",
            Startup.FakeCompanyProfile.Id.ToString());

        [Fact]
        public async Task Post()
        {
            var model = new CompanyProfileContactPersonModel
            {
                Title = "Sr",
                FirstName = "Pepe",
                MiddleName = "Ramon",
                LastName = "Martin",
                Email = "pepe@mail.com",
                MobileNumber = "645789063",
                OfficeNumber = "654367896",
                OfficeNumberExt = 123,
                Position = "Manager"
            };
            HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(_client, RequestUri(), model);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<CompanyProfileContactPersonModel>();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.Set<CompanyProfileContactPerson>().SingleAsync(c => c.Id == detail.Id);
            Assert.Equal(model.Title, entity.Title);
            Assert.Equal(model.FirstName, entity.FirstName);
            Assert.Equal(model.MiddleName, entity.MiddleName);
            Assert.Equal(model.LastName, entity.LastName);
            Assert.Equal(model.Email, entity.Email);
            Assert.Equal(model.MobileNumber, entity.MobileNumber);
            Assert.Equal(model.OfficeNumber, entity.OfficeNumber);
            Assert.Equal(model.OfficeNumberExt, entity.OfficeNumberExt);
            Assert.Equal(model.Position, entity.Position);
        }

        [Fact]
        public async Task Put()
        {
            var model = new CompanyProfileContactPersonModel
            {
                Title = "Ms",
                FirstName = "Mary",
                MiddleName = "MaryM",
                LastName = "MaryL",
                Email = "mary@mail.com",
                MobileNumber = "6480002233",
                OfficeNumber = "647998867",
                OfficeNumberExt = 8,
                Position = "Coordinator"
            };
            Guid id = Startup.FakeUpdateContactPerson.Id;
            HttpResponseMessage response = await HttpClientJsonExtensions.PutAsJsonAsync(_client, $"{RequestUri()}/{id}", model);
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            CompanyProfileContactPerson entity = await context.Set<CompanyProfileContactPerson>().SingleAsync(c => c.Id == id);
            Assert.Equal(model.Title, entity.Title);
            Assert.Equal(model.FirstName, entity.FirstName);
            Assert.Equal(model.MiddleName, entity.MiddleName);
            Assert.Equal(model.LastName, entity.LastName);
            Assert.Equal(model.Email, entity.Email);
            Assert.Equal(model.MobileNumber, entity.MobileNumber);
            Assert.Equal(model.OfficeNumber, entity.OfficeNumber);
            Assert.Equal(model.OfficeNumberExt, entity.OfficeNumberExt);
            Assert.Equal(model.Position, entity.Position);
        }

        [Fact]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync(RequestUri());
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<IEnumerable<CompanyProfileContactPersonModel>>();
            Assert.NotEmpty(list);
            CompanyProfileContactPerson entity = Startup.FakeContactPerson;
            var model = list.Single(c => c.Id == entity.Id);
            AssertDetailAndEntity(model, entity);
        }

        [Fact]
        public async Task GetById()
        {
            CompanyProfileContactPerson entity = Startup.FakeContactPerson;
            HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/{entity.Id}");
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadAsJsonAsync<CompanyProfileContactPersonModel>();
            AssertDetailAndEntity(model, entity);
        }

        [Fact]
        public async Task Delete()
        {
            Guid id = Startup.FakeDeleteContactPerson.Id;
            HttpResponseMessage response = await _client.DeleteAsync($"{RequestUri()}/{id}");
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            Assert.False(await context.Set<CompanyProfileContactPerson>().AnyAsync(c => c.Id == id));
        }

        private static void AssertDetailAndEntity(CompanyProfileContactPersonModel model, CompanyProfileContactPerson entity)
        {
            Assert.Equal(model.Title, entity.Title);
            Assert.Equal(model.FirstName, entity.FirstName);
            Assert.Equal(model.MiddleName, entity.MiddleName);
            Assert.Equal(model.LastName, entity.LastName);
            Assert.Equal(model.Email, entity.Email);
            Assert.Equal(model.MobileNumber, entity.MobileNumber);
            Assert.Equal(model.OfficeNumber, entity.OfficeNumber);
            Assert.Equal(model.OfficeNumberExt, entity.OfficeNumberExt);
            Assert.Equal(model.Position, entity.Position);
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o => o.AddAgencyPersonnelRole());
                services.AddDbContext<CovenantContext>(b
                    => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<ICompanyRepository, CompanyRepository>();
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<AgencyIdFilter>();
            }

            private static readonly Covenant.Common.Entities.Agency.Agency FakeAgency = new Covenant.Common.Entities.Agency.Agency();
            public static readonly CompanyProfile FakeCompanyProfile = new CompanyProfile(new User(CvnEmail.Create("c@mail.com").Value), FakeAgency,
                "", "", "", new CompanyProfileIndustry("Company Industry"));

            public static readonly CompanyProfileContactPerson FakeContactPerson = new CompanyProfileContactPerson(FakeCompanyProfile.Id)
            {
                Title = "Sr",
                FirstName = "Bob",
                MiddleName = "BobM",
                LastName = "BobL",
                Position = "Supervisor",
                Email = "bob@mail.com",
                MobileNumber = "456780943",
                OfficeNumber = "2345789654",
                OfficeNumberExt = 999
            };

            public static readonly CompanyProfileContactPerson FakeUpdateContactPerson = new CompanyProfileContactPerson(FakeCompanyProfile.Id);
            public static readonly CompanyProfileContactPerson FakeDeleteContactPerson = new CompanyProfileContactPerson(FakeCompanyProfile.Id);

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
                FakeCompanyProfile.ContactPersons.Add(FakeContactPerson);
                FakeCompanyProfile.ContactPersons.Add(FakeUpdateContactPerson);
                FakeCompanyProfile.ContactPersons.Add(FakeDeleteContactPerson);
                context.CompanyProfile.Add(FakeCompanyProfile);
                context.SaveChanges();
            }
        }
    }
}