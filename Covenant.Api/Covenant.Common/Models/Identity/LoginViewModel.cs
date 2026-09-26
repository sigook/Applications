namespace Covenant.Common.Models.Identity;

public class LoginViewModel : LoginInputModel
{
    public bool AllowRememberLogin { get; set; } = true;
    public string ClientUri { get; set; }
    public bool ShowLinkHome { get; set; }
    public bool IsAccountConfirmed { get; set; } = true;
    public bool ShowMicrosoft365Button { get; set; }
    public string ExternalLoginScheme { get; set; }
}
