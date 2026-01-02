using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace Covenant.IdentityServer.Controllers.Account.Models
{
    public class LoginViewModel : LoginInputModel
    {
        public bool AllowRememberLogin { get; set; } = true;
        public bool EnableLocalLogin { get; set; } = true;

        public IEnumerable<ExternalProvider> ExternalProviders { get; set; }
        public IEnumerable<ExternalProvider> VisibleExternalProviders => ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));

        public bool IsExternalLoginOnly => EnableLocalLogin == false && ExternalProviders?.Count() == 1;
        public string ExternalLoginScheme => IsExternalLoginOnly ? ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;
        public string ClientUri { get; set; }
        public bool ShowLinkHome { get; set; }
        public bool IsAccountConfirmed { get; set; } = true;
        public bool ShowMicrosoft365Button { get; set; }
    }
}