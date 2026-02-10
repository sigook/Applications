using IdentityModel.Client;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SigookFunctions.Models;
using SigookFunctions.Utils;

namespace SigookFunctions.Functions
{
    public class ScheduleTasks
    {
        private static readonly HttpClient Client = new HttpClient();
        private readonly ILogger<ScheduleTasks> _logger;

        public ScheduleTasks(ILogger<ScheduleTasks> logger)
        {
            _logger = logger;
        }

        [Function(nameof(NotificationSinExpiration))]
        public async Task NotificationSinExpiration(
            [TimerTrigger("0 0 * * 1-5")] TimerInfo timerInfo) => await Execute(nameof(NotificationSinExpiration));

        [Function(nameof(WarnLicensesExpiration))]
        public async Task WarnLicensesExpiration(
            [TimerTrigger("0 0 * * 1-5")] TimerInfo timerInfo) => await Execute(nameof(WarnLicensesExpiration));

        private async Task Execute(string action)
        {
            TeamsMessage message;
            try
            {
                string baseApiUrl = Environment.GetEnvironmentVariable("ScheduleTasks_ApiUrl");
                if (string.IsNullOrEmpty(baseApiUrl))
                {
                    message = TeamsMessage.CreateError("API url is missing", "API url is missing");
                }
                else
                {
                    var url = $"{baseApiUrl}{action}";
                    Client.SetBearerToken(await Client.GetToken());
                    HttpResponseMessage response = await Client.PostAsync(url, new StringContent(string.Empty));

                    if (response.IsSuccessStatusCode) message = TeamsMessage.CreateSuccess(url, "OK");
                    else
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        _logger.LogError("Error: {Content}", content);
                        message = TeamsMessage.CreateError(url, content);
                    }
                }
            }
            catch (Exception e)
            {
                message = TeamsMessage.CreateError(e.Message, e.ToString());
            }

            await Client.SendTeamsNotification(message);
        }
    }
}
