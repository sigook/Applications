using Newtonsoft.Json;
using SigookFunctions.Models;
using System.Net.Http.Headers;

namespace SigookFunctions.Utils
{
    public static class Notifications
    {
        public static async Task<string> SendTeamsNotification(this HttpClient client, TeamsMessage message)
        {
            string url = Environment.GetEnvironmentVariable("TeamsWebhook");
            if (string.IsNullOrEmpty(url)) return "Teams url not found";
            string json = JsonConvert.SerializeObject(message);
            var content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode) return string.Empty;
            return await response.Content.ReadAsStringAsync();
        }

    }
}