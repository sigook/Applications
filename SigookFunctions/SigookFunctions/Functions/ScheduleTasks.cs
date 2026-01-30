using IdentityModel.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SigookFunctions.Models;
using SigookFunctions.Options;
using SigookFunctions.Services;

namespace SigookFunctions.Functions
{
    public class ScheduleTasks
    {
        private readonly SigookFunctionsOptions _options;
        private readonly ITokenService _tokenService;
        private readonly INotificationService _notificationService;
        private readonly HttpClient _client;

        public ScheduleTasks(
            IOptions<SigookFunctionsOptions> options,
            ITokenService tokenService,
            INotificationService notificationService)
        {
            _options = options.Value;
            _tokenService = tokenService;
            _notificationService = notificationService;
            _client = new HttpClient();
        }

        [FunctionName(nameof(NotificationSinExpiration))]
        public async Task NotificationSinExpiration(
            [TimerTrigger("0 0 * * 1-5")] TimerInfo timerInfo,
            ILogger logger) => await Execute(nameof(NotificationSinExpiration), logger);

        [FunctionName(nameof(WarnLicensesExpiration))]
        public async Task WarnLicensesExpiration(
            [TimerTrigger("0 0 * * 1-5")] TimerInfo timerInfo,
            ILogger logger) => await Execute(nameof(WarnLicensesExpiration), logger);

        [FunctionName(nameof(StartRequests))]
        public async Task StartRequests(
            [TimerTrigger("0 */20 * * * *")] TimerInfo timerInfo,
            ILogger logger) => await Execute(nameof(StartRequests), logger);

        private async Task Execute(string action, ILogger logger)
        {
            TeamsMessage message;
            try
            {
                string baseApiUrl = _options.Urls.ScheduleTasksApiUrl;
                if (string.IsNullOrEmpty(baseApiUrl))
                {
                    message = TeamsMessage.CreateError("API url is missing", "API url is missing");
                }
                else
                {
                    var url = $"{baseApiUrl}{action}";
                    _client.SetBearerToken(await _tokenService.GetTokenAsync());
                    HttpResponseMessage response = await _client.PostAsync(url, new StringContent(string.Empty));

                    if (response.IsSuccessStatusCode)
                        message = TeamsMessage.CreateSuccess(url, "OK");
                    else
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        logger.LogError("Error: {Content}", content);
                        message = TeamsMessage.CreateError(url, content);
                    }
                }
            }
            catch (Exception e)
            {
                message = TeamsMessage.CreateError(e.Message, e.ToString());
            }

            await _notificationService.SendTeamsNotificationAsync(message);
        }
    }
}
