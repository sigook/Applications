using Covenant.Api.AgencyModule.AgencyRequest.Controllers;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Agency;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories;
using Covenant.Infrastructure.Repositories.Company;
using Covenant.Infrastructure.Repositories.Request;
using Covenant.Infrastructure.Repositories.Worker;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.AgencyRequest
{
    public class AgencyRequestControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<AgencyRequestControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public AgencyRequestControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => AgencyRequestController.RouteName;

        [Fact]
        public async Task Get()
        {
            Request entity = Data.FakeRequest;
            HttpResponseMessage response = await _client.GetAsync(RequestUri());
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<PaginatedList<AgencyRequestListModel>>();
            AgencyRequestListModel model = list.Items.Single(c => c.Id == entity.Id);
            AssertListModelEntity(model, entity);
        }

        private static void AssertListModelEntity(AgencyRequestListModel model, Request entity)
        {
            Assert.Equal(model.Id, entity.Id);
            Assert.Equal(model.NumberId, entity.NumberId);
            Assert.Equal(model.JobTitle, entity.JobTitle);
            Assert.Equal(model.CreatedAt, entity.CreatedAt);
            Assert.Equal(model.UpdatedAt, entity.UpdatedAt);
            Assert.Equal(model.FinishAt, entity.FinishAt);
            Assert.Equal(model.StartAt, entity.StartAt);
            Assert.Equal(model.Location, entity.JobLocation.FormattedAddress);
            Assert.Equal(model.Entrance, entity.JobLocation.Entrance);
            Assert.Equal(model.CompanyFullName, Data.FakeCompany.FullName);
            Assert.Equal(model.Status, entity.Status.ToString());
            Assert.Equal(model.DurationTerm, entity.DurationTerm.ToString());
            Assert.Equal(model.WorkersQuantity, entity.WorkersQuantity);
            Assert.Equal(model.WorkersQuantityWorking, entity.WorkersQuantityWorking);
            Assert.Equal(model.IsAsap, entity.IsAsap);
            Assert.Equal(model.WorkerRate, entity.WorkerRate ?? entity.WorkerSalary);
            Assert.Equal(model.DisplayRecruiters, entity.DisplayRecruiters);
            Assert.Equal(model.DisplayShift, entity.Shift?.DisplayShift);
        }

        [Fact]
        public async Task GetBoard()
        {
            HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/Board");
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<PaginatedList<WorkerRequestAgencyBoardModel>>();
            Assert.NotEmpty(list.Items);
            Request entity = Data.FakeRequest;
            var workerRequest = entity.Workers.First();
            WorkerProfile workerProfile = Data.FakeWorkerProfile;
            CompanyProfile companyProfile = Data.FakeCompany;
            WorkerRequestAgencyBoardModel model = list.Items.Single(s => s.Id == workerRequest.Id);
            Assert.Equal(model.Id, workerRequest.Id);
            Assert.Equal(model.StartWorking, workerRequest.StartWorking);
            Assert.Equal(model.WeekStartWorking, workerRequest.WeekStartWorking);
            Assert.Equal(model.WorkerRequestStatus, workerRequest.WorkerRequestStatus.ToString());
            Assert.Equal(model.RejectComments, workerRequest.RejectComments);
            Assert.Equal(model.RequestId, entity.Id);
            Assert.Equal(model.NumberId, entity.NumberId);
            Assert.Equal(model.RequestStatus, entity.Status.ToString());
            Assert.Equal(model.JobTitle, entity.JobTitle);
            Assert.Equal(model.WorkerRate, entity.WorkerRate);
            Assert.Equal(model.DurationTerm, entity.DurationTerm.ToString());
            Assert.Equal(model.DisplayRecruiters, entity.DisplayRecruiters);
            Assert.Equal(model.Entrance, entity.JobLocation.Entrance);
            Assert.Equal(model.Location, entity.JobLocation.FormattedAddress);
            Assert.Equal(model.DisplayShift, entity.Shift.DisplayShift);
            Assert.Equal(model.WorkerProfileId, workerProfile.Id);
            Assert.Equal(model.WorkerId, workerProfile.WorkerId);
            Assert.Equal(model.FirstName, workerProfile.FirstName);
            Assert.Equal(model.MiddleName, workerProfile.MiddleName);
            Assert.Equal(model.LastName, workerProfile.LastName);
            Assert.Equal(model.SecondLastName, workerProfile.SecondLastName);
            Assert.Equal(model.SocialInsurance, workerProfile.MaskedSocialInsurance);
            Assert.Equal(model.SocialInsuranceExpire, workerProfile.SocialInsuranceExpire);
            Assert.Equal(model.DueDate, workerProfile.DueDate);
            Assert.Equal(model.MobileNumber, workerProfile.MobileNumber);
            Assert.Equal(model.IsSubcontractor, workerProfile.IsSubcontractor);
            Assert.Equal(model.CompanyProfileId, companyProfile.Id);
            Assert.Equal(model.CompanyFullName, companyProfile.FullName);
        }

        [Fact]
        public async Task GetById()
        {
            HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/{Data.FakeRequest.Id}");
            response.EnsureSuccessStatusCode();
            AgencyRequestDetailModel model = await response.Content.ReadAsJsonAsync<AgencyRequestDetailModel>();
            Assert.Equal(model.Id, Data.FakeRequest.Id);
            Assert.Equal(model.JobTitle, Data.FakeRequest.JobTitle);
            Assert.EndsWith(Data.FakeCompany.Logo.FileName, model.CompanyLogo);
            Assert.Equal(model.FullName, Data.FakeCompany.FullName);
            Assert.Equal(model.CompanyProfileId, Data.FakeCompany.Id);
            Assert.Equal(model.Description, Data.FakeRequest.Description);
            Assert.Equal(model.Requirements, Data.FakeRequest.Requirements);
            Assert.Equal(model.JobLocation.FormattedAddress, Data.FakeRequest.JobLocation.FormattedAddress);
            Assert.Equal(model.JobLocation.Entrance, Data.FakeRequest.JobLocation.Entrance);
            Assert.Equal(model.JobLocation.MainIntersection, Data.FakeRequest.JobLocation.MainIntersection);
            Assert.Equal(model.WorkersQuantity, Data.FakeRequest.WorkersQuantity);
            Assert.Equal(model.WorkersQuantityWorking, Data.FakeRequest.WorkersQuantityWorking);
            Assert.Equal(model.JobPosition, Data.FakeRequest.JobPositionRate.JobPosition?.Value ?? Data.FakeRequest.JobPositionRate.OtherJobPosition);
            Assert.Equal(model.HolidayIsPaid, Data.FakeRequest.HolidayIsPaid);
            Assert.Equal(model.BreakIsPaid, Data.FakeRequest.BreakIsPaid);
            Assert.Equal(model.Status, Data.FakeRequest.Status.ToString());
            Assert.Equal(model.CreatedAt, Data.FakeRequest.CreatedAt);
            Assert.Equal(model.CreatedBy, Data.FakeRequest.CreatedBy);
            Assert.Equal(model.FinishAt, Data.FakeRequest.FinishAt);
            Assert.Equal(model.StartAt, Data.FakeRequest.StartAt);
            Assert.Equal(model.InvitationSentItAt, Data.FakeRequest.InvitationSentItAt);
            Assert.Equal(model.DurationBreak, Data.FakeRequest.DurationBreak);
            Assert.Equal(model.Incentive, Data.FakeRequest.Incentive);
            Assert.Equal(model.IncentiveDescription, Data.FakeRequest.IncentiveDescription);
            Assert.Equal(model.AgencyRate, Data.FakeRequest.AgencyRate);
            Assert.Equal(model.WorkerRate, Data.FakeRequest.WorkerRate);
            Assert.Equal(model.DurationTerm, Data.FakeRequest.DurationTerm.ToString());
            Assert.Equal(model.DisplayShift, Data.FakeRequest.Shift.DisplayShift);
            Assert.Equal(model.VaccinationRequired, Data.FakeCompany.VaccinationRequired);
        }

        [Fact]
        public async Task PostRequest()
        {
            var model = new RequestCreateModel
            {
                JobTitle = "General Labour",
                WorkersQuantity = 5,
                Description = "It is a long established fact that a reader will be distracted by the readable content of a page when looking at its layout.",
                DurationBreak = TimeSpan.FromMinutes(15),
                BreakIsPaid = true,
                Incentive = 10,
                IncentiveDescription = "Incentive",
                Requirements = "There are many variations of passages of Lorem Ipsum available, but the majority have suffered alteration in some form, by injected humour, or randomised words which don't look even slightly believable.",
                IsAsap = true,
                JobIsOnBranchOffice = true,
                LocationId = Data.FakeLocation.Id,
                JobPositionRateId = Data.FakeCompanyProfileJobPositionRate.Id,
                CompanyProfileId = Data.FakeCompany.Id,
                DurationTerm = DurationTerm.ShortTerm,
                StartAt = new DateTime(2019, 01, 01),
                FinishAt = new DateTime(2020, 02, 03)
            };
            var response = await HttpClientJsonExtensions.PostAsJsonAsync(_client, RequestUri(), model);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<AgencyRequestDetailModel>();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.Request.SingleAsync(c => c.Id == detail.Id);
            AssertModelAndEntity(model, entity);
            Assert.Equal(model.StartAt, entity.StartAt);
            Assert.Equal(model.FinishAt, entity.FinishAt);
        }

        [Fact]
        public async Task PostWithoutWorkShift()
        {
            var model = new RequestCreateModel
            {
                JobTitle = "Forklift",
                WorkersQuantity = 5,
                Description = new string('D', 50),
                Requirements = new string('R', 50),
                LocationId = Data.FakeLocation.Id,
                JobPositionRateId = Data.FakeCompanyProfileJobPositionRate.Id,
                CompanyProfileId = Data.FakeCompany.Id,
                DurationTerm = DurationTerm.ShortTerm,
                StartAt = Data.FakeNow,
                FinishAt = Data.FakeNow.AddDays(7),
                Shift = new ShiftModel
                {
                    Monday = true,
                    MondayStart = TimeSpan.Parse("01:00"),
                    MondayFinish = TimeSpan.Parse("02:00"),
                    Tuesday = true,
                    TuesdayStart = TimeSpan.Parse("03:00"),
                    TuesdayFinish = TimeSpan.Parse("04:00"),
                    Wednesday = true,
                    WednesdayStart = TimeSpan.Parse("05:00"),
                    WednesdayFinish = TimeSpan.Parse("06:00"),
                    Thursday = true,
                    ThursdayStart = TimeSpan.Parse("07:00"),
                    ThursdayFinish = TimeSpan.Parse("08:00"),
                    Friday = true,
                    FridayStart = TimeSpan.Parse("09:00"),
                    FridayFinish = TimeSpan.Parse("10:00"),
                    Saturday = true,
                    SaturdayStart = TimeSpan.Parse("11:00"),
                    SaturdayFinish = TimeSpan.Parse("12:00"),
                    Sunday = true,
                    SundayStart = TimeSpan.Parse("13:00"),
                    SundayFinish = TimeSpan.Parse("14:00"),
                    Comments = new string('C', 10)
                }
            };
            var response = await HttpClientJsonExtensions.PostAsJsonAsync(_client, RequestUri(), model);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<AgencyRequestDetailModel>();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.Request.SingleAsync(c => c.Id == detail.Id);
            AssertModelAndEntity(model, entity);
            Assert.Equal(model.StartAt, entity.StartAt);
            Assert.Equal(model.FinishAt, entity.FinishAt);
        }

        [Fact]
        public async Task PutRequest()
        {
            var request = Data.FakeUpdateRequest;
            var updateUrl = $"{RequestUri()}/{request.Id}";
            var model = new RequestCreateModel
            {
                JobTitle = "General Labour - 2",
                Description = @"Vivamus tempus turpis ligula, eget auctor lectus ultrices ac. Sed sit amet imperdiet tellus. Pellentesque venenatis felis id facilisis ultrices. Curabitur tristique varius ligula vitae tempor. Sed mi dolor, tincidunt sit amet erat eu, ultrices lobortis dolor.",
                Requirements = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit.
                         Nunc et nulla sit amet lectus sagittis mattis vitae ut ante. Cras augue magna, vulputate quis erat quis, eleifend cursus metus. Aliquam sed scelerisque est. Aenean eu tellus metus. Maecenas eget maximus magna. Proin at urna vel sem tincidunt ultrices. Quisque interdum lorem ac accumsan vulputate. In eu imperdiet ipsum. Aenean et nibh vitae magna maximus ultricies sollicitudin et nulla. In condimentum lobortis risus vitae feugiat. In sollicitudin mollis felis id rhoncus. Lorem ipsum dolor sit amet, 
                         consectetur adipiscing elit. Mauris consequat eros id neque efficitur hendrerit.",
                LocationId = Data.FakeLocation.Id,
                WorkerSalary = 50_000m
            };
            HttpResponseMessage response = await _client.PutAsJsonAsync(updateUrl, model);
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.Request.SingleAsync(s => s.Id == request.Id);
            Assert.Equal(model.JobTitle, entity.JobTitle);
            Assert.Equal(model.Description, entity.Description);
            Assert.Equal(model.Requirements, entity.Requirements);
            Assert.Equal(Data.FakeLocation.FormattedAddress, entity.JobLocation.FormattedAddress);
            Assert.Equal(model.WorkerSalary, entity.WorkerSalary);
            Assert.NotNull(entity.UpdatedAt);
        }

        [Fact]
        public async Task PutIsAsap()
        {
            Request request = Data.FakeIsAsapRequest;
            var updateUrl = $"{RequestUri()}/{request.Id}/IsAsap";
            HttpResponseMessage response = await _client.PutAsJsonAsync(updateUrl, new { });
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            Assert.True((await context.Request.SingleAsync(s => s.Id == request.Id)).IsAsap);

            response = await _client.PutAsJsonAsync(updateUrl, new { });
            response.EnsureSuccessStatusCode();
            Assert.False((await context.Request.SingleAsync(s => s.Id == request.Id)).IsAsap);
        }

        [Fact]
        public async Task PutIncreaseWorkersQuantityByOne()
        {
            Request request = Data.FakeRequestIncreaseQuantity;
            int expected = request.WorkersQuantity + 1;
            var updateUrl = $"{RequestUri()}/{request.Id}/IncreaseWorkersQuantityByOne";
            HttpResponseMessage response = await _client.PutAsJsonAsync(updateUrl, new { });
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            Assert.Equal(expected, (await context.Request.SingleAsync(s => s.Id == request.Id)).WorkersQuantity);
        }

        [Fact]
        public async Task PutReduceWorkersQuantityByOne()
        {
            Request request = Data.FakeRequestReduceQuantity;
            int expected = request.WorkersQuantity - 1;
            var updateUrl = $"{RequestUri()}/{request.Id}/ReduceWorkersQuantityByOne";
            HttpResponseMessage response = await _client.PutAsJsonAsync(updateUrl, new { });
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            Assert.Equal(expected, (await context.Request.SingleAsync(s => s.Id == request.Id)).WorkersQuantity);
            response = await _client.PutAsJsonAsync(updateUrl, new { });
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutCancel()
        {
            Request request = Data.FakeRequest;
            var updateUrl = $"{RequestUri()}/{request.Id}";
            var model = new RequestCancellationDetailModel { OtherCancellationReason = "Contact finished" };
            HttpResponseMessage response = await _client.PutAsJsonAsync($"{updateUrl}/Cancel", model);
            response.EnsureSuccessStatusCode();
            Assert.Equal(model.OtherCancellationReason, (await (await _client.GetAsync(updateUrl)).Content.ReadAsJsonAsync<AgencyRequestDetailModel>()).CancellationDetail);
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            Assert.Equal(RequestStatus.Cancelled, (await context.Request.SingleAsync(r => r.Id == request.Id)).Status);
            var detail = await context.RequestCancellationDetail.SingleAsync(c => c.RequestId == request.Id);
            Assert.Equal(model.OtherCancellationReason, detail.OtherReasonCancellationRequest);
            Assert.NotNull(detail.CancelBy);
            Assert.NotNull(detail.CancelAt);
        }

        [Fact]
        public async Task PutOpen()
        {
            Request request = Data.FakeRequestToOpen;
            HttpResponseMessage response = await _client.PutAsJsonAsync($"{RequestUri()}/{request.Id}/Open", new { });
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            Assert.Equal(RequestStatus.InProcess, (await context.Request.SingleAsync(r => r.Id == request.Id)).Status);
            Assert.True(await context.RequestNotes.AnyAsync(a => a.RequestId == request.Id));
        }

        [Fact]
        public async Task SendInvitation()
        {
            var fakeNow = new DateTime(2019, 01, 01);
            var timeService = new Mock<ITimeService>();
            timeService.Setup(s => s.GetCurrentDateTime()).Returns(fakeNow);
            HttpClient client = _factory.WithWebHostBuilder(b =>
            {
                b.ConfigureTestServices(s =>
                {
                    var requestService = new Mock<IRequestService>();
                    requestService.Setup(rs => rs.SendInvitationToApply(It.IsAny<InvitationToApplyModel>())).Returns(Task.CompletedTask);
                    s.AddSingleton(timeService.Object);
                    s.AddSingleton(requestService.Object);
                });
            }).CreateClient();
            var url = $"{RequestUri()}/{Data.FakeRequestToSendInvitation.Id}/{nameof(AgencyRequestController.SendInvitation)}";
            HttpResponseMessage response = await client.PostAsJsonAsync(url, new { });
            response.EnsureSuccessStatusCode();

            response = await client.PostAsJsonAsync(url, new { });
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            string error = await response.Content.ReadAsStringAsync();
            Assert.Contains("The invitation must be sent it only once every 7 days.", error);

            timeService.Setup(s => s.GetCurrentDateTime()).Returns(fakeNow.AddDays(Request.DaysToWaitToResendInvitation));
            response = await client.PostAsJsonAsync(url, new { });
            response.EnsureSuccessStatusCode();
        }

        private static void AssertModelAndEntity(RequestCreateModel model, Request entity)
        {
            Assert.Equal(model.JobTitle, entity.JobTitle);
            Assert.Equal(model.WorkersQuantity, entity.WorkersQuantity);
            Assert.Equal(model.Description, entity.Description);
            Assert.Equal(model.DurationBreak, entity.DurationBreak);
            Assert.Equal(model.BreakIsPaid, entity.BreakIsPaid);
            Assert.Equal(model.Incentive, entity.Incentive);
            Assert.Equal(model.IncentiveDescription, entity.IncentiveDescription);
            Assert.Equal(model.Requirements, entity.Requirements);
            Assert.Equal(model.IsAsap, entity.IsAsap);
            Assert.Equal(model.JobIsOnBranchOffice, entity.JobIsOnBranchOffice);
            Assert.Equal(model.LocationId, entity.JobLocationId);
            Assert.Equal(model.DurationTerm, entity.DurationTerm);
            Assert.Equal(Data.FakeUserRecruiter.Email, entity.CreatedBy);
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
            Assert.Equal(model.Shift?.Sunday, entity.Shift?.Monday);
            Assert.Equal(model.Shift?.SundayStart, entity.Shift?.SundayStart);
            Assert.Equal(model.Shift?.SundayFinish, entity.Shift?.SundayFinish);
            Assert.Equal(model.Shift?.Comments, entity.Shift?.Comments);
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o =>
                    {
                        o.AddSub(Data.AgencyId);
                        o.AddAgencyPersonnelRole(Data.AgencyId);
                        o.AddName(Data.FakeUserRecruiter.Email);
                    });
                services.AddHttpClient();
                services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<IRequestRepository, RequestRepository>();
                services.AddSingleton<IRequestService, RequestService>();
                services.AddSingleton<IWorkerRequestRepository, WorkerRequestRepository>();
                services.AddSingleton<ICompanyRepository, CompanyRepository>();
                services.AddSingleton<IWorkerRepository, WorkerRepository>();
                services.AddSingleton<IIdentityServerService, IdentityServerService>();
                services.AddSingleton<ILocationRepository, LocationRepository>();
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
                context.Agencies.Add(Data.FakeAgency);
                context.User.Add(Data.FakeUserRecruiter);
                context.AgencyPersonnel.Add(Data.FakeRecruiter);
                context.AddRange(Data.FakeCompany, Data.FakeRequest,
                    Data.FakeUpdateRequest, Data.FakeRequestIncreaseQuantity,
                    Data.FakeRequestReduceQuantity, Data.FakeRequestFinalize,
                    Data.FakeIsAsapRequest, Data.FakeRequestUpdateLocation, Data.FakeRequestToOpen,
                    Data.FakeRequestToSendInvitation, Data.FakeWorker, Data.FakeWorkerProfile, Data.FakeCompanyProfileJobPositionRate);
                context.SaveChanges();
            }
        }

        private static class Data
        {
            public static readonly DateTime FakeNow = DateTime.Now;
            public static readonly Agency FakeAgency = new Agency("Test", "Test");
            public static readonly Guid AgencyId = FakeAgency.Id;
            public static readonly City Toronto = new City { Value = "Toronto", Province = new Province { Country = Country.Canada } };

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
            public static readonly Location FakeLocation = new Location
            {
                Address = "11 Main Street",
                Entrance = "Main Door",
                MainIntersection = "Main & Street",
                City = new City { Province = new Province { Country = Country.Canada } }
            };
            public static readonly CompanyProfileJobPositionRate FakeCompanyProfileJobPositionRate = CompanyProfileJobPositionRate.Create(FakeCompany.Id, "Forklift", 20, 15).Value;

            public static readonly Request FakeRequest;
            public static readonly Request FakeUpdateRequest;
            public static readonly Request FakeRequestIncreaseQuantity;
            public static readonly Request FakeRequestReduceQuantity;
            public static readonly Request FakeRequestFinalize;
            public static readonly Request FakeIsAsapRequest;
            public static readonly Request FakeRequestUpdateLocation;
            public static readonly Request FakeRequestToOpen;
            public static readonly Request FakeRequestToSendInvitation;

            public static readonly User FakeWorker = new User(CvnEmail.Create("w1@mai.com").Value);
            public static readonly WorkerProfile FakeWorkerProfile = new WorkerProfile(FakeWorker, AgencyId);

            public static readonly User FakeUserRecruiter = new User(CvnEmail.Create("re@rec.com").Value);
            public static readonly AgencyPersonnel FakeRecruiter = AgencyPersonnel.CreatePrimary(AgencyId, FakeUserRecruiter.Id, FakeUserRecruiter.Email);

            static Data()
            {
                FakeCompany.AddJobPositionRate(FakeCompanyProfileJobPositionRate);
                FakeCompany.UpdateVaccinationInfo(true, "require vaccine certificate");
                FakeRequest = FakeData.FakeRequest(AgencyId, FakeCompany.Company.Id, FakeCompany.JobPositionRates.First().Id, FakeLocation);
                FakeRequest.NumberId = 99;
                FakeRequest.CreatedAt = new DateTime(2021, 01, 01);
                FakeRequest.CreatedBy = "recruiter@mail.com";
                FakeRequest.InvitationSentItAt = new DateTime(2021, 01, 01);
                FakeRequest.WorkerSalary = 50_000m;
                FakeRequest.UpdateIsAsap(true);
                FakeRequest.AddRecruiter(FakeRecruiter);
                FakeRequest.AddWorker(FakeWorker.Id, FakeRequest.CreatedAt.AddDays(1));
                var newShift = new Shift();
                newShift.AddMonday(TimeSpan.Parse("08:00"), TimeSpan.Parse("16:00"));
                FakeRequest.UpdateShift(newShift);

                FakeUpdateRequest = FakeData.FakeRequest(AgencyId, FakeCompany.Company.Id, FakeCompany.JobPositionRates.First().Id);
                FakeIsAsapRequest = FakeData.FakeRequest(AgencyId, FakeCompany.Company.Id, FakeCompany.JobPositionRates.First().Id);
                FakeRequestIncreaseQuantity = FakeData.FakeRequest(AgencyId, FakeCompany.Company.Id, FakeCompany.JobPositionRates.First().Id, workersQuantity: 2);
                FakeRequestReduceQuantity = FakeData.FakeRequest(AgencyId, FakeCompany.Company.Id, FakeCompany.JobPositionRates.First().Id, workersQuantity: 2);
                FakeRequestFinalize = FakeData.FakeRequest(AgencyId, FakeCompany.Company.Id, FakeCompany.JobPositionRates.First().Id, workersQuantity: 2);
                FakeRequestUpdateLocation = FakeData.FakeRequest(AgencyId, FakeCompany.Company.Id, FakeCompany.JobPositionRates.First().Id, workersQuantity: 2);
                FakeRequestToOpen = FakeData.FakeRequest(AgencyId, FakeCompany.Company.Id, FakeCompany.JobPositionRates.First().Id, workersQuantity: 2);
                FakeRequestToOpen.Cancel(FakeNow);
                FakeRequestToSendInvitation = FakeData.FakeRequest(AgencyId, FakeCompany.Company.Id, FakeCompany.JobPositionRates.First().Id, workersQuantity: 2);
                FakeRequestToSendInvitation.WorkerSalary = 50_000m;
                var sinInfo = new Mock<ISinInformation<CovenantFile>>();
                sinInfo.SetupGet(g => g.SocialInsurance).Returns("989-987-678");
                sinInfo.SetupGet(g => g.SocialInsuranceExpire).Returns(true);
                sinInfo.SetupGet(g => g.DueDate).Returns(new DateTime(2019, 01, 01));
                FakeWorkerProfile.PatchSinInformation(sinInfo.Object);
            }
        }
    }
}