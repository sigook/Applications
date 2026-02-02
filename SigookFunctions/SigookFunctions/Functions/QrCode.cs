using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SigookFunctions.Services;
using System.Net.Mime;

namespace SigookFunctions.Functions
{
    public class QrCode
    {
        [FunctionName(nameof(QrCode))]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req, ILogger log)
        {
            await Task.CompletedTask;
            string text = req.Query["text"];
            if (string.IsNullOrEmpty(text)) return new BadRequestResult();
            byte[] image = QrCodeGenerator.GenerateByteArray(text);
            return new FileContentResult(image, MediaTypeNames.Image.Jpeg);
        }
    }
}