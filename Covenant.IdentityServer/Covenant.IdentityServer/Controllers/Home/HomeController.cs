using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.IdentityServer.Controllers.Home
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public HomeController(IIdentityServerInteractionService interaction,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            _interaction = interaction;
            _configuration = configuration;
            _environment = environment;
        }

        [Authorize]
        public IActionResult Index()
        {
            if (_environment.IsDevelopment()) return View();
            string webClientUrl = _configuration["WebClientUrl"];
            if (!string.IsNullOrEmpty(webClientUrl)) return Redirect(webClientUrl);
            return View();
        }

        /// <summary>
        /// Shows the error page
        /// </summary>
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();
            // retrieve error details from identityserver
            ErrorMessage message = await _interaction.GetErrorContextAsync(errorId);
            if (message == null) return View("Error", vm);
            vm.Error = message;
            return View("Error", vm);
        }

        [HttpGet]
        public IActionResult Success(string message)
        {
            ViewData["WebClientUrl"] = _configuration["WebClientUrl"];
            if (string.IsNullOrEmpty(message))
            {
                message = "That's all, thanks";
            }
            ViewData["Message"] = message;
            return View();
        }

        [HttpGet]
        public IActionResult InvalidUser() => View();

        [HttpGet]
        public IActionResult Info(string key)
        {
            if (key == "3B5D476C-BEE3-4F1E-9437-7423EA357E63") return View();
            return NotFound();
        }
    }
}