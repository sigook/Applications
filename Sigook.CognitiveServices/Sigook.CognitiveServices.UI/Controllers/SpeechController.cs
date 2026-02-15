using Microsoft.AspNetCore.Mvc;
using Sigook.CognitiveServices.Core.BussinesServices.Interfaces;
using Sigook.CognitiveServices.Core.Models.Speech;
using Sigook.CognitiveServices.UI.Models;
using Sigook.CognitiveServices.UI.ViewModels;
using System.Diagnostics;

namespace Sigook.CognitiveServices.UI.Controllers
{
    public class SpeechController : Controller
    {
        private readonly ILogger<SpeechController> _logger;
        private readonly ISpeechService speechService;

        public SpeechController(ILogger<SpeechController> logger, ISpeechService speechService)
        {
            _logger = logger;
            this.speechService = speechService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var viewModel = new SpeechViewModel
            {
                Languages = await speechService.GetLanguages()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm] SpeechViewModel viewModel)
        {
            var audio = await speechService.ConvertText(new SpeechOptionsModel
            {
                Text = viewModel.Text,
                Locale = viewModel.Language,
                ShortName = viewModel.Voice
            });
            return Ok(audio);
        }

        [HttpGet("{language}/Voices")]
        public async Task<IActionResult> Voices([FromRoute] string language)
        {
            var result = await speechService.GetVoices(language);
            return Ok(result);
        }

    }
}