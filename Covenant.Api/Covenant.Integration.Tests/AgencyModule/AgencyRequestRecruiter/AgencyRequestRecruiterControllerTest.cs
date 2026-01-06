using Covenant.Api.AgencyModule.AgencyRequestRecruiter.Controllers;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Agency;
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
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.AgencyRequestRecruiter
{
    public class AgencyRequestRecruiterControllerTest :
        IClassFixture<CustomWebApplicationFactory<AgencyRequestRecruiterControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public AgencyRequestRecruiterControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => AgencyRequestRecruiterController.RouteName.Replace("{requestId}",
            Data.FakeRequest.Id.ToString());

        [Fact]
        public async Task Post()
        {
            var model = new RequestRecruiterModel { RecruiterId = Data.FakeRecruiter.Id };
            HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(_client, RequestUri(), model);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<RequestRecruiterDetailModel>();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            RequestRecruiter entity = await context.RequestRecruiter.SingleAsync(c => c.RecruiterId == detail.RecruiterId);
            Assert.Equal(model.RecruiterId, entity.RecruiterId);
            Assert.NotNull(entity.Request.DisplayRecruiters);
        }

        [Fact]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync(RequestUri());
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<PaginatedList<RequestRecruiterDetailModel>>();
            Assert.NotEmpty(list.Items);
            RequestRecruiter entity = Data.FakeRequestRecruiter;
            RequestRecruiterDetailModel model = list.Items.Single(c => c.RecruiterId == entity.RecruiterId);
            Assert.NotNull(model);
            Assert.Equal(Data.FakeRecruiterList.User.Email, model.Email);
        }

        [Fact]
        public async Task Delete()
        {
            Guid id = Data.FakeRequestRecruiterDelete.RecruiterId;
            HttpResponseMessage response = await _client.DeleteAsync($"{RequestUri()}/{id}");
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            Assert.False(await context.RequestRecruiter.AnyAsync(c => c.RecruiterId == id));
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
                services.AddSingleton<IRequestRepository, RequestRepository>();
                services.AddSingleton<ITimeService, TimeService>();
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
                context.CompanyProfile.Add(Data.FakeCompany);
                context.Request.Add(Data.FakeRequest);
                context.AgencyPersonnel.AddRange(Data.FakeRecruiter, Data.FakeRecruiterList, Data.FakeRecruiterDelete);
                context.RequestRecruiter.AddRange(Data.FakeRequestRecruiter, Data.FakeRequestRecruiterDelete);
                context.SaveChanges();
            }
        }

        private static class Data
        {
            private static readonly Guid AgencyId = Guid.NewGuid();

            internal static readonly CompanyProfile FakeCompany = CompanyProfile.AgencyCreateCompany(
                new User(CvnEmail.Create("c@m.com").Value),
                AgencyId,
                CompanyName.Create("ABX").Value,
                CompanyName.Create("ABX").Value,
                default,
                default,
                default,
                default,
                default,
                new CompanyProfileIndustry("Drivers"),
                default,
                default,
                default,
                default,
                "juan@covenantgroupl.com",
                CompanyStatus.Lead,
                null).Value;

            public static readonly Request FakeRequest;
            public static readonly AgencyPersonnel FakeRecruiter = AgencyPersonnel.CreatePrimary(AgencyId, new User(CvnEmail.Create("create@mail.com").Value), "create");
            public static readonly AgencyPersonnel FakeRecruiterList = AgencyPersonnel.CreatePrimary(AgencyId, new User(CvnEmail.Create("list@mail.com").Value), "list");
            public static readonly AgencyPersonnel FakeRecruiterDelete = AgencyPersonnel.CreatePrimary(AgencyId, new User(CvnEmail.Create("delete@mail.com").Value), "delete");
            public static readonly RequestRecruiter FakeRequestRecruiter;
            public static readonly RequestRecruiter FakeRequestRecruiterDelete;

            static Data()
            {
                FakeCompany.AddJobPositionRate(CompanyProfileJobPositionRate.Create(FakeCompany.Id, "Labor", 1, 1).Value);
                FakeRequest = FakeData.FakeRequest(AgencyId, FakeCompany.CompanyId, FakeCompany.JobPositionRates.First().Id);
                FakeRequestRecruiter = new RequestRecruiter(FakeRequest.Id, FakeRecruiterList.Id);
                FakeRequestRecruiterDelete = new RequestRecruiter(FakeRequest.Id, FakeRecruiterDelete.Id);
            }
        }
    }
}