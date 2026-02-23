using System.ComponentModel.DataAnnotations;

namespace Sigook.CognitiveServices.UI.ViewModels
{
    public class SpeechViewModel
    {
        [Required]
        public string Text { get; set; }
        [Required]
        public string Language { get; set; }
        [Required]
        public string Voice { get; set; }
        public IEnumerable<string> Languages { get; set; }
    }
}
