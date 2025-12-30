using Azure.Identity;
using Covenant.Common.Configuration;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Notification;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Covenant.Infrastructure.Services;

public class TeamsService : ITeamsService
{
    private string siteId;
    private string driveId;

    private readonly Microsoft365Configuration microsoft365Configuration;
    private readonly IHttpClientFactory _clientFactory;
    private readonly GraphServiceClient graphClient;

    public TeamsService(IOptions<Microsoft365Configuration> options, IHttpClientFactory clientFactory)
    {
        microsoft365Configuration = options.Value;
        _clientFactory = clientFactory;
        var credentials = new ClientSecretCredential(microsoft365Configuration.TenantId, microsoft365Configuration.ClientId, microsoft365Configuration.ClientSecret);
        graphClient = new GraphServiceClient(credentials);
    }

    private string DriveId
    {
        get
        {
            if (string.IsNullOrWhiteSpace(siteId))
            {
                var site = graphClient.Sites["covenantgroupltd.sharepoint.com:/sites/Covenant"]
                    .GetAsync()
                    .GetAwaiter()
                    .GetResult();
                siteId = site.Id;
            }
            if (string.IsNullOrWhiteSpace(driveId))
            {
                var drive = graphClient.Sites[siteId]
                    .Drive
                    .GetAsync()
                    .GetAwaiter()
                    .GetResult();
                driveId = drive.Id;
            }
            return driveId;
        }
    }

    public async Task<Stream> GetTeamsFile(string url)
    {
        var filePath = GetFilePath(url);
        var file = await graphClient
            .Drives[DriveId]
            .Root
            .ItemWithPath(filePath)
            .Content
            .GetAsync();
        return file;
    }

    public async Task<Result> SendNotification(string webhook, TeamsNotificationModel notification)
    {
        try
        {
            if (string.IsNullOrEmpty(webhook))
            {
                return Result.Fail("TeamsAccountingWebhook url not found");
            }
            string json = JsonConvert.SerializeObject(notification);
            var content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpClient client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.PostAsync(webhook, content);
            return response.IsSuccessStatusCode ? Result.Ok() : Result.Fail(await response.Content.ReadAsStringAsync());
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }

    private string GetFilePath(string url)
    {
        var decodedUrl = Uri.UnescapeDataString(url);
        var index = decodedUrl.IndexOf("General", StringComparison.OrdinalIgnoreCase);
        var result = decodedUrl.Substring(index);
        return result;
    }

    public async Task<Result> ExistsTeamsFile(string url)
    {
        var filePath = GetFilePath(url);
        try
        {
            var file = await graphClient
                .Drives[DriveId]
                .Root
                .ItemWithPath(filePath)
                .Content
                .GetAsync();
            return Result.Ok();
        }
        catch (Exception)
        {
            return Result.Fail();
        }
    }
}