using Covenant.Api;
using Covenant.Api.AgencyModule.AgencyWorkerProfile.Controllers;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Notification;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models;
using Covenant.Common.Models.Security;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Worker;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.AgencyWorkerProfile
{
    public partial class AgencyWorkerProfileControllerTest : BaseTestOrder
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly Availability FakeAvailabilityPartTime;
        private readonly Availability FakeAvailabilityNew;
        private readonly Availability FakeAvailabilityRemove;
        private readonly Availability FakeAvailabilityDontChange;
        private readonly AvailabilityTime FakeAvailabilityTimeNew;
        private readonly Day FakeDayNew;
        private readonly Day FakeDayDontChange;
        private readonly IdentificationType FakeIdentificationType1;
        private readonly IdentificationType FakeIdentificationType2;
        private readonly Gender FakeGender;
        private readonly City FakeCity;
        private readonly Lift FakeLift;
        private readonly Language FakeLanguage;
        private readonly User FakeAgencyUser;
        private readonly Covenant.Common.Entities.Agency.Agency FakeAgency;
        private readonly WorkerProfile FakeWorkerProfileToFilter;
        private readonly WorkerProfile FakeWorkerToDelete;
        private readonly WorkerProfile FakeWorkerToDnu;
        private readonly WorkerProfile FakeWorkerIsContractor;
        private readonly WorkerProfile FakeWorkerIsSubcontractor;
        private readonly WorkerProfile FakeWorkerToUpdateEmail;

        public AgencyWorkerProfileControllerTest()
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
                                o.AddAgencyPersonnelRole(FakeAgencyUser.Id);
                                o.AddName(FakeAgencyUser.Email);
                            });
                        services.AddDbContext<CovenantContext>(b =>
                            b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                        services.AddSingleton<IWorkerRepository, WorkerRepository>();
                        services.AddSingleton(new Mock<ITimeService>().Object);
                        var identityServerService = new Mock<IIdentityServerService>();
                        identityServerService.Setup(c => c.CreateUser(It.IsAny<CreateUserModel>())).ReturnsAsync(Result.Ok(new User("email@test.com", Guid.NewGuid())));
                        services.AddSingleton(identityServerService.Object);
                        var filesContainer = new Mock<IFilesContainer>();
                        filesContainer.Setup(c => c.FileExist(It.IsAny<string>())).ReturnsAsync(true);
                        services.AddSingleton(filesContainer.Object);
                        services.AddSingleton<AgencyIdFilter>();
                    });
                });
            FakeAvailabilityPartTime = new Availability("Part Time");
            FakeAvailabilityNew = new Availability();
            FakeAvailabilityRemove = new Availability();
            FakeAvailabilityDontChange = new Availability();
            FakeDayNew = new Day("Sunday");
            FakeDayDontChange = new Day("Monday");
            FakeAvailabilityTimeNew = new AvailabilityTime();
            FakeIdentificationType1 = new IdentificationType();
            FakeIdentificationType2 = new IdentificationType();
            FakeGender = new Gender("Male");
            FakeCity = new City { Value = "Toronto", Province = new Province { Country = new Country() } };
            FakeLift = new Lift("Lift");
            FakeLanguage = new Language();
            FakeAgencyUser = new User(CvnEmail.Create("FakeAgencyUser@email.com").Value);
            FakeAgency = new Covenant.Common.Entities.Agency.Agency { Id = FakeAgencyUser.Id, User = FakeAgencyUser };
            FakeWorkerProfileToFilter = new WorkerProfile(new User(CvnEmail.Create("some@e.com").Value))
            {
                AvailabilityTimes = new List<WorkerProfileAvailabilityTime> { new WorkerProfileAvailabilityTime { AvailabilityTime = new AvailabilityTime { Value = "Morning" } } },
                AvailabilityDays = new List<WorkerProfileAvailabilityDay> { new WorkerProfileAvailabilityDay { Day = new Day { Value = "Monday" } } },
                Location = new Location { City = new City { Province = new Province { Country = Country.Canada }, Value = "Toronto" } },
                ApprovedToWork = true,
                HavePoliceCheckBackground = true,
                IdentificationNumber1 = "987654321",
                IdentificationType1 = new IdentificationType { Value = "type1" },
                IdentificationType2 = new IdentificationType(),
                IdentificationType1File = new CovenantFile("type.pdf"),
                IdentificationType2File = new CovenantFile("type.pdf"),
                PoliceCheckBackGround = new CovenantFile("type.pdf"),
                Certificates = new List<WorkerProfileCertificate>(), //Requires SQL
                Skills = new List<WorkerProfileSkill>(), //Requires SQL
                Licenses = new List<WorkerProfileLicense>(), //Requires SQL
                Languages = new List<WorkerProfileLanguage>
                {
                    new WorkerProfileLanguage {Language = new Language {Value = "English"}},
                    new WorkerProfileLanguage {Language = new Language {Value = "Spanish"}}
                },
                Resume = new CovenantFile("resume.pdf"),
                JobExperiences = new List<WorkerProfileJobExperience>
                {
                    new WorkerProfileJobExperience {Duties = "Some duty one"},
                    new WorkerProfileJobExperience {Duties = "Some duty two"}
                },
                LocationPreferences = new List<WorkerProfileLocationPreference>
                {
                    new WorkerProfileLocationPreference {City = new City {Value = "Toronto"}}
                },
                Agency = FakeAgency,
                CreatedBy = FakeAgency.User.Email,
                PunchCardId = "SIGOOK-0987654321"
            };
            FakeWorkerToDelete = new WorkerProfile(new User(CvnEmail.Create("delete@e.com").Value), FakeAgency.Id)
            {
                Location = new Location { City = new City { Province = new Province { Country = new Country { Code = "USA" } } } },
            };
            FakeWorkerToDnu = new WorkerProfile(new User(CvnEmail.Create("dnu@e.com").Value), FakeAgency.Id)
            {
                Location = new Location { City = new City { Province = new Province { Country = new Country { Code = "USA" } } } }
            };
            FakeWorkerIsContractor = new WorkerProfile(new User(CvnEmail.Create("contractor@e.com").Value), FakeAgency.Id)
            {
                Location = new Location { City = new City { Province = new Province { Country = new Country { Code = "USA" } } } },
            };
            FakeWorkerIsSubcontractor = new WorkerProfile(new User(CvnEmail.Create("subcontractor@e.com").Value), FakeAgency.Id)
            {
                Location = new Location { City = new City { Province = new Province { Country = new Country { Code = "USA" } } } },
            };
            FakeWorkerToUpdateEmail = new WorkerProfile(new User(CvnEmail.Create("updateMyEmail@e.com").Value), FakeAgency.Id)
            {
                Location = new Location { City = new City { Province = new Province { Country = new Country { Code = "USA" } } } }
            };
            var basicInformation = new Mock<IWorkerBasicInformation<ICatalog<Guid>>>();
            basicInformation.SetupGet(i => i.FirstName).Returns("Juan");
            basicInformation.SetupGet(i => i.LastName).Returns("Perez");
            basicInformation.SetupGet(i => i.HasVehicle).Returns(true);
            basicInformation.SetupGet(i => i.Gender).Returns(FakeGender);
            FakeWorkerProfileToFilter.PatchOtherInformation(new WorkerProfileOtherInformationModel { Lift = new BaseModel<Guid>(FakeLift.Id) });
            FakeWorkerProfileToFilter.PatchBasicInformation(basicInformation.Object);
            FakeWorkerProfileToFilter.PatchAvailabilities(new[] { new BaseModel<Guid>(FakeAvailabilityPartTime.Id) });
            FakeWorkerProfileToFilter.PatchAvailabilities(new[] { new BaseModel<Guid>(FakeAvailabilityPartTime.Id) });
            FakeWorkerProfileToFilter.AddOtherDocuments(new List<CovenantFile> { new CovenantFile("contract.pdf", "contract doc") });
            _client = _factory.CreateClient();
            Seed(_factory.Services.GetService<CovenantContext>());
        }

        private string RequestUri() => AgencyWorkerProfileController.RouteName;

        [Fact]
        public async Task GetById()
        {
            HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/{FakeWorkerProfileToFilter.Id}");
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<WorkerProfileDetailModel>();
            WorkerProfile entity = FakeWorkerProfileToFilter;
            AssertEntityAndModel(entity, detail);
        }

        [Fact]
        public async Task GetOtherDocument()
        {
            var response = await _client.GetAsync($"{RequestUri()}/{FakeWorkerProfileToFilter.Id}/OtherDocument");
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<List<CovenantFileModel>>();
            Assert.NotEmpty(list);
            var entity = FakeWorkerProfileToFilter.OtherDocuments.Single();
            var model = list.Single(s => s.Id == entity.Id);
            Assert.Equal(entity.Id, model.Id);
            Assert.Equal(entity.Document.Description, model.Description);
            Assert.EndsWith(entity.Document.FileName, model.FileName);
            Assert.EndsWith(entity.Document.FileName, model.PathFile);
        }

        [Fact]
        public async Task UpdateApprovedToWork()
        {
            var worker = FakeWorkerToDelete;
            var requestUri = $"{RequestUri()}/{worker.Id}/ApprovedToWork";
            var response = await _client.PutAsync(requestUri, new StringContent(string.Empty));
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var context = _factory.Services.GetRequiredService<CovenantContext>();
            var sinInfo = new Mock<ISinInformation<CovenantFileModel>>();
            sinInfo.SetupGet(s => s.SocialInsurance).Returns("987-654-321");
            sinInfo.SetupGet(s => s.SocialInsuranceExpire).Returns(false);
            sinInfo.SetupGet(s => s.SocialInsuranceFile).Returns(new CovenantFileModel("sin.pdf", "sin"));
            worker.OnNewDocumentAdded += async (sender, args) => await context.CovenantFile.AddAsync(args);
            worker.PatchSinInformation(sinInfo.Object);
            worker.PatchProfileImage(new CovenantFile("worker.png", "profile"));

            response = await _client.PutAsync(requestUri, new StringContent(string.Empty));
            response.EnsureSuccessStatusCode();
            Assert.True((await context.WorkerProfile.SingleAsync(c => c.Id == worker.Id)).ApprovedToWork);
        }

        [Fact]
        public async Task PutDnu()
        {
            Guid id = FakeWorkerToDnu.Id;
            var response = await _client.PutAsJsonAsync($"{RequestUri()}/{id}/Dnu", new { });
            response.EnsureSuccessStatusCode();
            var context = _factory.Services.GetRequiredService<CovenantContext>();
            WorkerProfile entity = await context.WorkerProfile.SingleAsync(s => s.Id == id);
            Assert.True(entity.Dnu);
        }

        [Fact]
        public async Task PutIsContractor()
        {
            Guid id = FakeWorkerIsContractor.Id;
            var url = $"{RequestUri()}/{id}/{nameof(AgencyWorkerProfileController.IsContractor)}";
            var response = await _client.PutAsJsonAsync(url, new { });
            response.EnsureSuccessStatusCode();
            var context = _factory.Services.GetRequiredService<CovenantContext>();
            var entity = await context.WorkerProfile.SingleAsync(s => s.Id == id);
            Assert.True(entity.IsContractor);
            response = await _client.PutAsJsonAsync(url, new { });
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var sinInfo = new Mock<ISinInformation<CovenantFile>>();
            sinInfo.SetupGet(g => g.SocialInsurance).Returns("909-987-123");
            sinInfo.SetupGet(g => g.SocialInsuranceExpire).Returns(false);
            sinInfo.SetupGet(g => g.SocialInsuranceFile).Returns(CovenantFile.Create("sin.pdf").Value);
            entity.PatchSinInformation(sinInfo.Object);
            context.WorkerProfile.Update(entity);
            await context.SaveChangesAsync();

            response = await _client.PutAsJsonAsync(url, new { });
            response.EnsureSuccessStatusCode();
            entity = await context.WorkerProfile.SingleAsync(s => s.Id == id);
            Assert.False(entity.IsContractor);
        }

        [Fact]
        public async Task PutIsSubcontractor()
        {
            Guid id = FakeWorkerIsSubcontractor.Id;
            var url = $"{RequestUri()}/{id}/{nameof(AgencyWorkerProfileController.IsSubcontractor)}";
            var response = await _client.PutAsJsonAsync(url, new { });
            response.EnsureSuccessStatusCode();
            var context = _factory.Services.GetRequiredService<CovenantContext>();
            var entity = await context.WorkerProfile.SingleAsync(s => s.Id == id);
            Assert.True(entity.IsSubcontractor);
            response = await _client.PutAsJsonAsync(url, new { });
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var sinInfo = new Mock<ISinInformation<CovenantFile>>();
            sinInfo.SetupGet(g => g.SocialInsurance).Returns("909-987-876");
            sinInfo.SetupGet(g => g.SocialInsuranceExpire).Returns(false);
            sinInfo.SetupGet(g => g.SocialInsuranceFile).Returns(CovenantFile.Create("sin.pdf").Value);
            entity.PatchSinInformation(sinInfo.Object);
            context.WorkerProfile.Update(entity);
            await context.SaveChangesAsync();

            response = await _client.PutAsJsonAsync(url, new { });
            response.EnsureSuccessStatusCode();
            entity = await context.WorkerProfile.SingleAsync(s => s.Id == id);
            Assert.False(entity.IsSubcontractor);
        }

        private void AssertEntityAndModel(WorkerProfile entity, WorkerProfileDetailModel detail)
        {
            Assert.Equal(entity.FirstName, detail.FirstName);
            Assert.Equal(entity.MiddleName, detail.MiddleName);
            Assert.Equal(entity.LastName, detail.LastName);
            Assert.Equal(entity.SecondLastName, detail.SecondLastName);
            Assert.Equal(entity.Location.City.Id, detail.Location.City.Id);
            Assert.Equal(entity.Location.City.Province.Country.Id, detail.Location.City.Province.Country.Id);
            Assert.Equal(entity.Location.Address, detail.Location.Address);
            Assert.Equal(entity.Location.PostalCode, detail.Location.PostalCode);
            Assert.Equal(entity.BirthDay.Date, detail.BirthDay.Date);
            Assert.Equal(entity.Gender?.Id, detail.Gender?.Id);
            Assert.Equal(entity.Lift?.Id, detail.Lift?.Id);
            Assert.Equal(entity.SocialInsurance, detail.SocialInsurance);
            Assert.Equal(entity.SocialInsuranceExpire, detail.SocialInsuranceExpire);
            Assert.Equal(entity.DueDate, detail.DueDate);
            Assert.Equal(entity.SocialInsuranceFile?.FileName, detail.SocialInsuranceFile?.FileName);
            Assert.Equal(entity.IdentificationNumber1, detail.IdentificationNumber1);
            Assert.Equal(entity.IdentificationNumber2, detail.IdentificationNumber2);
            Assert.Equal(entity.HavePoliceCheckBackground, detail.HavePoliceCheckBackground);
            Assert.Equal(entity.PoliceCheckBackGround?.FileName, detail.PoliceCheckBackGround?.FileName);
            Assert.Equal(entity.IdentificationType1File?.FileName, detail.IdentificationType1File?.FileName);
            Assert.Equal(entity.IdentificationType2File?.FileName, detail.IdentificationType2File?.FileName);
            Assert.Equal(entity.IdentificationType1?.Id, detail.IdentificationType1?.Id);
            Assert.Equal(entity.IdentificationType2?.Id, detail.IdentificationType2?.Id);
            Assert.Equal(entity.MobileNumber, detail.MobileNumber);
            Assert.Equal(entity.Phone, detail.Phone);
            Assert.Equal(entity.PhoneExt, detail.PhoneExt);
            Assert.Equal(entity.HasVehicle, detail.HasVehicle);
            Assert.Equal(entity.Resume?.FileName, detail.Resume?.FileName);
            Assert.Equal(entity.HaveAnyHealthProblem, detail.HaveAnyHealthProblem);
            Assert.Equal(entity.HealthProblem, detail.HealthProblem);
            Assert.Equal(entity.OtherHealthProblem, detail.OtherHealthProblem);
            Assert.Equal(entity.ContactEmergencyName, detail.ContactEmergencyName);
            Assert.Equal(entity.ContactEmergencyLastName, detail.ContactEmergencyLastName);
            Assert.Equal(entity.ContactEmergencyPhone, detail.ContactEmergencyPhone);
            Assert.Equal(entity.ApprovedToWork, detail.ApprovedToWork);
            Assert.Equal(entity.IsContractor, detail.IsSubcontractor);
            Assert.Equal(entity.IsContractor, detail.IsContractor);
            Assert.Equal(entity.CreatedBy, detail.CreatedBy);
            Assert.Equal(entity.PunchCardId ?? entity.WorkerId.ToString(), detail.PunchCardId);
            AssertCollection(entity.Availabilities, detail.Availabilities, (a, b) => a.AvailabilityId == b.Id);
            AssertCollection(entity.Certificates, detail.Certificates, (a, b) => a.Certificate.FileName == b.FileName);
            AssertCollection(entity.Languages, detail.Languages, (a, b) => a.LanguageId == b.Id);
            AssertCollection(entity.Licenses, detail.Licenses, (a, b) =>
                a.Number == b.Number && a.Issued.GetValueOrDefault() == b.Issued.GetValueOrDefault() &&
                a.Expires.GetValueOrDefault() == b.Expires.GetValueOrDefault() && a.License.FileName == b.License.FileName);
            AssertCollection(entity.Skills, detail.Skills, (a, b) => a.Skill == b.Skill);
            AssertCollection(entity.AvailabilityDays, detail.AvailabilityDays, (a, b) => a.DayId == b.Id);
            AssertCollection(entity.AvailabilityTimes, detail.AvailabilityTimes, (a, b) => a.AvailabilityTimeId == b.Id);
            AssertCollection(entity.LocationPreferences, detail.LocationPreferences, (a, b) => a.CityId == b.Id);
        }

        private void AssertCollection<T, TF>(IEnumerable<T> expected, IEnumerable<TF> actual, Func<T, TF, bool> filter)
        {
            Assert.Equal(expected.Count(), actual.Count());
            foreach (T license in expected) Assert.Contains(actual, f => filter(license, f));
        }

        private void Seed(CovenantContext context)
        {
            context.Agencies.Add(FakeAgency);
            context.WorkerProfile.AddRange(FakeWorkerProfileToFilter, FakeWorkerToDelete, FakeWorkerToDnu, FakeWorkerIsContractor, FakeWorkerIsSubcontractor, FakeWorkerToUpdateEmail);
            context.NotificationType.Add(NotificationType.NewRequestNotifyWorker);
            context.UserNotificationType.Add(new UserNotificationType(FakeWorkerProfileToFilter.WorkerId, NotificationType.NewRequestNotifyWorker.Id) { EmailNotification = true });
            context.City.Add(FakeCity);
            context.Gender.Add(FakeGender);
            context.IdentificationType.AddRange(FakeIdentificationType1, FakeIdentificationType2);
            context.Availability.AddRange(FakeAvailabilityNew, FakeAvailabilityRemove, FakeAvailabilityDontChange, FakeAvailabilityPartTime);
            context.AvailabilityTime.Add(FakeAvailabilityTimeNew);
            context.Day.Add(FakeDayNew);
            context.Lift.Add(FakeLift);
            context.Language.Add(FakeLanguage);
            context.SaveChanges();
        }
    }
}