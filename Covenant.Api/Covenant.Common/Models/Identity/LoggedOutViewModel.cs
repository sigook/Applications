namespace Covenant.Common.Models.Identity;

public class LoggedOutViewModel
{
    public string PostLogoutRedirectUri { get; set; }
    public bool AutomaticRedirectAfterSignOut { get; set; } = true;
}
