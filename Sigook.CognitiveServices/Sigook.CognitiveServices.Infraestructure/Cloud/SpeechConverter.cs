using System.Net.Http.Json;
using System.Text;
using Sigook.CognitiveServices.Core.Interfaces.Cloud;
using Sigook.CognitiveServices.Core.Models.Speech;

namespace Sigook.CognitiveServices.Infraestructure.Cloud
{
    public class SpeechConverter : ISpeechConverter
    {
        private readonly HttpClient httpClient;
        private readonly string subscriptionKey;
        private readonly string region;
        private readonly string defaultLanguage;
        private readonly string defaultVoiceName;

        public SpeechConverter(HttpClient httpClient, string subscriptionKey, string region, string defaultLanguage, string defaultVoiceName)
        {
            this.httpClient = httpClient;
            this.subscriptionKey = subscriptionKey;
            this.region = region;
            this.defaultLanguage = defaultLanguage;
            this.defaultVoiceName = defaultVoiceName;
        }

        public async Task<IReadOnlyCollection<VoiceInfoModel>> GetVoicesList()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://{region}.tts.speech.microsoft.com/cognitiveservices/voices/list");
            request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var voices = await response.Content.ReadFromJsonAsync<List<VoiceInfoModel>>();
            return voices ?? [];
        }

        public async Task<byte[]> TextToAudio(SpeechOptionsModel options)
        {
            var language = options.Locale ?? defaultLanguage;
            var voiceName = options.ShortName ?? defaultVoiceName;

            var ssml = $"""
                <speak version='1.0' xml:lang='{language}'>
                    <voice name='{voiceName}'>{System.Security.SecurityElement.Escape(options.Text)}</voice>
                </speak>
                """;

            var request = new HttpRequestMessage(HttpMethod.Post,
                $"https://{region}.tts.speech.microsoft.com/cognitiveservices/v1");
            request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            request.Headers.Add("X-Microsoft-OutputFormat", "audio-24khz-160kbitrate-mono-mp3");
            request.Headers.Add("User-Agent", "SigookCognitiveServices");
            request.Content = new StringContent(ssml, Encoding.UTF8, "application/ssml+xml");

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
