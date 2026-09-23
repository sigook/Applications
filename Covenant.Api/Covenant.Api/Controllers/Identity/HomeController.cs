using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Constants;
using Covenant.Common.Models.Identity;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Server.AspNetCore;

namespace Covenant.Api.Controllers.Identity;

[SecurityHeaders]
[AllowAnonymous]
[ApiExplorerSettings(IgnoreApi = true)]
public class HomeController(IConfiguration configuration) : Controller
{
    [HttpGet("~/")]
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    public IActionResult Index()
    {
        var webClientUrl = configuration.GetWebClientUrl();
        if (!string.IsNullOrEmpty(webClientUrl)) return Redirect(webClientUrl);
        return RedirectToAction(nameof(Success));
    }

    [HttpGet("~/Home/Error")]
    public IActionResult Error()
    {
        var response = HttpContext.GetOpenIddictServerResponse();
        return View("Error", new ErrorViewModel
        {
            Error = response?.Error,
            ErrorDescription = response?.ErrorDescription
        });
    }

    [HttpGet("~/Home/Success")]
    public IActionResult Success(string message)
    {
        ViewData["WebClientUrl"] = configuration.GetWebClientUrl();
        ViewData["Message"] = string.IsNullOrEmpty(message) ? AccountMessages.ThatIsAll : message;
        return View();
    }

    [HttpGet("~/Home/InvalidUser")]
    public IActionResult InvalidUser() => View();
}
