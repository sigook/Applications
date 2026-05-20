using Covenant.Common.Functionals;
using Covenant.Common.Models;

namespace Covenant.Common.Interfaces;

public interface ISendGridService
{
    Task<Result> SendEmail(SendGridModel model);

    Task<Result> SendTemplateBatch(string recruitmentEmail, IReadOnlyCollection<TemplateRecipient> recipients);
}
