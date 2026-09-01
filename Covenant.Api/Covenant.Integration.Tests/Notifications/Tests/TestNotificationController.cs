using Covenant.Common.Entities.Notification;
using Covenant.Common.Models.Notification;
using Covenant.Common.Repositories.Notification;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Notification;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Net.Http.Json;

using Covenant.Common.Entities;
namespace Covenant.Integration.Tests.Notifications.Tests
{
    public class TestNotificationController : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<TestNotificationController.Startup>>
    {
        private const string Url = "api/UserNotification";
        private readonly HttpClient _client;

        public TestNotificationController(CustomWebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

        [Fact]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync(Url);
            response.EnsureSuccessStatusCode();
            List<UserNotificationListModel> list = await response.Content.ReadFromJsonAsync<List<UserNotificationListModel>>();
            Assert.NotEmpty(list);
            Assert.All(list, m => Assert.False(m.EmailNotification));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Put(bool emailNotifications)
        {
            int id = NotificationType.NewRequestNotifyWorker.Id;
            HttpResponseMessage response = await _client.PutAsJsonAsync(Url, new UserNotificationUpdateModel { Id = id, EmailNotification = emailNotifications });
            response.EnsureSuccessStatusCode();
            List<UserNotificationListModel> list = await (await _client.GetAsync(Url)).Content.ReadFromJsonAsync<List<UserNotificationListModel>>();
            Assert.Equal(emailNotifications, list.First(m => m.Id == id).EmailNotification);
        }


        public class Startup
        {
            public static readonly User FakeUser = FakeData.FakeUser();

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o =>
                    {
                        o.AddSub(FakeUser.Id);
                        o.AddWorkerRole();
                        o.AddAgencyPersonnelRole();
                        o.AddCompanyRole();
                    });
                services.AddSingleton<INotificationRepository, NotificationRepository>();
                services.AddTestDatabase();
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
                context.Users.Add(FakeUser);
                context.NotificationTypes.AddRange(NotificationType.GetAll);
                context.SaveChanges();
            }
        }
    }
}
