using Covenant.Common.Models.Agency;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SigookFunctions.Models;
using SigookFunctions.Services;

namespace SigookFunctions.Functions
{
    public class SendInvitationToApply
    {
        private readonly ISigookApi _sigookApi;
        private readonly IEmailService _emailService;
        private readonly ILogger<SendInvitationToApply> _logger;

        public SendInvitationToApply(ISigookApi sigookApi, IEmailService emailService, ILogger<SendInvitationToApply> logger)
        {
            _sigookApi = sigookApi;
            _emailService = emailService;
            _logger = logger;
        }

        [Function(nameof(SendInvitationToApply))]
        public async Task Run([QueueTrigger("invitation-to-apply", Connection = "SigookStorageAccount")] InvitationToApplyModel model)
        {
            if (model is null || !model.IsValidModel())
            {
                _logger.LogError("Model is null Model={Model}", model?.ToString());
                return;
            }
            _logger.LogInformation("Sending email for request: {Request}", model);
            PaginatedList<WorkerContactInfoModel> workers = await _sigookApi.GetWorkers(1, model.AgencyId);
            await _emailService.SendEmail(model, workers.Items, _logger);
            _logger.LogInformation("Email sent it to: {Total}", workers.Items.Count);

            if (!workers.HasNextPage) return;

            for (var i = 2; i <= workers.TotalPages; i++)
            {
                workers = await _sigookApi.GetWorkers(i, model.AgencyId);
                await _emailService.SendEmail(model, workers.Items, _logger);
                _logger.LogInformation("Email sent it to: {Total}", workers.Items.Count);
            }
        }
    }
}
