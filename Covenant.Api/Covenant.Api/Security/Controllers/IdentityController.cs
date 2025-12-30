using Covenant.Common.Interfaces;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet]
        public IActionResult Get()
        {
            var enumerable = User.Identities.Select(c => new { id = c.Claims.Select(i => new { i.Type, i.Value }) });
            return new JsonResult(enumerable);
        }

        [HttpGet]
        [Route("userId")]
        public IActionResult UserId()
        {
            bool result = User.TryGetUserId(out Guid id);
            return Json(new { id });
        }

        [HttpPatch]
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