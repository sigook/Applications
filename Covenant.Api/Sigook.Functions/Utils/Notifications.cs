using Covenant.Common.Models.Notification;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sigook.Functions.Utils;

public static class Notifications
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static async Task<string> SendTeamsNotification(this HttpClient client, TeamsNotificationModel message, IConfiguration configuration)
    {
        string url = configuration["TeamsWebhook"];
        if (string.IsNullOrEmpty(url)) return "TeamsWebhook configuration is not set";
        var adaptiveMessage = TeamsAdaptiveMessage.FromNotification(message);
        string json = JsonSerializer.Serialize(adaptiveMessage, SerializerOptions);

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json)
        };
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        HttpResponseMessage response = await client.SendAsync(request);
        if (response.IsSuccessStatusCode) return string.Empty;
        return $"Teams webhook returned {response.StatusCode}: {await response.Content.ReadAsStringAsync()}";
    }
}
