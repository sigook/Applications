using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SigookFunctions.Models;
using SigookFunctions.Services;

namespace SigookFunctions.Functions;

public class SendEmail
{
    private readonly IEmailService _emailService;
    private readonly ILogger<SendEmail> _logger;

    public SendEmail(IEmailService emailService, ILogger<SendEmail> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>
    /// Used by sigook api (WorkerApplicationService)
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Function(nameof(SendEmail))]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var model = JsonConvert.DeserializeObject<EmailModel>(requestBody);
        await _emailService.SendEmail(model, _logger);
        return new OkResult();
    }
}
