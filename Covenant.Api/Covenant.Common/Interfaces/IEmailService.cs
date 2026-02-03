using Covenant.Common.Models;

namespace Covenant.Common.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmail(EmailParams emailParams);
    Task<bool> SendCovenantEmail(EmailParams emailParams);
}