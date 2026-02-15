using Sigook.CognitiveServices.Core.Models.Speech;

namespace Sigook.CognitiveServices.Core.Interfaces.Cloud
{
    public interface ISpeechConverter
    {
        Task<byte[]> TextToAudio(SpeechOptionsModel options);
        Task<IReadOnlyCollection<VoiceInfoModel>> GetVoicesList();
    }
}
