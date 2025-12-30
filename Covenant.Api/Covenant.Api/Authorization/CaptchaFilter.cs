using Covenant.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Text;

namespace Covenant.Api.Authorization
{
    public class CaptchaFilter : IAsyncActionFilter
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;

        public CaptchaFilter(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (await ValidCaptcha(context))
            {
                await next();
            }
            context.Result = new BadRequestObjectResult("Captcha not valid");
        }

        private async Task<bool> ValidCaptcha(ActionExecutingContext context)
        {
            var client = httpClientFactory.CreateClient();
            var secret = configuration.GetValue<string>("ReCAPTCHASecretKey");
            var payload = context.ActionArguments["contact"] as ContactDto;
            var serializeObject = JsonConvert.SerializeObject(new { secret, response = payload.CaptchaResponse });
            var content = new StringContent(serializeObject, Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await client.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={payload.CaptchaResponse}", content);
            var stringResponse = await response.Content.ReadAsStringAsync() ?? "";
            bool isValid = stringResponse.Contains("true");
            return isValid;
        }
    }
}
