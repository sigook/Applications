using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SigookFunctions.Models;
using SigookFunctions.Services;

namespace SigookFunctions.Functions
{
    public class SendEmail
    {
        private readonly IEmailService _emailService;

        public SendEmail(IEmailService emailService) => _emailService = emailService;

        /// <summary>
        /// Used by sigook api (WorkerApplicationService)
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(SendEmail))]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var model = JsonConvert.DeserializeObject<EmailModel>(requestBody);
            await _emailService.SendEmail(model, log);
            return new OkResult();
        }
    }
}