using Covenant.Common.Models.Agency;
using Microsoft.Extensions.Logging;
using Sigook.Functions.Models;

namespace Sigook.Functions.Services
{
    public interface IEmailService
    {
        Task SendEmail(InvitationToApplyModel request, List<WorkerContactInfoModel> workers, ILogger logger);
        Task SendEmail(EmailModel model, ILogger logger);
    }
}