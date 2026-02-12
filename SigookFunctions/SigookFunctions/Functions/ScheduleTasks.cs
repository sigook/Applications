using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SigookFunctions.Models;
using SigookFunctions.Utils;
using System.Net.Http.Headers;

namespace SigookFunctions.Functions;

public class ScheduleTasks
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ScheduleTasks> _logger;

    public ScheduleTasks(IHttpClientFactory httpClientFactory, ILogger<ScheduleTasks> logger)
    {
        _httpClientFactory = httpClientFactory;
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
        _logger.LogInformation("Starting scheduled task: {Action}", action);
        TeamsMessage message;
        try
        {
            string baseApiUrl = Environment.GetEnvironmentVariable("ScheduleTasks_ApiUrl");
            if (string.IsNullOrEmpty(baseApiUrl))
            {
                _logger.LogError("ScheduleTasks_ApiUrl is not configured");
                message = TeamsMessage.CreateError("API url is missing", "ScheduleTasks_ApiUrl environment variable is not set");
            }
            else
            {
                var url = $"{baseApiUrl}{action}";
                var apiClient = _httpClientFactory.CreateClient("Api");
                var token = await apiClient.GetToken();

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                _logger.LogInformation("Calling API: {Url}", url);
                HttpResponseMessage response = await apiClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Scheduled task {Action} completed successfully", action);
                    message = TeamsMessage.CreateSuccess(url, "OK");
                }
                else
                {
                    string content = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Scheduled task {Action} failed with status {StatusCode}: {Content}", action, response.StatusCode, content);
                    message = TeamsMessage.CreateError(url, content);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Scheduled task {Action} threw an exception", action);
            message = TeamsMessage.CreateError(e.Message, e.ToString());
        }

        var teamsClient = _httpClientFactory.CreateClient("Teams");
        var notificationResult = await teamsClient.SendTeamsNotification(message);
        if (!string.IsNullOrEmpty(notificationResult))
        {
            _logger.LogWarning("Teams notification failed for {Action}: {Result}", action, notificationResult);
        }
    }
}
