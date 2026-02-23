using Sigook.CognitiveServices.Core.BussinesServices.Interfaces;
using Sigook.CognitiveServices.Core.Interfaces.Cloud;
using Sigook.CognitiveServices.Core.Models.Speech;

namespace Sigook.CognitiveServices.Core.BussinesServices.Implementations
{
    public class SpeechService : ISpeechService
    {
        private readonly ISpeechConverter speechConverter;

        public SpeechService(ISpeechConverter speechConverter)
        {
            this.speechConverter = speechConverter;
        }

        public async Task<byte[]> ConvertText(SpeechOptionsModel model)
        {
            return await speechConverter.TextToAudio(model);
        }

        public async Task<IEnumerable<string>> GetLanguages()
        {
            var voicesList = await speechConverter.GetVoicesList();
            var languages = voicesList.GroupBy(vl => vl.Locale).Select(x => x.Key);
            return languages;
        }

        public async Task<IEnumerable<VoiceInfoModel>> GetVoices(string language)
        {
            var voicesList = await speechConverter.GetVoicesList();
            var selectedVoices = voicesList.Where(vl => vl.Locale == language);
            return selectedVoices;
        }
    }
}
