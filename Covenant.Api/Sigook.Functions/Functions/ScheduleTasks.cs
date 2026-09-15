using Covenant.Common.Models.Notification;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sigook.Functions.Configuration;
using Sigook.Functions.Services;
using Sigook.Functions.Utils;
using System.Net.Http.Headers;

namespace Sigook.Functions.Functions;

public class ScheduleTasks(
    IHttpClientFactory httpClientFactory,
    IAccessTokenProvider accessTokenProvider,
    ILogger<ScheduleTasks> logger,
    IConfiguration configuration,
    IOptions<ScheduleTasksOptions> options)
{
    private readonly ScheduleTasksOptions _options = options.Value;

    [Function(nameof(NotificationSinExpiration))]
    public async Task NotificationSinExpiration(
        [TimerTrigger("0 0 0 * * 1-5")] TimerInfo timerInfo) => await Execute(nameof(NotificationSinExpiration));

    [Function(nameof(WarnLicensesExpiration))]
    public async Task WarnLicensesExpiration(
        [TimerTrigger("0 0 0 * * 1-5")] TimerInfo timerInfo) => await Execute(nameof(WarnLicensesExpiration));

    private async Task Execute(string action)
    {
        logger.LogInformation("Starting scheduled task: {Action}", action);
        TeamsNotificationModel message;
        try
        {
            if (string.IsNullOrEmpty(_options.ApiUrl))
            {
                logger.LogError("ScheduleTasks:ApiUrl is not configured");
                message = TeamsNotificationModel.CreateError("API url is missing", "ScheduleTasks:ApiUrl is not set");
            }
            else
            {
                var url = $"{_options.ApiUrl}{action}";
                var apiClient = httpClientFactory.CreateClient(HttpClients.Api);
                var token = await accessTokenProvider.GetToken();

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                logger.LogInformation("Calling API: {Url}", url);
                HttpResponseMessage response = await apiClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation("Scheduled task {Action} completed successfully", action);
                    message = TeamsNotificationModel.CreateSuccess(url, "OK");
                }
                else
                {
                    string content = await response.Content.ReadAsStringAsync();
                    logger.LogError("Scheduled task {Action} failed with status {StatusCode}: {Content}", action, response.StatusCode, content);
                    message = TeamsNotificationModel.CreateError(url, content);
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Scheduled task {Action} threw an exception", action);
            message = TeamsNotificationModel.CreateError(e.Message, e.ToString());
        }

        var teamsClient = httpClientFactory.CreateClient(HttpClients.Teams);
        var notificationResult = await teamsClient.SendTeamsNotification(message, configuration);
        if (!string.IsNullOrEmpty(notificationResult))
        {
            logger.LogWarning("Teams notification failed for {Action}: {Result}", action, notificationResult);
        }
    }
}
