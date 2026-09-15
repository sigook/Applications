using Azure.Storage.Blobs;
using Covenant.Common.Models.Notification;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sigook.Functions.Configuration;
using Sigook.Functions.Models;
using Sigook.Functions.Services;
using Sigook.Functions.Utils;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Sigook.Functions.Functions;

public class CraTables(
    IHttpClientFactory httpClientFactory,
    IAccessTokenProvider accessTokenProvider,
    ILogger<CraTables> logger,
    IConfiguration configuration,
    IOptions<CraTablesOptions> options)
{
    private readonly CraTablesOptions _options = options.Value;

    [Function(nameof(CraTableUploaded))]
    public async Task CraTableUploaded(
        [BlobTrigger($"{CraTablesOptions.ContainerName}/{{name}}", Connection = CraTablesOptions.StorageConnectionName)] BlobClient blob,
        string name)
    {
        logger.LogInformation("A CRA table was uploaded: {Name}", name);

        if (!CraBlobName.TryParse(name, out var table, out var nameError))
        {
            logger.LogError("The CRA table {Name} was ignored: {Error}", name, nameError);
            await Notify(TeamsNotificationModel.CreateError($"CRA table ignored: {name}", nameError));
            return;
        }

        TeamsNotificationModel message;
        try
        {
            var apiUrl = table.Kind == CraTableKind.Cpp ? _options.CppApiUrl : _options.TaxApiUrl;
            if (string.IsNullOrEmpty(apiUrl))
            {
                var setting = $"CraTables:{(table.Kind == CraTableKind.Cpp ? nameof(_options.CppApiUrl) : nameof(_options.TaxApiUrl))}";
                logger.LogError("{Setting} is not configured", setting);
                message = TeamsNotificationModel.CreateError("API url is missing", $"{setting} is not set");
            }
            else
            {
                var apiClient = httpClientFactory.CreateClient(HttpClients.Api);
                var token = await accessTokenProvider.GetToken();

                var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
                {
                    Content = JsonContent.Create(table.Import)
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                logger.LogInformation("Importing the {PayPeriod} {Year} {Table} table from {Name}",
                    table.Import.PayPeriod, table.Import.Year, table.Label, name);
                HttpResponseMessage response = await apiClient.SendAsync(request);
                string content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation("The CRA table {Name} was imported with {Rows} rows", name, content);
                    message = TeamsNotificationModel.CreateSuccess($"CRA table imported: {name}",
                        $"{content} {table.Import.PayPeriod} {table.Label} brackets were stored for {table.Import.Year}");
                }
                else
                {
                    logger.LogError("The CRA table {Name} failed with status {StatusCode}: {Content}", name, response.StatusCode, content);
                    message = TeamsNotificationModel.CreateError($"CRA table failed: {name}", content);
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "The CRA table {Name} threw an exception", name);
            message = TeamsNotificationModel.CreateError($"CRA table failed: {name}", e.ToString());
        }

        await Notify(message);
    }

    private async Task Notify(TeamsNotificationModel message)
    {
        var teamsClient = httpClientFactory.CreateClient(HttpClients.Teams);
        var notificationResult = await teamsClient.SendTeamsNotification(message, configuration);
        if (!string.IsNullOrEmpty(notificationResult))
        {
            logger.LogWarning("The Teams notification failed: {Result}", notificationResult);
        }
    }
}
