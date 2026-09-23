using Covenant.Common.Configuration;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Covenant.Api.Configuration;

public class OpenIddictSeeder(
    IOpenIddictApplicationManager applications,
    IOpenIddictScopeManager scopes,
    IOptions<IdentityConfiguration> options,
    IHostEnvironment environment,
    ILogger<OpenIddictSeeder> logger)
{
    private const string WebClientId = "sigook.com";
    private const string AndroidClientId = "android";
    private const string DevelopmentWebClientUrl = "http://localhost:3001";

    public async Task Seed()
    {
        await EnsureScope(OpenIddictConfiguration.ApiScope, "Sigook API", [OpenIddictConfiguration.ApiScope]);
        await EnsureScope(Scopes.Roles, "Your role(s)", []);
        await EnsureFunctionsClient();
        if (environment.IsDevelopment())
        {
            await EnsureDevelopmentClients();
        }
    }

    private async Task EnsureScope(string name, string displayName, string[] resources)
    {
        if (await scopes.FindByNameAsync(name) is not null) return;
        var descriptor = new OpenIddictScopeDescriptor { Name = name, DisplayName = displayName };
        descriptor.Resources.UnionWith(resources);
        await scopes.CreateAsync(descriptor);
        logger.LogInformation("OpenIddict scope created: {Scope}", name);
    }

    private async Task EnsureFunctionsClient()
    {
        var identity = options.Value;
        if (string.IsNullOrEmpty(identity.FunctionsClientId) || string.IsNullOrEmpty(identity.FunctionsClientSecret))
        {
            logger.LogWarning("Identity:FunctionsClientId/FunctionsClientSecret are missing. The Sigook.Functions client was not seeded.");
            return;
        }

        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = identity.FunctionsClientId,
            ClientSecret = identity.FunctionsClientSecret,
            ClientType = ClientTypes.Confidential,
            DisplayName = "Sigook Functions",
            Permissions =
            {
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.ClientCredentials,
                Permissions.Prefixes.Scope + OpenIddictConfiguration.ApiScope
            }
        };
        await CreateOrUpdate(descriptor);
    }

    private async Task EnsureDevelopmentClients()
    {
        await CreateIfMissing(new OpenIddictApplicationDescriptor
        {
            ClientId = WebClientId,
            ClientType = ClientTypes.Public,
            ApplicationType = ApplicationTypes.Web,
            ConsentType = ConsentTypes.Implicit,
            DisplayName = "Sigook Web",
            RedirectUris = { new Uri($"{DevelopmentWebClientUrl}/callback"), new Uri($"{DevelopmentWebClientUrl}/silent-refresh") },
            PostLogoutRedirectUris = { new Uri($"{DevelopmentWebClientUrl}/"), new Uri($"{DevelopmentWebClientUrl}/callback") },
            Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.Endpoints.EndSession,
                Permissions.Endpoints.Revocation,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.Password,
                Permissions.GrantTypes.RefreshToken,
                Permissions.ResponseTypes.Code,
                Permissions.Prefixes.Scope + Scopes.Profile,
                Permissions.Prefixes.Scope + Scopes.Email,
                Permissions.Prefixes.Scope + Scopes.Roles,
                Permissions.Prefixes.Scope + OpenIddictConfiguration.ApiScope
            },
            Requirements = { Requirements.Features.ProofKeyForCodeExchange }
        });

        await CreateIfMissing(new OpenIddictApplicationDescriptor
        {
            ClientId = AndroidClientId,
            ClientType = ClientTypes.Public,
            ApplicationType = ApplicationTypes.Native,
            ConsentType = ConsentTypes.Implicit,
            DisplayName = "Sigook App",
            RedirectUris = { new Uri("com.sigook:/callback") },
            Permissions =
            {
                Permissions.Endpoints.Token,
                Permissions.Endpoints.Revocation,
                Permissions.GrantTypes.Password,
                Permissions.GrantTypes.RefreshToken,
                Permissions.Prefixes.Scope + Scopes.Profile,
                Permissions.Prefixes.Scope + Scopes.Email,
                Permissions.Prefixes.Scope + Scopes.Roles,
                Permissions.Prefixes.Scope + OpenIddictConfiguration.ApiScope
            }
        });
    }

    private async Task CreateOrUpdate(OpenIddictApplicationDescriptor descriptor)
    {
        var existing = await applications.FindByClientIdAsync(descriptor.ClientId);
        if (existing is null)
        {
            await Create(descriptor);
            return;
        }
        await applications.UpdateAsync(existing, descriptor);
    }

    private async Task CreateIfMissing(OpenIddictApplicationDescriptor descriptor)
    {
        if (await applications.FindByClientIdAsync(descriptor.ClientId) is not null) return;
        await Create(descriptor);
    }

    private async Task Create(OpenIddictApplicationDescriptor descriptor)
    {
        await applications.CreateAsync(descriptor);
        logger.LogInformation("OpenIddict client created: {ClientId}", descriptor.ClientId);
    }
}
