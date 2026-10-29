using Covenant.Api.Authorization;
using Covenant.Api.Controllers.Agencies.Recruiting.Clients;
using Covenant.Common.Entities.Agencies;
using Covenant.Common.Entities.Companies;
using Covenant.Common.Entities.Requests;
using Covenant.Common.Entities;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Companies.Agencies;
using Covenant.Common.Models.Companies;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Companies;
using Covenant.Common.Repositories;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces.Agencies;
using Covenant.Core.BL.Services.Agencies;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Companies;
using Covenant.Infrastructure.Repositories;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using Xunit;

namespace Covenant.Integration.Tests.Agencies.Recruiting.Clients;

public class JobPositionsControllerTest
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private const string GeneralLabour = "General Labour";
    private const string Driver = "Driver";
    private readonly Agency FakeAgency;
    private readonly CompanyProfile FakeCompanyProfile;
    private readonly CompanyProfileJobPositionRate FakePosition;
    private readonly CompanyProfileJobPositionRate FakeUpdatePosition;
    private readonly CompanyProfileJobPositionRate FakeDeletePosition;
    private readonly CompanyProfileJobPositionRate FakePositionWithoutRates;
    private readonly CompanyProfileJobPositionRate FakeDeletedPosition;
    private readonly Request FakeRequestPendingRates;
    private readonly Request FakeRequestWithSalary;

    public JobPositionsControllerTest()
    {
        _factory = new CustomWebApplicationFactory()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddDefaultTestConfiguration();
                    services.AddTestAuthenticationBuilder()
                        .AddTestAuth(o =>
                        {
                            o.AddAgencyPersonnelRole(FakeAgency.Id);
                            o.AddName();
                        });
                    services.AddTestDatabase();
                    services.AddSingleton<ICompanyRepository, CompanyRepository>();
                    services.AddSingleton<IShiftRepository, ShiftRepository>();
                    services.AddSingleton<IAgencyService, AgencyService>();
                    services.AddSingleton<AgencyIdFilter>();
                });
            });
        FakeAgency = new Agency() { User = FakeData.FakeUser() };
        FakeCompanyProfile = new CompanyProfile(new User(CvnEmail.Create("c@mail.com").Value), FakeAgency,
            "", "", new CompanyProfileIndustry("Company Industry"));
        FakePosition = CompanyProfileJobPositionRate.Create(FakeCompanyProfile.Id, "Forklift List", 2, 1, default, "tst@mail.com").Value;
        FakeUpdatePosition = CompanyProfileJobPositionRate.Create(FakeCompanyProfile.Id, "General Labour", 2, 1, default, "tst@mail.com").Value;
        FakeDeletePosition = CompanyProfileJobPositionRate.Create(FakeCompanyProfile.Id, "Assistance", 2, 1, default, "tst@mail.com").Value;
        FakePositionWithoutRates = CompanyProfileJobPositionRate.CompanyCreate(FakeCompanyProfile.Id, "Assembler").Value;
        FakeDeletedPosition = CompanyProfileJobPositionRate.CompanyCreate(FakeCompanyProfile.Id, "Welder").Value;
        FakeDeletedPosition.Delete("tst@mail.com");
        FakeRequestPendingRates = FakeData.FakeRequest(FakeAgency.Id, FakeCompanyProfile.Id, FakePositionWithoutRates.Id,
            workerRate: 0, agencyRate: 0);
        FakeRequestWithSalary = FakeData.FakeRequest(FakeAgency.Id, FakeCompanyProfile.Id, FakePositionWithoutRates.Id,
            workerRate: null, agencyRate: null);
        FakeRequestWithSalary.UpdateRates(50000M, null, null, FakePositionWithoutRates.Id);
        _client = _factory.CreateClient();
        Seed(_factory.Services.GetService<CovenantContext>());
    }

    private string RequestUri() => JobPositionsController.RouteName.Replace("{profileId}",
        FakeCompanyProfile.Id.ToString());

    [Fact]
    public async Task Post()
    {
        var model = new CompanyProfileJobPositionRateModel
        {
            JobPosition = "Picker",
            Rate = 0.5M,
            WorkerRate = 0.4M,
            Description = "Night Position",
            WorkerRateMin = 0.4M,
            WorkerRateMax = 0.5M,
            Shift = new ShiftModel
            {
                Monday = true,
                MondayStart = TimeSpan.Parse("08:00"),
                MondayFinish = TimeSpan.Parse("16:00"),
                Wednesday = true,
                WednesdayStart = TimeSpan.Parse("09:00"),
                WednesdayFinish = TimeSpan.Parse("17:00"),
                Friday = true,
                FridayStart = TimeSpan.Parse("10:00"),
                FridayFinish = TimeSpan.Parse("18:00")
            }
        };
        var response = await HttpClientJsonExtensions.PostAsJsonAsync(_client, RequestUri(), model);
        response.EnsureSuccessStatusCode();
        var detail = await response.Content.ReadFromJsonAsync<CompanyProfileJobPositionRateModel>();
        var context = _factory.Services.GetRequiredService<CovenantContext>();
        var entity = await context.CompanyProfileJobPositionRates.SingleAsync(c => c.Id == detail.Id);
        AssertEntityAndModel(model, entity);
        Assert.True(entity.CreatedAt <= DateTime.Now);
        Assert.NotNull(entity.CreatedBy);
    }

    [Fact]
    public async Task Put()
    {
        var model = new CompanyProfileJobPositionRateModel
        {
            JobPosition = "AZ Driver",
            Rate = 1000M,
            WorkerRate = 1000M,
            Description = "Day Position",
            WorkerRateMin = 1000M,
            WorkerRateMax = 1000M,
            Shift = new ShiftModel
            {
                Sunday = true,
                SundayStart = TimeSpan.Parse("00:00"),
                SundayFinish = TimeSpan.Parse("01:00"),
                Monday = true,
                MondayStart = TimeSpan.Parse("01:00"),
                MondayFinish = TimeSpan.Parse("02:00"),
                Tuesday = true,
                TuesdayStart = TimeSpan.Parse("02:00"),
                TuesdayFinish = TimeSpan.Parse("03:00"),
                Wednesday = true,
                WednesdayStart = TimeSpan.Parse("03:00"),
                WednesdayFinish = TimeSpan.Parse("04:00"),
                Thursday = true,
                ThursdayStart = TimeSpan.Parse("05:00"),
                ThursdayFinish = TimeSpan.Parse("06:00"),
                Friday = true,
                FridayStart = TimeSpan.Parse("07:00"),
                FridayFinish = TimeSpan.Parse("08:00"),
                Saturday = true,
                SaturdayStart = TimeSpan.Parse("09:00"),
                SaturdayFinish = TimeSpan.Parse("10:00"),
            }
        };
        Guid id = FakeUpdatePosition.Id;
        HttpResponseMessage response = await HttpClientJsonExtensions.PutAsJsonAsync(_client, $"{RequestUri()}/{id}", model);
        response.EnsureSuccessStatusCode();
        var context = _factory.Services.GetRequiredService<CovenantContext>();
        CompanyProfileJobPositionRate entity = await context.CompanyProfileJobPositionRates.SingleAsync(c => c.Id == id);
        AssertEntityAndModel(model, entity);
        Assert.True(entity.UpdatedAt <= DateTime.Now);
        Assert.NotNull(entity.CreatedBy ?? entity.UpdatedBy);
    }

    private void AssertEntityAndModel(CompanyProfileJobPositionRateModel model, CompanyProfileJobPositionRate entity)
    {
        Assert.Equal(model.Rate, entity.Rate);
        Assert.Equal(model.WorkerRate, entity.WorkerRate);
        Assert.Equal(model.WorkerRateMin, entity.WorkerRateMin);
        Assert.Equal(model.WorkerRateMax, entity.WorkerRateMax);
        Assert.Equal(model.Description, entity.Description);
        Assert.Equal(model.JobPosition, entity.JobPosition);
        Assert.Equal(model.Shift?.Sunday, entity.Shift?.Sunday);
        Assert.Equal(model.Shift?.SundayStart, entity.Shift?.SundayStart);
        Assert.Equal(model.Shift?.SundayFinish, entity.Shift?.SundayFinish);
        Assert.Equal(model.Shift?.Monday, entity.Shift?.Monday);
        Assert.Equal(model.Shift?.MondayStart, entity.Shift?.MondayStart);
        Assert.Equal(model.Shift?.MondayFinish, entity.Shift?.MondayFinish);
        Assert.Equal(model.Shift?.Tuesday, entity.Shift?.Tuesday);
        Assert.Equal(model.Shift?.TuesdayStart, entity.Shift?.TuesdayStart);
        Assert.Equal(model.Shift?.TuesdayFinish, entity.Shift?.TuesdayFinish);
        Assert.Equal(model.Shift?.Wednesday, entity.Shift?.Wednesday);
        Assert.Equal(model.Shift?.WednesdayStart, entity.Shift?.WednesdayStart);
        Assert.Equal(model.Shift?.WednesdayFinish, entity.Shift?.WednesdayFinish);
        Assert.Equal(model.Shift?.Thursday, entity.Shift?.Thursday);
        Assert.Equal(model.Shift?.ThursdayStart, entity.Shift?.ThursdayStart);
        Assert.Equal(model.Shift?.ThursdayFinish, entity.Shift?.ThursdayFinish);
        Assert.Equal(model.Shift?.Friday, entity.Shift?.Friday);
        Assert.Equal(model.Shift?.FridayStart, entity.Shift?.FridayStart);
        Assert.Equal(model.Shift?.FridayFinish, entity.Shift?.FridayFinish);
        Assert.Equal(model.Shift?.Saturday, entity.Shift?.Saturday);
        Assert.Equal(model.Shift?.SaturdayStart, entity.Shift?.SaturdayStart);
        Assert.Equal(model.Shift?.SaturdayFinish, entity.Shift?.SaturdayFinish);
    }

    [Fact]
    public async Task Get()
    {
        HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}");
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<CompanyProfileJobPositionRateModel>>();
        Assert.NotEmpty(list.Items);
        var entity = FakePosition;
        var model = list.Items.Single(c =>
            c.Id == entity.Id &&
            c.Rate == entity.Rate &&
            c.WorkerRate == entity.WorkerRate &&
            c.Description == entity.Description &&
            c.JobPosition == entity.JobPosition &&
            c.WorkerRateMin == entity.WorkerRateMin &&
            c.WorkerRateMax == entity.WorkerRateMax &&
            c.CreatedBy == entity.CreatedBy &&
            c.DisplayShift == entity.Shift?.DisplayShift);
        Assert.NotNull(model);
    }

    [Fact]
    public async Task Get_FiltersByRoleOnServer()
    {
        HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}?role=forklift");
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<CompanyProfileJobPositionRateModel>>();
        var model = Assert.Single(list.Items);
        Assert.Equal(FakePosition.Id, model.Id);
        Assert.Equal(1, list.TotalItems);
    }

    [Fact]
    public async Task Get_SortsAndPaginatesOnServer()
    {
        HttpResponseMessage response = await _client.GetAsync(
            $"{RequestUri()}?sortBy={GetJobPositionsSortBy.Role}&isDescending=true&pageSize=1&pageIndex=1");
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<CompanyProfileJobPositionRateModel>>();
        var model = Assert.Single(list.Items);
        Assert.Equal(FakeUpdatePosition.Id, model.Id);
        Assert.True(list.TotalItems >= 2);
    }

    [Fact]
    public async Task GetDropdown()
    {
        HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/Dropdown");
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<List<CompanyProfileJobPositionRateModel>>();
        Assert.Contains(list, c => c.Id == FakePosition.Id);
        Assert.Contains(list, c => c.Id == FakeUpdatePosition.Id);
    }

    [Fact]
    public async Task GetById()
    {
        var entity = FakePosition;
        HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/{entity.Id}");
        response.EnsureSuccessStatusCode();
        var model = await response.Content.ReadFromJsonAsync<CompanyProfileJobPositionRateModel>();
        Assert.Equal(entity.Id, model.Id);
        Assert.True(entity.CreatedAt <= DateTime.Now);
        Assert.NotNull(entity.CreatedBy);
        AssertEntityAndModel(model, entity);
    }

    [Fact]
    public async Task Delete()
    {
        Guid id = FakeDeletePosition.Id;
        HttpResponseMessage response = await _client.DeleteAsync($"{RequestUri()}/{id}");
        response.EnsureSuccessStatusCode();
        var context = _factory.Services.GetRequiredService<CovenantContext>();
        CompanyProfileJobPositionRate entity = await context.Set<CompanyProfileJobPositionRate>().SingleAsync(c => c.Id == id);
        Assert.True(entity.IsDeleted);
        Assert.True(entity.UpdatedAt <= DateTime.Now);
        Assert.NotNull(entity.UpdatedBy);
    }

    [Fact]
    public async Task PostWithoutRates()
    {
        var model = new CreateJobPositionModel
        {
            JobPosition = "Clerical",
            Description = "Front desk",
            Shift = new ShiftModel
            {
                Monday = true,
                MondayStart = TimeSpan.Parse("08:00"),
                MondayFinish = TimeSpan.Parse("16:00")
            }
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync($"{RequestUri()}/WithoutRates", model);
        response.EnsureSuccessStatusCode();
        var detail = await response.Content.ReadFromJsonAsync<CompanyProfileJobPositionRateModel>();
        var context = _factory.Services.GetRequiredService<CovenantContext>();
        var entity = await context.CompanyProfileJobPositionRates.Include(c => c.Shift).SingleAsync(c => c.Id == detail.Id);
        Assert.Equal(model.JobPosition, entity.JobPosition);
        Assert.Equal(model.Description, entity.Description);
        Assert.Equal(FakeCompanyProfile.Id, entity.CompanyProfileId);
        Assert.Equal(0, entity.Rate);
        Assert.Equal(0, entity.WorkerRate);
        Assert.NotNull(entity.CreatedBy);
        Assert.True(entity.Shift.Monday);
        Assert.Equal(TimeSpan.Parse("08:00"), entity.Shift.MondayStart);
    }

    [Fact]
    public async Task PostWithoutRatesRequiresName()
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync($"{RequestUri()}/WithoutRates", new CreateJobPositionModel());
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostRejectsSameNameAndRates()
    {
        var model = new CompanyProfileJobPositionRateModel { JobPosition = " forklift LIST ", Rate = 2M, WorkerRate = 1M };
        HttpResponseMessage response = await _client.PostAsJsonAsync(RequestUri(), model);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var context = _factory.Services.GetRequiredService<CovenantContext>();
        Assert.Equal(1, await context.CompanyProfileJobPositionRates.CountAsync(c =>
            c.CompanyProfileId == FakeCompanyProfile.Id && c.JobPosition.ToLower().Trim() == "forklift list"));
    }

    [Fact]
    public async Task PostAllowsSameNameWithDifferentRates()
    {
        var model = new CompanyProfileJobPositionRateModel { JobPosition = FakePosition.JobPosition, Rate = 3M, WorkerRate = 1M };
        HttpResponseMessage response = await _client.PostAsJsonAsync(RequestUri(), model);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task PostWithoutRatesAllowsNameOfRoleWithRates()
    {
        var model = new CreateJobPositionModel { JobPosition = FakePosition.JobPosition };
        HttpResponseMessage response = await _client.PostAsJsonAsync($"{RequestUri()}/WithoutRates", model);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task PostWithoutRatesRejectsDuplicateName()
    {
        var model = new CreateJobPositionModel { JobPosition = FakePositionWithoutRates.JobPosition };
        HttpResponseMessage response = await _client.PostAsJsonAsync($"{RequestUri()}/WithoutRates", model);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostAllowsNameOfDeletedRole()
    {
        var model = new CreateJobPositionModel { JobPosition = FakeDeletedPosition.JobPosition };
        HttpResponseMessage response = await _client.PostAsJsonAsync($"{RequestUri()}/WithoutRates", model);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task PutRejectsNameAndRatesOfAnotherRole()
    {
        var model = new CompanyProfileJobPositionRateModel { JobPosition = FakePosition.JobPosition, Rate = 2M, WorkerRate = 1M };
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{RequestUri()}/{FakeDeletePosition.Id}", model);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var context = _factory.Services.GetRequiredService<CovenantContext>();
        var entity = await context.CompanyProfileJobPositionRates.AsNoTracking().SingleAsync(c => c.Id == FakeDeletePosition.Id);
        Assert.Equal("Assistance", entity.JobPosition);
    }

    [Fact]
    public async Task PutKeepsOwnName()
    {
        var model = new CompanyProfileJobPositionRateModel { JobPosition = FakePosition.JobPosition, Rate = 3M, WorkerRate = 2M };
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{RequestUri()}/{FakePosition.Id}", model);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task PutRatesPropagatesToRequestsPendingRates()
    {
        var model = new CompanyProfileJobPositionRateModel
        {
            JobPosition = FakePositionWithoutRates.JobPosition,
            Rate = 25M,
            WorkerRate = 18M
        };
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{RequestUri()}/{FakePositionWithoutRates.Id}", model);
        response.EnsureSuccessStatusCode();
        var context = _factory.Services.GetRequiredService<CovenantContext>();
        var pending = await context.Requests.AsNoTracking().SingleAsync(r => r.Id == FakeRequestPendingRates.Id);
        Assert.Equal(25M, pending.AgencyRate);
        Assert.Equal(18M, pending.WorkerRate);
        Assert.True(pending.HasRates);
        var salary = await context.Requests.AsNoTracking().SingleAsync(r => r.Id == FakeRequestWithSalary.Id);
        Assert.Equal(50000M, salary.WorkerSalary);
        Assert.Null(salary.AgencyRate);
    }

    private void Seed(CovenantContext context)
    {
        var shift = new Shift();
        shift.AddSunday(TimeSpan.Parse("08:00"), TimeSpan.Parse("16:00"));
        shift.AddSaturday(TimeSpan.Parse("09:00"), TimeSpan.Parse("17:00"));
        FakePosition.AddShift(shift);
        FakeCompanyProfile.JobPositionRates.Add(FakePosition);
        FakeCompanyProfile.JobPositionRates.Add(FakeUpdatePosition);
        FakeCompanyProfile.JobPositionRates.Add(FakeDeletePosition);
        FakeCompanyProfile.JobPositionRates.Add(FakePositionWithoutRates);
        FakeCompanyProfile.JobPositionRates.Add(FakeDeletedPosition);
        context.CompanyProfiles.Add(FakeCompanyProfile);
        context.Requests.AddRange(FakeRequestPendingRates, FakeRequestWithSalary);
        context.SaveChanges();
    }
}
