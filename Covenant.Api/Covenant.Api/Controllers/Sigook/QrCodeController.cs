using Covenant.Api.Utils.Extensions;
using Covenant.Common.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Covenant.Api.Controllers.Sigook
{
    [Route(RouteName)]
    [ApiController]
    [Authorize]
    public class QrCodeController : ControllerBase
    {
        public const string RouteName = "api/QrCode";

        [HttpGet("{text}")]
        public async Task<IActionResult> Get([FromServices] IHttpClientFactory clientFactory,
            [FromServices] IConfiguration configuration,
            string text)
        {
            if (string.IsNullOrEmpty(text)) return BadRequest(ModelState.AddError(ValidationMessages.RequiredMsg("text")));
            var serviceUrl = configuration.GetValue<string>("UrlQrCodeService");
            if (string.IsNullOrEmpty(serviceUrl)) return BadRequest(ModelState.AddError("Service unavailable"));
            HttpClient client = clientFactory.CreateClient();
            byte[] image = await client.GetByteArrayAsync($"{serviceUrl}&text={text}");
            return File(image, MediaTypeNames.Image.Jpeg);
        }
    }
}