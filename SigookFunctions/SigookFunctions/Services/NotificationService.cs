using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SigookFunctions.Models;
using SigookFunctions.Options;

namespace SigookFunctions.Services
{
    public class NotificationService : INotificationService
    {
        private readonly SigookFunctionsOptions _options;
        private readonly HttpClient _client;

        public NotificationService(IOptions<SigookFunctionsOptions> options)
        {
            _options = options.Value;
            _client = new HttpClient();
        }

        public async Task<string> SendTeamsNotificationAsync(TeamsMessage message)
        {
            string url = Environment.GetEnvironmentVariable("TeamsWebhook")
                         ?? _options.TeamsWebhook;

            if (string.IsNullOrEmpty(url))
                return "Teams url not found";

            string json = JsonConvert.SerializeObject(message);
            var content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await _client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
                return string.Empty;

            return await response.Content.ReadAsStringAsync();
        }
    }
}
