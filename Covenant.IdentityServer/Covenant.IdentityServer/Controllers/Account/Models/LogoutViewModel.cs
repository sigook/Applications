namespace Covenant.IdentityServer.Controllers.Account.Models
{
    public class LogoutViewModel : LogoutInputModel
    {
        public bool ShowLogoutPrompt { get; set; } = true;
    }
}
