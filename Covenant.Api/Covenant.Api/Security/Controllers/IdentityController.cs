using Covenant.Common.Interfaces;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Security.Controllers
{
    [Route("identity")]
    [Authorize]
    public class IdentityController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration _configuration;

        public IdentityController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        /// <summary>Gets the claims of the current authenticated identities.</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            var enumerable = User.Identities.Select(c => new { id = c.Claims.Select(i => new { i.Type, i.Value }) });
            return new JsonResult(enumerable);
        }

        /// <summary>Gets the identifier of the current authenticated user.</summary>
        [HttpGet]
        [Route("userId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult UserId()
        {
            bool result = User.TryGetUserId(out Guid id);
            return Json(new { id });
        }

        /// <summary>Deactivates the current authenticated user account.</summary>
        /// <param name="identityServerService">Identity server service.</param>
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> InactiveAccount([FromServices] IIdentityServerService identityServerService)
        {
            if (User.TryGetUserId(out Guid id))
            {
                var result = await identityServerService.InactiveUser(id);
                if (!result)
                {
                    return BadRequest(result.Errors);
                }
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}