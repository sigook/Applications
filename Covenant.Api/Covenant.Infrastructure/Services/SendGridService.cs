using Covenant.Common.Configuration;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Http.Headers;

namespace Covenant.Infrastructure.Services
{
    public class SendGridService : ISendGridService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly SendGridConfiguration configuration;
        private readonly ILogger<SendGridService> logger;

        public SendGridService(
            IHttpClientFactory httpClientFactory,
            IOptions<SendGridConfiguration> options,
            ILogger<SendGridService> logger)
        {
            configuration = options.Value;
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        public async Task<Result> SendEmail(SendGridModel model)
        {
            var templateId = await GetTemplateId(model.Template);
            if (!templateId) return Result.Fail(templateId.Errors);
            var message = MailHelper.CreateSingleTemplateEmailToMultipleRecipients(
                new EmailAddress(configuration.From),
                model.Tos.Select(t => new EmailAddress(t)).ToList(),
                templateId.Value,
                model.Data);
            message.SetSandBoxMode(configuration.SandBox);
            var client = new SendGridClient(configuration.ApiKey);
            var response = await client.SendEmailAsync(message);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Error sending emails error: {Error}", await response.Body.ReadAsStringAsync());
                return Result.Fail();
            }
            return Result.Ok();
        }

        private async Task<Result<string>> GetTemplateId(string templateName)
        {
            var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(configuration.TemplatesUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuration.ApiKey);
            var response = await client.GetAsync("templates?generations=dynamic");
            if (!response.IsSuccessStatusCode) return Result.Fail<string>("It is not possible to get the templates");
            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            var template = json["templates"].FirstOrDefault(t => t["name"].ToString() == templateName);
            if (template == null) return Result.Fail<string>($"{templateName} was not found");
            var templateId = template["id"].ToString();
            return Result.Ok(templateId);
        }
    }
}
