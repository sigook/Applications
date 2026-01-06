using Covenant.Api;
using Covenant.Api.AgencyModule.AgencyCompanyProfileJobPosition.Controllers;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.Company.Models;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories;
using Covenant.Infrastructure.Repositories.Company;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.AgencyCompanyProfileJobPosition
{
    public class AgencyCompanyProfileJobPositionControllerTest
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly JobPosition GeneralLabour;
        private readonly JobPosition Driver;
        private readonly Covenant.Common.Entities.Agency.Agency FakeAgency;
        private readonly CompanyProfile FakeCompanyProfile;
        private readonly CompanyProfileJobPositionRate FakePosition;
        private readonly CompanyProfileJobPositionRate FakeUpdatePosition;
        private readonly CompanyProfileJobPositionRate FakeDeletePosition;

        public AgencyCompanyProfileJobPositionControllerTest()
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
                        services.AddDbContext<CovenantContext>(b
                            => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                        services.AddSingleton<ICompanyRepository, CompanyRepository>();
                        services.AddSingleton<ITimeService, TimeService>();
                        services.AddSingleton<IShiftRepository, ShiftRepository>();
                        services.AddSingleton<IAgencyService, AgencyService>();
                        services.AddSingleton<AgencyIdFilter>();
                    });
                    builder.UseSetting("EmailsPetitionNewJobPosition", "e@sigook.com,b@siggok.com");
                });
            GeneralLabour = new JobPosition { Industry = new Industry(), Value = "General Labour" };
            Driver = new JobPosition { Industry = new Industry(), Value = "Driver" };
            FakeAgency = new Covenant.Common.Entities.Agency.Agency();
            FakeCompanyProfile = new CompanyProfile(new User(CvnEmail.Create("c@mail.com").Value), FakeAgency,
                "", "", "", new CompanyProfileIndustry("Company Industry"));
            FakePosition = CompanyProfileJobPositionRate.Create(FakeCompanyProfile.Id, "Forklift List", 2, 1, default, "tst@mail.com").Value;
            FakeUpdatePosition = CompanyProfileJobPositionRate.Create(FakeCompanyProfile.Id, "General Labour", 2, 1, default, "tst@mail.com").Value;
            FakeDeletePosition = CompanyProfileJobPositionRate.Create(FakeCompanyProfile.Id, "Assistance", 2, 1, default, "tst@mail.com").Value;
            _client = _factory.CreateClient();
            Seed(_factory.Services.GetService<CovenantContext>());
        }

        private string RequestUri() => AgencyCompanyProfileJobPositionController.RouteName.Replace("{profileId}",
            FakeCompanyProfile.Id.ToString());

        [Theory]
        [InlineData("Other")]
        [InlineData("General")]
        public async Task Post(string jobPosition)
        {
            var model = new CompanyProfileJobPositionRateModel
            {
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
            if (jobPosition == "Other") model.OtherJobPosition = "General Labour";
            else model.JobPosition = new JobPositionDetailModel
            {
                Id = GeneralLabour.Id,
                Value = GeneralLabour.Value
            };
            var response = await HttpClientJsonExtensions.PostAsJsonAsync(_client, RequestUri(), model);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<CompanyProfileJobPositionRateModel>();
            var context = _factory.Services.GetRequiredService<CovenantContext>();
            var entity = await context.CompanyProfileJobPositionRate.SingleAsync(c => c.Id == detail.Id);
            AssertEntityAndModel(model, entity);
            Assert.True(entity.CreatedAt <= DateTime.Now);
            Assert.NotNull(entity.CreatedBy);
        }

        [Theory]
        [InlineData("Other")]
        [InlineData("General")]
        public async Task Put(string jobPosition)
        {
            var model = new CompanyProfileJobPositionRateModel
            {
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
            if (jobPosition == "Other") model.OtherJobPosition = "AZ Driver";
            else model.JobPosition = new JobPositionDetailModel
            {
                Id = Driver.Id,
                Value = Driver.Value
            };
            Guid id = FakeUpdatePosition.Id;
            HttpResponseMessage response = await HttpClientJsonExtensions.PutAsJsonAsync(_client, $"{RequestUri()}/{id}", model);
            response.EnsureSuccessStatusCode();
            var context = _factory.Services.GetRequiredService<CovenantContext>();
            CompanyProfileJobPositionRate entity = await context.CompanyProfileJobPositionRate.SingleAsync(c => c.Id == id);
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
            Assert.Equal(model.OtherJobPosition, entity.OtherJobPosition);
            Assert.Equal(model.JobPosition?.Id, entity.JobPositionId);
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
            var list = await response.Content.ReadAsJsonAsync<IEnumerable<CompanyProfileJobPositionRateModel>>();
            Assert.NotEmpty(list);
            var entity = FakePosition;
            var model = list.Single(c =>
                c.Id == entity.Id &&
                c.Rate == entity.Rate &&
                c.WorkerRate == entity.WorkerRate &&
                c.Description == entity.Description &&
                c.OtherJobPosition == entity.OtherJobPosition &&
                c.JobPosition?.Id == entity.JobPosition?.Id &&
                c.WorkerRateMin == entity.WorkerRateMin &&
                c.WorkerRateMax == entity.WorkerRateMax &&
                c.CreatedBy == entity.CreatedBy &&
                c.DisplayShift == entity.Shift?.DisplayShift);
            Assert.NotNull(model);
        }

        [Fact]
        public async Task GetById()
        {
            var entity = FakePosition;
            HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/{entity.Id}");
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadAsJsonAsync<CompanyProfileJobPositionRateModel>();
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
        public async Task Petition()
        {
            var model = new JobPositionPetitionModel
            {
                JobPosition = "Clerical"
            };
            HttpResponseMessage response = await _client.PostAsJsonAsync($"{RequestUri()}/Petition", model);
            response.EnsureSuccessStatusCode();
        }

        private void Seed(CovenantContext context)
        {
            context.JobPosition.Add(GeneralLabour);
            var shift = new Shift();
            shift.AddSunday(TimeSpan.Parse("08:00"), TimeSpan.Parse("16:00"));
            shift.AddSaturday(TimeSpan.Parse("09:00"), TimeSpan.Parse("17:00"));
            FakePosition.AddShift(shift);
            FakeCompanyProfile.JobPositionRates.Add(FakePosition);
            FakeCompanyProfile.JobPositionRates.Add(FakeUpdatePosition);
            FakeCompanyProfile.JobPositionRates.Add(FakeDeletePosition);
            context.CompanyProfile.Add(FakeCompanyProfile);
            context.SaveChanges();
        }
    }
}