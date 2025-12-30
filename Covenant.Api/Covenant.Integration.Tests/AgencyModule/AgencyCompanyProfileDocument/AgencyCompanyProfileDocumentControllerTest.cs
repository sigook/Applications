using Covenant.Api.AgencyModule.AgencyCompanyProfileDocument.Controllers;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Company;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.AgencyCompanyProfileDocument
{
    public class AgencyCompanyProfileDocumentControllerTest : IClassFixture<CustomWebApplicationFactory<AgencyCompanyProfileDocumentControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public AgencyCompanyProfileDocumentControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => AgencyCompanyProfileDocumentController.RouteName.Replace("{profileId}",
            Startup.FakeCompanyProfile.Id.ToString());

        [Fact]
        public async Task Post()
        {
            var model = new CovenantFileModel("contract.pdf", "Contract 2021");
            HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(_client, RequestUri(), model);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<Guid>();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.CompanyProfileDocuments.SingleAsync(c => c.DocumentId == detail);
            Assert.NotNull(entity.CreatedBy);
            Assert.True(entity.CreatedAt <= DateTime.Now);
            Assert.Equal(model.FileName, entity.Document.FileName);
            Assert.Equal(model.Description, entity.Document.Description);
        }

        [Fact]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync(RequestUri());
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<PaginatedList<CovenantFileModel>>();
            Assert.NotEmpty(list.Items);
            var entity = Startup.FakeDocument;
            var model = list.Items.Single(c => c.Id == entity.DocumentId);
            AssertDetailAndEntity(model, entity);
        }

        private static void AssertDetailAndEntity(CovenantFileModel model, CompanyProfileDocument entity)
        {
            Assert.Equal(model.Id, entity.DocumentId);
            Assert.Equal(model.FileName, entity.Document.FileName);
            Assert.Equal(model.Description, entity.Document.Description);
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o =>
                    {
                        o.AddAgencyPersonnelRole();
                        o.AddName("recruiter@mail.com");
                    });
                services.AddDbContext<CovenantContext>(b
                    => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<ICompanyRepository, CompanyRepository>();
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<AgencyIdFilter>();
            }

            private static readonly Covenant.Common.Entities.Agency.Agency FakeAgency = new Covenant.Common.Entities.Agency.Agency();
            public static readonly CompanyProfile FakeCompanyProfile = new CompanyProfile(new User(CvnEmail.Create("c@mail.com").Value), FakeAgency,
                "", "", "", new CompanyProfileIndustry("Company Industry"));

            public static readonly CompanyProfileDocument FakeDocument = new CompanyProfileDocument(FakeCompanyProfile.Id, CovenantFile.Create("doc.pdf").Value, "u@mail.com");
            public static readonly CompanyProfileDocument FakeDeleteDocument = new CompanyProfileDocument(FakeCompanyProfile.Id, CovenantFile.Create("delete.pdf").Value, "u@mail.com");


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
                FakeCompanyProfile.Documents.Add(FakeDocument);
                FakeCompanyProfile.Documents.Add(FakeDeleteDocument);
                context.CompanyProfile.Add(FakeCompanyProfile);
                context.SaveChanges();
            }
        }
    }
}