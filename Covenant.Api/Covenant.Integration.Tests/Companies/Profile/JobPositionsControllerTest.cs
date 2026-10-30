using Covenant.Api.Authorization;
using Covenant.Api.Controllers.Companies.Profile;
using Covenant.Common.Entities.Agencies;
using Covenant.Common.Entities.Companies;
using Covenant.Common.Entities;
using Covenant.Common.Models.Companies;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Companies;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Companies;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using Xunit;

namespace Covenant.Integration.Tests.Companies.Profile;

public class JobPositionsControllerTest : IClassFixture<CustomWebApplicationFactory<JobPositionsControllerTest.Startup>>
{
    private readonly CustomWebApplicationFactory<Startup> _factory;
    private readonly HttpClient _client;

    public JobPositionsControllerTest(CustomWebApplicationFactory<Startup> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private static string RequestUri() => JobPositionsController.RouteName;

    [Fact]
    public async Task Get()
    {
        HttpResponseMessage response = await _client.GetAsync(RequestUri());
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<IEnumerable<CompanyProfileJobPositionRateModel>>();
        Assert.NotEmpty(list);
        var entity = Startup.FakePosition;
        var model = list.Single(c =>
            c.Id == entity.Id &&
            c.Rate == entity.Rate &&
            c.JobPosition == entity.JobPosition);
        Assert.NotNull(model);
    }

    [Fact]
    public async Task GetById()
    {
        CompanyProfileJobPositionRate entity = Startup.FakePosition;
        HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/{entity.Id}");
        response.EnsureSuccessStatusCode();
        var model = await response.Content.ReadFromJsonAsync<CompanyProfileJobPositionRateModel>();
        Assert.Equal(entity.Id, model.Id);
        Assert.Equal(entity.Rate, model.Rate);
        Assert.Equal(entity.Description, model.Description);
        Assert.Equal(entity.JobPosition, model.JobPosition);
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

    [Fact]
    public async Task PostCreatesJobPositionWithoutRates()
    {
        var model = new CreateJobPositionModel
        {
            JobPosition = "Picker",
            Description = "Warehouse",
            Shift = new ShiftModel
            {
                Tuesday = true,
                TuesdayStart = TimeSpan.Parse("07:00"),
                TuesdayFinish = TimeSpan.Parse("15:00")
            }
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync(RequestUri(), model);
        response.EnsureSuccessStatusCode();
        var detail = await response.Content.ReadFromJsonAsync<CompanyProfileJobPositionRateModel>();
        var context = _factory.Services.GetRequiredService<CovenantContext>();
        var entity = await context.CompanyProfileJobPositionRates.Include(c => c.Shift).SingleAsync(c => c.Id == detail.Id);
        Assert.Equal(model.JobPosition, entity.JobPosition);
        Assert.Equal(model.Description, entity.Description);
        Assert.Equal(Startup.FakePosition.CompanyProfileId, entity.CompanyProfileId);
        Assert.Equal(0, entity.Rate);
        Assert.Equal(0, entity.WorkerRate);
        Assert.False(entity.HasRates);
        Assert.True(entity.Shift.Tuesday);
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
            services.AddTestDatabase();
            services.AddSingleton<ICompanyRepository, CompanyRepository>();
            services.AddSingleton<CompanyIdFilter>();
        }

        public static readonly Agency FakeAgency = new Agency() { User = FakeData.FakeUser() };

        private static readonly CompanyProfile FakeCompanyProfile = new CompanyProfile(new User(CvnEmail.Create("c@mail.com").Value), FakeAgency,
            "", "", new CompanyProfileIndustry("Company Industry"));

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
            var shift = new Shift();
            shift.AddSunday(TimeSpan.Parse("08:00"), TimeSpan.Parse("16:00"));
            shift.AddSaturday(TimeSpan.Parse("09:00"), TimeSpan.Parse("17:00"));
            FakePosition.AddShift(shift);
            FakeCompanyProfile.JobPositionRates.Add(FakePosition);
            context.CompanyProfiles.Add(FakeCompanyProfile);
            context.SaveChanges();
        }
    }
}
