using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Company;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.CompanyModule.CompanyAgencyJobPosition
{
    public class CompanyJobPositionControllerTest : IClassFixture<CustomWebApplicationFactory<CompanyJobPositionControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public CompanyJobPositionControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => "api/CompanyJobPosition";

        [Fact]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync(RequestUri());
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<IEnumerable<CompanyProfileJobPositionRateModel>>();
            Assert.NotEmpty(list);
            var entity = Startup.FakePosition;
            var model = list.Single(c =>
                c.Id == entity.Id &&
                c.Rate == entity.Rate &&
                c.OtherJobPosition == entity.OtherJobPosition &&
                c.JobPosition?.Value == entity.JobPosition?.Value &&
                !string.IsNullOrEmpty(c.Value));
            Assert.NotNull(model);
        }

        [Fact]
        public async Task GetById()
        {
            CompanyProfileJobPositionRate entity = Startup.FakePosition;
            HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/{entity.Id}");
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadAsJsonAsync<CompanyProfileJobPositionRateModel>();
            Assert.Equal(entity.Id, model.Id);
            Assert.Equal(entity.Rate, model.Rate);
            Assert.Equal(entity.Description, model.Description);
            Assert.Equal(entity.OtherJobPosition, model.OtherJobPosition);
            Assert.Equal(entity.JobPosition?.Value, model.JobPosition?.Value);
            Assert.NotNull(model.Value);
            Assert.Equal(entity.Shift?.Sunday, model.Shift?.Sunday);
            Assert.Equal(entity.Shift?.SundayStart, model.Shift?.SundayStart);
            Assert.Equal(entity.Shift?.SundayFinish, model.Shift?.SundayFinish);
            Assert.Equal(entity.Shift?.Monday, model.Shift?.Monday);
            Assert.Equal(entity.Shift?.MondayStart, model.Shift?.MondayStart);
            Assert.Equal(entity.Shift?.MondayFinish, model.Shift?.MondayFinish);
            Assert.Equal(entity.Shift?.Tuesday, model.Shift?.Tuesday);
            Assert.Equal(entity.Shift?.TuesdayStart, model.Shift?.TuesdayStart);
            Assert.Equal(entity.Shift?.TuesdayFinish, model.Shift?.TuesdayFinish);
            Assert.Equal(entity.Shift?.Wednesday, model.Shift?.Wednesday);
            Assert.Equal(entity.Shift?.WednesdayStart, model.Shift?.WednesdayStart);
            Assert.Equal(entity.Shift?.WednesdayFinish, model.Shift?.WednesdayFinish);
            Assert.Equal(entity.Shift?.Thursday, model.Shift?.Thursday);
            Assert.Equal(entity.Shift?.ThursdayStart, model.Shift?.ThursdayStart);
            Assert.Equal(entity.Shift?.ThursdayFinish, model.Shift?.ThursdayFinish);
            Assert.Equal(entity.Shift?.Friday, model.Shift?.Friday);
            Assert.Equal(entity.Shift?.FridayStart, model.Shift?.FridayStart);
            Assert.Equal(entity.Shift?.FridayFinish, model.Shift?.FridayFinish);
            Assert.Equal(entity.Shift?.Saturday, model.Shift?.Saturday);
            Assert.Equal(entity.Shift?.SaturdayStart, model.Shift?.SaturdayStart);
            Assert.Equal(entity.Shift?.SaturdayFinish, model.Shift?.SaturdayFinish);
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o =>
                    {
                        o.AddSub(FakeCompanyProfile.Company.Id);
                        o.AddCompanyRole();
                        o.AddName();
                    });
                services.AddDbContext<CovenantContext>(b
                    => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<ICompanyRepository, CompanyRepository>();
                services.AddSingleton<CompanyIdFilter>();
            }

            private static readonly JobPosition GeneralLabour = new JobPosition { Industry = new Industry(), Value = "General Labour" };
            public static readonly Covenant.Common.Entities.Agency.Agency FakeAgency = new Covenant.Common.Entities.Agency.Agency();

            private static readonly CompanyProfile FakeCompanyProfile = new CompanyProfile(new User(CvnEmail.Create("c@mail.com").Value), FakeAgency,
                "", "", "", new CompanyProfileIndustry("Company Industry"));

            public static readonly CompanyProfileJobPositionRate FakePosition = CompanyProfileJobPositionRate.Create(FakeCompanyProfile.Id, "Forklift List", 2, 1, default, "tst@mail.com").Value;

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
                context.JobPosition.Add(GeneralLabour);
                var shift = new Shift();
                shift.AddSunday(TimeSpan.Parse("08:00"), TimeSpan.Parse("16:00"));
                shift.AddSaturday(TimeSpan.Parse("09:00"), TimeSpan.Parse("17:00"));
                FakePosition.AddShift(shift);
                FakeCompanyProfile.JobPositionRates.Add(FakePosition);
                context.CompanyProfile.Add(FakeCompanyProfile);
                context.SaveChanges();
            }
        }
    }
}