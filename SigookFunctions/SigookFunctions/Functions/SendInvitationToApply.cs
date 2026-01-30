using Covenant.Common.Models.Agency;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SigookFunctions.Models;
using SigookFunctions.Services;

namespace SigookFunctions.Functions
{
    public class SendInvitationToApply
    {
        private readonly ISigookApi _sigookApi;
        private readonly IEmailService _emailService;

        public SendInvitationToApply(ISigookApi sigookApi, IEmailService emailService)
        {
            _sigookApi = sigookApi;
            _emailService = emailService;
        }

        [FunctionName(nameof(SendInvitationToApply))]
        public async Task Run([QueueTrigger("invitation-to-apply", Connection = "SigookStorageAccount")] InvitationToApplyModel model,
            ILogger log)
        {
            if (model is null || !model.IsValidModel())
            {
                log?.LogError("Model is null Model={Model}", model?.ToString());
                return;
            }
            log?.LogInformation("Sending email for request: {Request}", model);
            PaginatedList<WorkerContactInfoModel> workers = await _sigookApi.GetWorkers(1, model.AgencyId);
            await _emailService.SendEmail(model, workers.Items, log);
            log?.LogInformation("Email sent it to: {Total}", workers.Items.Count);

            if (!workers.HasNextPage) return;

            for (var i = 2; i <= workers.TotalPages; i++)
            {
                workers = await _sigookApi.GetWorkers(i, model.AgencyId);
                await _emailService.SendEmail(model, workers.Items, log);
                log?.LogInformation("Email sent it to: {Total}", workers.Items.Count);
            }
        }
    }
}