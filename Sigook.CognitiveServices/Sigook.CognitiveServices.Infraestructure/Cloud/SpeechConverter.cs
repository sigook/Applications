using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Sigook.CognitiveServices.Core.Interfaces.Cloud;
using Sigook.CognitiveServices.Core.Models.Speech;

namespace Sigook.CognitiveServices.Infraestructure.Cloud
{
    public class SpeechConverter : ISpeechConverter
    {
        private readonly SpeechConfig speechConfig;

        public SpeechConverter(SpeechConfig speechConfig)
        {
            this.speechConfig = speechConfig;
        }

        public async Task<IReadOnlyCollection<VoiceInfo>> GetVoicesList()
        {
            using SpeechSynthesizer synthesizer = new(speechConfig);
            SynthesisVoicesResult voicesResult = await synthesizer.GetVoicesAsync();
            return voicesResult.Voices;
        }

        public async Task<byte[]> TextToAudio(SpeechOptionsModel options)
        {
            speechConfig.SpeechSynthesisLanguage = options.Locale ?? speechConfig.SpeechSynthesisLanguage;
            speechConfig.SpeechSynthesisVoiceName = options.ShortName ?? speechConfig.SpeechSynthesisVoiceName;
            speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio24Khz160KBitRateMonoMp3);
            
            using SpeechSynthesizer synthesizer = new(speechConfig);
            SpeechSynthesisResult result = await synthesizer.SpeakTextAsync(options.Text);
            return result?.AudioData;
        }
    }
}
