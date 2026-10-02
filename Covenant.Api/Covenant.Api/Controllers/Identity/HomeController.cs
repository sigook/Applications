using Covenant.Api.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Identity;

[AllowAnonymous]
[ApiExplorerSettings(IgnoreApi = true)]
public class HomeController(IConfiguration configuration) : ControllerBase
{
    private const string InvalidUserError = "invalid_user";

    [HttpGet("~/")]
    public IActionResult Index() => Redirect(WebClientUrl());

    [HttpGet("~/Home/InvalidUser")]
    public IActionResult InvalidUser() => Redirect($"{WebClientUrl()}/login?error={InvalidUserError}");

    private string WebClientUrl() => configuration.GetWebClientUrl()?.TrimEnd('/');
}
