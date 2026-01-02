using System.ComponentModel.DataAnnotations;

namespace Covenant.IdentityServer.Models.Enums
{
    public enum GrantTypes : byte
    {
        [Display(Name = "Implicit")]
        Implicit = 1,
        [Display(Name = "Implict And Client Credentials")]
        ImplicitAndClientCredentials,
        [Display(Name = "Code")]
        Code,
        [Display(Name = "Code And Credentials")]
        CodeAndClientCredentials,
        [Display(Name = "Hybrid")]
        Hybrid,
        [Display(Name = "Hybrid And Client Credentials")]
        HybridAndClientCredentials,
        [Display(Name = "Client Credentials")]
        ClientCredentials,
        [Display(Name = "Resource Owner Password")]
        ResourceOwnerPassword,
        [Display(Name = "Resource Owner Password And Client Credentials")]
        ResourceOwnerPasswordAndClientCredentials
    }
}
