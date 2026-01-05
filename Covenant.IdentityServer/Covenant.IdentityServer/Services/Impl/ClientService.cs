using Covenant.IdentityServer.Models.ViewModels;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;

namespace Covenant.IdentityServer.Services.Impl
{
    public class ClientService : IClientService
    {
        private readonly ConfigurationDbContext context;

        public ClientService(ConfigurationDbContext context)
        {
            this.context = context;
        }

        public async Task AddClient(ClientViewModel clientViewModel)
        {
            var client = new IdentityServer4.Models.Client
            {
                ClientId = clientViewModel.ClientId,
                ClientName = clientViewModel.ClientName,
                RequireConsent = clientViewModel.RequireConsent,
                RequirePkce = clientViewModel.RequirePkce,
                AllowedGrantTypes = clientViewModel.AllowedGrantTypes,
                AccessTokenLifetime = clientViewModel.AccessTokenLifetime,
                AllowAccessTokensViaBrowser = clientViewModel.AllowAccessTokensViaBrowser,
                AllowOfflineAccess = clientViewModel.AllowOfflineAccess,
                RedirectUris = clientViewModel.RedirectUris.Split(","),
                PostLogoutRedirectUris = clientViewModel.PostLogoutRedirectUris.Split(","),
                AllowedCorsOrigins = clientViewModel.AllowedCorsOrigins.Split(","),
                ClientUri = clientViewModel.ClientUri,
                AllowedScopes = clientViewModel.AllowedScopes,
                ClientSecrets = clientViewModel.ClientSecrets.Select(cs => new IdentityServer4.Models.Secret
                {
                    Value = cs.Value.Sha256(),
                    Description = cs.Description,
                }).ToList()
            };
            if (!await context.Clients.AnyAsync(c => c.ClientId == client.ClientId))
            {
                await context.Clients.AddAsync(client.ToEntity());
                await context.SaveChangesAsync();
            }
        }

        public async Task<IdentityServer4.EntityFramework.Entities.Client> GetClientDetails(int id)
        {
            var client = await context.Clients
                .FirstOrDefaultAsync(c => c.Id == id);
            if (client == null)
            {
                throw new Exception("Client not found");
            }
            var redirectUris = await context.Set<ClientRedirectUri>()
                .Where(cru => cru.Client.Id == client.Id)
                .ToListAsync();
            var postLogoutRedirectUris = await context.Set<ClientPostLogoutRedirectUri>()
                .Where(cplru => cplru.Client.Id == client.Id)
                .ToListAsync();
            var clientCorsOrigin = await context.Set<ClientCorsOrigin>()
                .Where(cco => cco.Client.Id == client.Id)
                .ToListAsync();
            var clientClaims = await context.Set<IdentityServer4.EntityFramework.Entities.ClientClaim>()
                .Where(cc => cc.Client.Id == client.Id)
                .ToListAsync();
            var clientScopes = await context.Set<ClientScope>()
                .Where(cs => cs.Client.Id == client.Id)
                .ToListAsync();
            var clientGrantTypes = await context.Set<ClientGrantType>()
                .Where(cgt => cgt.Client.Id == client.Id)
                .ToListAsync();
            var clientSecrets = await context.Set<ClientSecret>()
                .Where(cs => cs.Client.Id == client.Id)
                .ToListAsync();
            client.RedirectUris = redirectUris;
            client.PostLogoutRedirectUris = postLogoutRedirectUris;
            client.AllowedCorsOrigins = clientCorsOrigin;
            client.Claims = clientClaims;
            client.AllowedScopes = clientScopes;
            client.AllowedGrantTypes = clientGrantTypes;
            client.ClientSecrets = clientSecrets;
            return client;
        }

        public async Task UpdateClient(int id, ClientViewModel clientViewModel)
        {
            var client = await GetClientDetails(id);
            #region Remove Data
            context.Set<ClientRedirectUri>().RemoveRange(client.RedirectUris);
            context.Set<ClientPostLogoutRedirectUri>().RemoveRange(client.PostLogoutRedirectUris);
            context.Set<ClientCorsOrigin>().RemoveRange(client.AllowedCorsOrigins);
            context.Set<ClientScope>().RemoveRange(client.AllowedScopes);
            var clientSecretsToRemove = client.ClientSecrets.Where(cs => !clientViewModel.ClientSecrets.Where(csvm => csvm.Id.HasValue).Any(csvm => cs.Id == csvm.Id));
            context.Set<ClientSecret>().RemoveRange(clientSecretsToRemove);
            await context.SaveChangesAsync();
            #endregion
            #region Add New Data
            await context.Set<ClientRedirectUri>().AddRangeAsync(clientViewModel.RedirectUris.Split(",").Select(ru => new ClientRedirectUri
            {
                RedirectUri = ru,
                Client = client
            }));
            await context.Set<ClientPostLogoutRedirectUri>().AddRangeAsync(clientViewModel.PostLogoutRedirectUris.Split(",").Select(plru => new ClientPostLogoutRedirectUri
            {
                PostLogoutRedirectUri = plru,
                Client = client
            }));
            await context.Set<ClientCorsOrigin>().AddRangeAsync(clientViewModel.AllowedCorsOrigins.Split(",").Select(acr => new ClientCorsOrigin
            {
                Origin = acr,
                Client = client
            }));
            await context.Set<ClientScope>().AddRangeAsync(clientViewModel.AllowedScopes.Select(@as => new ClientScope
            {
                Scope = @as,
                Client = client
            }));
            foreach (var clientSecret in clientViewModel.ClientSecrets)
            {
                var secret = client.ClientSecrets.FirstOrDefault(cs => cs.Id == clientSecret.Id);
                if (secret != null)
                {
                    secret.Description = clientSecret.Description;
                }
                else
                {
                    await context.Set<ClientSecret>().AddAsync(new ClientSecret
                    {
                        Description = clientSecret.Description,
                        Type = clientSecret.Type,
                        Value = clientSecret.Value.Sha256(),
                        Client = client
                    });
                }
            }
            #endregion
            client.ClientUri = clientViewModel.ClientUri;
            await context.SaveChangesAsync();
        }
    }
}
