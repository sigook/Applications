using System.Net.Http.Headers;
using System.Net.Http.Json;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sigook.Functions.Configuration;
using Sigook.Functions.Models;
using Sigook.Functions.Utils;

namespace Sigook.Functions.Functions;

public class CraTables(
    IHttpClientFactory httpClientFactory,
    ILogger<CraTables> logger,
    IConfiguration configuration,
    IOptions<CraTablesOptions> options,
    IOptions<ScheduleTasksOptions> apiCredentials)
{
    private readonly CraTablesOptions _options = options.Value;
    private readonly ScheduleTasksOptions _apiCredentials = apiCredentials.Value;

    [Function(nameof(CraTableUploaded))]
    public async Task CraTableUploaded(
        [BlobTrigger($"{CraTablesOptions.ContainerName}/{{name}}", Connection = CraTablesOptions.StorageConnectionName)] BlobClient blob,
        string name)
    {
        logger.LogInformation("A CRA table was uploaded: {Name}", name);

        if (!CraBlobName.TryParse(name, out var model, out var nameError))
        {
            logger.LogError("The CRA table {Name} was ignored: {Error}", name, nameError);
            await Notify(TeamsMessage.CreateError($"CRA table ignored: {name}", nameError));
            return;
        }

        TeamsMessage message;
        try
        {
            if (string.IsNullOrEmpty(_options.ApiUrl))
            {
                logger.LogError("CraTables:ApiUrl is not configured");
                message = TeamsMessage.CreateError("API url is missing", "CraTables:ApiUrl is not set");
            }
            else
            {
                var apiClient = httpClientFactory.CreateClient("Api");
                var token = await apiClient.GetToken(_apiCredentials);

                var request = new HttpRequestMessage(HttpMethod.Post, _options.ApiUrl)
                {
                    Content = JsonContent.Create(model)
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                logger.LogInformation("Importing the {PayPeriod} {Year} CPP table from {Name}", model.PayPeriod, model.Year, name);
                HttpResponseMessage response = await apiClient.SendAsync(request);
                string content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation("The CRA table {Name} was imported with {Rows} rows", name, content);
                    message = TeamsMessage.CreateSuccess($"CRA table imported: {name}",
                        $"{content} {model.PayPeriod} CPP brackets were stored for {model.Year}");
                }
                else
                {
                    logger.LogError("The CRA table {Name} failed with status {StatusCode}: {Content}", name, response.StatusCode, content);
                    message = TeamsMessage.CreateError($"CRA table failed: {name}", content);
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "The CRA table {Name} threw an exception", name);
            message = TeamsMessage.CreateError($"CRA table failed: {name}", e.ToString());
        }

        await Notify(message);
    }

    private async Task Notify(TeamsMessage message)
    {
        var teamsClient = httpClientFactory.CreateClient("Teams");
        var notificationResult = await teamsClient.SendTeamsNotification(message, configuration);
        if (!string.IsNullOrEmpty(notificationResult))
        {
            logger.LogWarning("The Teams notification failed: {Result}", notificationResult);
        }
    }
}
