using Covenant.Common.Configuration;
using Covenant.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Covenant.Api.Controllers.Shared;

[Route(RouteName)]
[ApiController]
public class MobileAppController(IOptions<MobileAppConfiguration> options) : ControllerBase
{
    public const string RouteName = "api/mobileapp";
    private readonly MobileAppConfiguration configuration = options.Value;

    /// <summary>Gets the minimum supported version of the worker mobile app and the store links to update it.</summary>
    [HttpGet("version")]
    [ProducesResponseType(typeof(MobileAppVersionModel), StatusCodes.Status200OK)]
    public IActionResult GetVersion()
    {
        return Ok(new MobileAppVersionModel
        {
            MinimumVersion = configuration.MinimumVersion,
            AndroidStoreUrl = configuration.AndroidStoreUrl,
            IosStoreUrl = configuration.IosStoreUrl
        });
    }
}
