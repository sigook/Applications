using Microsoft.CognitiveServices.Speech;
using Sigook.CognitiveServices.Core.Models.Speech;

namespace Sigook.CognitiveServices.Core.BussinesServices.Interfaces
{
    public interface ISpeechService
    {
        Task<IEnumerable<string>> GetLanguages();
        Task<IEnumerable<VoiceInfo>> GetVoices(string language);
        Task<byte[]> ConvertText(SpeechOptionsModel model);
    }
}
