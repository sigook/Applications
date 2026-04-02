using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Sigook.Functions.Models;
using System.Net.Http.Headers;

namespace Sigook.Functions.Utils
{
    public static class Notifications
    {
        public static async Task<string> SendTeamsNotification(this HttpClient client, TeamsMessage message, IConfiguration configuration)
        {
            string url = configuration["TeamsWebhook"];
            if (string.IsNullOrEmpty(url)) return "TeamsWebhook configuration is not set";
            string json = JsonConvert.SerializeObject(message);

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(json);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode) return string.Empty;
            return $"Teams webhook returned {response.StatusCode}: {await response.Content.ReadAsStringAsync()}";
        }
    }
}
