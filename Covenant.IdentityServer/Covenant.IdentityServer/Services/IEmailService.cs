using System.Threading.Tasks;

namespace Covenant.IdentityServer.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmail(string email, string subject, string message);
    }
}