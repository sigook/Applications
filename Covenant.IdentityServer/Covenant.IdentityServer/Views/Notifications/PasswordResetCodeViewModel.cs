namespace Covenant.IdentityServer.Views.Notifications
{
    public class PasswordResetCodeViewModel
    {
        public string Code { get; set; }

        public int ExpiresMinutes { get; set; }
    }
}
