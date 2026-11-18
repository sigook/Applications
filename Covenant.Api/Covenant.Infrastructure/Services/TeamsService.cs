using Azure.Identity;
using Covenant.Common.Configuration;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Notifications;
using Covenant.Common.Models.Teams;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Covenant.Infrastructure.Services;

public class TeamsService : ITeamsService
{
    private const string SharePointHost = "covenantgroupltd.sharepoint.com";

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
            var message = TeamsAdaptiveMessage.FromNotification(notification);
            string json = JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
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
        var result = decodedUrl[index..];
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

    public async Task<Result<IReadOnlyList<TeamsChannelFileModel>>> FindChannelFiles(TeamsChannelFileFilter filter)
    {
        var driveId = await GetChannelDriveId(filter.Team, filter.Channel);
        if (driveId is null) return Result.Fail<IReadOnlyList<TeamsChannelFileModel>>(ChannelNotFound(filter));
        var files = await SearchFiles(driveId, filter.Name);
        return Result.Ok<IReadOnlyList<TeamsChannelFileModel>>(files
            .Select(f => new TeamsChannelFileModel(f.Name, f.WebUrl, f.Size, f.LastModifiedDateTime, f.LastModifiedBy?.User?.DisplayName))
            .ToList());
    }

    public async Task<Result<Stream>> GetChannelFile(TeamsChannelFileFilter filter)
    {
        var driveId = await GetChannelDriveId(filter.Team, filter.Channel);
        if (driveId is null) return Result.Fail<Stream>(ChannelNotFound(filter));
        var file = (await SearchFiles(driveId, filter.Name))
            .FirstOrDefault(f => string.Equals(f.Name, filter.Name.Trim(), StringComparison.OrdinalIgnoreCase));
        if (file is null) return Result.Fail<Stream>(ResultError.NotFound($"File '{filter.Name}' not found in channel '{filter.Channel}' of team '{filter.Team}'"));
        var content = await graphClient.Drives[driveId].Items[file.Id].Content.GetAsync();
        return Result.Ok(content);
    }

    private static ResultError ChannelNotFound(TeamsChannelFileFilter filter) =>
        ResultError.NotFound($"Channel '{filter.Channel}' of team '{filter.Team}' not found or not readable");

    private async Task<string> GetChannelDriveId(string team, string channel)
    {
        try
        {
            var site = await graphClient.Sites[$"{SharePointHost}:/sites/{team.Trim()}-{channel.Trim()}"].GetAsync();
            var drive = await graphClient.Sites[site.Id].Drive.GetAsync();
            return drive?.Id;
        }
        catch (ODataError e) when (e.ResponseStatusCode is 403 or 404)
        {
            return null;
        }
    }

    private async Task<List<DriveItem>> SearchFiles(string driveId, string name)
    {
        var response = await graphClient.Drives[driveId].SearchWithQ(name.Trim()).GetAsSearchWithQGetResponseAsync();
        return (response?.Value ?? [])
            .Where(i => i.File is not null)
            .OrderByDescending(i => i.LastModifiedDateTime)
            .ToList();
    }
}
