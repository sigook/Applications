using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Location;
using Covenant.Common.Models.Notification;
using Covenant.Common.Models.Security;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Candidate;
using Covenant.Common.Repositories.Notification;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Candidate;
using Covenant.Infrastructure.Repositories.Notification;
using Covenant.Infrastructure.Repositories.Request;
using Covenant.Infrastructure.Repositories.Worker;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.WorkerModule.WorkerProfileTest
{
    public class WorkerProfileControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory<WorkerProfileControllerIntegrationTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public WorkerProfileControllerIntegrationTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Post_Should_Create_And_Return_WorkerProfile()
        {
            // Arrange
            var model = new WorkerProfileCreateModel
            {
                ProfileImage = new CovenantFileModel("profile.png", "profile"),
                FirstName = "Bill",
                MiddleName = "Henry",
                LastName = "Gates",
                SecondLastName = "Smith",
                BirthDay = new DateTime(1990, 01, 01),
                Gender = new BaseModel<Guid>(Startup.FakeGender.Id),
                Location = new LocationModel
                {
                    Address = "Medina Wash",
                    PostalCode = "A1A1A1",
                    City = new CityModel(Startup.FakeCity.Id, Startup.FakeCity.Value)
                },
                Email = $"fake_worker@mail.com",
                MobileNumber = "647-909-7182",
                Phone = "416-123-4567",
                Lift = new BaseModel<Guid>(Startup.FakeLift.Id),
                Availabilities = new[] { new BaseModel<Guid>(Startup.FakeAvailability.Id) },
                AvailabilityTimes = new[] { new BaseModel<Guid>(Startup.FakeAvailabilityTime.Id) },
                AvailabilityDays = new[] { new BaseModel<Guid>(Startup.FakeDay.Id) },
                Languages = new[] { new BaseModel<Guid>(Startup.FakeLanguage.Id) },
                Skills = new[] { new SkillModel { Skill = "Forklift" } },
                ContactEmergencyName = "Melinda",
                ContactEmergencyLastName = "Gates",
                ContactEmergencyPhone = "647-908-7124",
                JobExperiences = new[]
                {
                    new WorkerProfileJobExperienceModel
                    {
                        Company = "Microsoft",
                        Duties = "Some duties",
                        StartDate = new DateTime(2019, 01, 01),
                        IsCurrentJobPosition = true
                    }
                },
                Password = "StrongPass1",
                ConfirmPassword = "StrongPass1",
                IdentificationNumber1 = "ABC123456",
                IdentificationNumber2 = "XYZ987654",
                IdentificationType1File = new CovenantFileModel("id1.png", "image/png"),
                IdentificationType2File = new CovenantFileModel("id2.png", "image/png"),
                HasVehicle = true
            };

            using (var content = new MultipartFormDataContent())
            {
                var json = JsonConvert.SerializeObject(model);
                content.Add(new StringContent(json), "data");

                var response = await _client.PostAsync("api/WorkerProfile", content);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var workerProfileId = await response.Content.ReadFromJsonAsync<Guid>();
                var context = _factory.Services.GetRequiredService<CovenantContext>();
                var detail = await context.WorkerProfile.FindAsync(workerProfileId);
                Assert.NotNull(detail);
                Assert.Equal(model.FirstName, detail.FirstName);
                Assert.Equal(model.LastName, detail.LastName);
                Assert.Equal(model.Email, detail.Worker.Email);
                Assert.Equal(model.Location?.Address, detail.Location?.Address);
                Assert.Equal(model.Location?.City?.Id, detail.Location?.City?.Id);
                Assert.Equal(model.MobileNumber, detail.MobileNumber);
                Assert.Equal(model.Lift?.Id, detail.Lift?.Id);
            }
        }

        public class Startup
        {
            public static readonly Gender FakeGender = new("Male");
            public static readonly City FakeCity = new() { Province = new Province { Country = new Country() }, Value = "Toronto" };
            public static readonly Availability FakeAvailability = new("Full Time");
            public static readonly AvailabilityTime FakeAvailabilityTime = new("Morning");
            public static readonly Day FakeDay = new("Monday");
            public static readonly Lift FakeLift = new("18 lbs");
            public static readonly Language FakeLanguage = new("English");
            public static readonly User FakeWorker = new(CvnEmail.Create("fake_worker@mail.com").Value, Guid.NewGuid());

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder().AddTestAuth(o =>
                {
                    o.AddSub(FakeWorker.Id);
                    o.AddWorkerRole();
                });
                services.AddHttpClient();

                services.AddDbContext<CovenantContext>(b =>
                    b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);

                var identityServerService = new Mock<IIdentityServerService>();
                identityServerService.Setup(c => c.CreateUser(It.IsAny<CreateUserModel>()))
                    .ReturnsAsync(Result.Ok(new User(FakeWorker.Email, FakeWorker.Id)));

                var teamNotification = new Mock<ITeamsService>();
                teamNotification.Setup(t => t.SendNotification(It.IsAny<string>(), It.IsAny<TeamsNotificationModel>()))
                    .ReturnsAsync(Result.Ok());

                services.AddSingleton(identityServerService.Object);
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<IWorkerRepository, WorkerRepository>();
                services.AddSingleton<IRequestRepository, RequestRepository>();
                services.AddSingleton<IWorkerRequestRepository, WorkerRequestRepository>();
                services.AddSingleton<ICandidateRepository, CandidateRepository>();
                services.AddSingleton<INotificationRepository, NotificationRepository>();
                services.AddSingleton<IWorkerService, WorkerService>();
                services.AddSingleton(teamNotification.Object);
                services.AddAutoMapper(typeof(Startup).Assembly);
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

                context.Gender.Add(FakeGender);
                context.City.Add(FakeCity);
                context.Availability.Add(FakeAvailability);
                context.AvailabilityTime.Add(FakeAvailabilityTime);
                context.Day.Add(FakeDay);
                context.Lift.Add(FakeLift);

                var agencyUser = new User(CvnEmail.Create("a@agency.com").Value, Guid.NewGuid());
                var agency = new Covenant.Common.Entities.Agency.Agency("", "")
                {
                    Id = agencyUser.Id,
                    AgencyType = AgencyType.Master
                };
                agency.AddLocation(Location.Create(FakeCity.Id, "Street False 123", "A1A1A1").Value);
                context.Agencies.Add(agency);

                context.SaveChanges();
            }
        }
    }
}
