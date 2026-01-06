using Covenant.IdentityServer.Models.Enums;
using IdentityServer4.EntityFramework.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Covenant.IdentityServer.Models.ViewModels
{
    public class ClientViewModel
    {
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string ClientName { get; set; }
        [Required]
        public bool RequireConsent { get; set; }
        [Required]
        public bool RequirePkce { get; set; }
        [Required]
        public GrantTypes AllowedGrantType { get; set; }
        [Required]
        public int AccessTokenLifetime { get; set; }
        [Required]
        public bool AllowAccessTokensViaBrowser { get; set; }
        [Required]
        public bool AllowOfflineAccess { get; set; }
        [Required]
        public string RedirectUris { get; set; }
        [Required]
        public string PostLogoutRedirectUris { get; set; }
        [Required]
        public string AllowedCorsOrigins { get; set; }
        [Required]
        public string ClientUri { get; set; }
        [Required]
        public List<string> AllowedScopes { get; set; }
        [Required]
        public string ClientSecret { get; set; }

        public ICollection<string> AllowedGrantTypes
        {
            get
            {
                switch (AllowedGrantType)
                {
                    case GrantTypes.Implicit:
                        return IdentityServer4.Models.GrantTypes.Implicit;
                    case GrantTypes.ImplicitAndClientCredentials:
                        return IdentityServer4.Models.GrantTypes.ImplicitAndClientCredentials;
                    case GrantTypes.Code:
                        return IdentityServer4.Models.GrantTypes.Code;
                    case GrantTypes.CodeAndClientCredentials:
                        return IdentityServer4.Models.GrantTypes.CodeAndClientCredentials;
                    case GrantTypes.Hybrid:
                        return IdentityServer4.Models.GrantTypes.Hybrid;
                    case GrantTypes.HybridAndClientCredentials:
                        return IdentityServer4.Models.GrantTypes.HybridAndClientCredentials;
                    case GrantTypes.ClientCredentials:
                        return IdentityServer4.Models.GrantTypes.ClientCredentials;
                    case GrantTypes.ResourceOwnerPassword:
                        return IdentityServer4.Models.GrantTypes.ResourceOwnerPassword;
                    case GrantTypes.ResourceOwnerPasswordAndClientCredentials:
                        return IdentityServer4.Models.GrantTypes.ResourceOwnerPasswordAndClientCredentials;
                    default:
                        return default;
                }
            }
        }

        public IEnumerable<CustomSecret> ClientSecrets
        {
            get
            {
                return ClientSecret == null ? default : JsonConvert.DeserializeObject<IEnumerable<CustomSecret>>(ClientSecret);
            }
        }

        public GrantTypes GetAllowedGrantType(IEnumerable<string> grantTypes)
        {
            if (IsGrantType(IdentityServer4.Models.GrantTypes.Implicit, grantTypes))
                return GrantTypes.Implicit;
            if (IsGrantType(IdentityServer4.Models.GrantTypes.ImplicitAndClientCredentials, grantTypes))
                return GrantTypes.ImplicitAndClientCredentials;
            if (IsGrantType(IdentityServer4.Models.GrantTypes.Code, grantTypes))
                return GrantTypes.Code;
            if (IsGrantType(IdentityServer4.Models.GrantTypes.CodeAndClientCredentials, grantTypes))
                return GrantTypes.CodeAndClientCredentials;
            if (IsGrantType(IdentityServer4.Models.GrantTypes.Hybrid, grantTypes))
                return GrantTypes.Hybrid;
            if (IsGrantType(IdentityServer4.Models.GrantTypes.HybridAndClientCredentials, grantTypes))
                return GrantTypes.HybridAndClientCredentials;
            if (IsGrantType(IdentityServer4.Models.GrantTypes.ClientCredentials, grantTypes))
                return GrantTypes.ClientCredentials;
            if (IsGrantType(IdentityServer4.Models.GrantTypes.ResourceOwnerPassword, grantTypes))
                return GrantTypes.ResourceOwnerPassword;
            if (IsGrantType(IdentityServer4.Models.GrantTypes.ResourceOwnerPasswordAndClientCredentials, grantTypes))
                return GrantTypes.ResourceOwnerPasswordAndClientCredentials;
            throw new ArgumentException("GrantType doesn't exist");
        }

        private bool IsGrantType(IEnumerable<string> grantTypesFromModels, IEnumerable<string> grantTypesFromDb)
        {
            return grantTypesFromModels.Any(gtfm => grantTypesFromDb.Contains(gtfm));
        }

        public static ClientViewModel ViewModelFromClient(Client client)
        {
            var viewModel = new ClientViewModel
            {
                ClientId = client.ClientId,
                ClientName = client.ClientName,
                RequireConsent = client.RequireConsent,
                RequirePkce = client.RequirePkce,
                AccessTokenLifetime = client.AccessTokenLifetime,
                AllowAccessTokensViaBrowser = client.AllowAccessTokensViaBrowser,
                AllowOfflineAccess = client.AllowOfflineAccess,
                RedirectUris = string.Join(",", client.RedirectUris.Select(ru => ru.RedirectUri)),
                PostLogoutRedirectUris = string.Join(",", client.PostLogoutRedirectUris.Select(plru => plru.PostLogoutRedirectUri)),
                AllowedCorsOrigins = string.Join(",", client.AllowedCorsOrigins.Select(plru => plru.Origin)),
                ClientUri = client.ClientUri,
                AllowedScopes = client.AllowedScopes.Select(@as => @as.Scope).ToList(),
                ClientSecret = JsonConvert.SerializeObject(client.ClientSecrets.Select(cs => new CustomSecret
                {
                    Id = cs.Id,
                    Value = cs.Value,
                    Description = cs.Description
                }))
            };
            viewModel.AllowedGrantType = viewModel.GetAllowedGrantType(client.AllowedGrantTypes.Select(agt => agt.GrantType));
            return viewModel;
        }
    }
}
