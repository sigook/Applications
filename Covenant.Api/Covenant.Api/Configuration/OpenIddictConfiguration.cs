using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Covenant.Common.Configuration;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using System.Security.Cryptography.X509Certificates;

namespace Covenant.Api.Configuration;

public static class OpenIddictConfiguration
{
    public const string ApiScope = "api1";
    public const string AuthorizeEndpoint = "connect/authorize";
    public const string TokenEndpoint = "connect/token";
    public const string UserInfoEndpoint = "connect/userinfo";
    public const string EndSessionEndpoint = "connect/endsession";
    public const string RevocationEndpoint = "connect/revocation";

    public static IServiceCollection AddCovenantOpenIddict(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var identity = configuration.GetSection(IdentityConfiguration.SectionName).Get<IdentityConfiguration>() ?? new IdentityConfiguration();

        services.AddOpenIddict()
            .AddServer(options =>
            {
                if (!string.IsNullOrEmpty(identity.IssuerUri))
                {
                    options.SetIssuer(new Uri(identity.IssuerUri));
                }

                options.SetAuthorizationEndpointUris(AuthorizeEndpoint)
                    .SetTokenEndpointUris(TokenEndpoint)
                    .SetUserInfoEndpointUris(UserInfoEndpoint)
                    .SetEndSessionEndpointUris(EndSessionEndpoint)
                    .SetRevocationEndpointUris(RevocationEndpoint);

                options.AllowAuthorizationCodeFlow()
                    .AllowPasswordFlow()
                    .AllowRefreshTokenFlow()
                    .AllowClientCredentialsFlow();

                options.RegisterScopes(
                    OpenIddictConstants.Scopes.OpenId,
                    OpenIddictConstants.Scopes.Profile,
                    OpenIddictConstants.Scopes.Email,
                    OpenIddictConstants.Scopes.Roles,
                    OpenIddictConstants.Scopes.OfflineAccess,
                    ApiScope);

                options.SetAccessTokenLifetime(TimeSpan.FromHours(1))
                    .SetRefreshTokenLifetime(TimeSpan.FromDays(30))
                    .DisableRollingRefreshTokens()
                    .DisableAccessTokenEncryption();

                if (environment.IsDevelopment())
                {
                    options.AddDevelopmentEncryptionCertificate()
                        .AddDevelopmentSigningCertificate();
                }
                else
                {
                    var vaultUrl = configuration["KeyVault:Url"];
                    options.Configure(serverOptions =>
                    {
                        serverOptions.SigningCredentials.Add(new SigningCredentials(
                            new X509SecurityKey(LoadCertificate(vaultUrl, identity.SigningCertificateName)),
                            SecurityAlgorithms.RsaSha256));
                        serverOptions.EncryptionCredentials.Add(new EncryptingCredentials(
                            new X509SecurityKey(LoadCertificate(vaultUrl, identity.EncryptionCertificateName)),
                            SecurityAlgorithms.RsaOAEP,
                            SecurityAlgorithms.Aes256CbcHmacSha512));
                    });
                }

                options.UseAspNetCore()
                    .EnableAuthorizationEndpointPassthrough()
                    .EnableTokenEndpointPassthrough()
                    .EnableUserInfoEndpointPassthrough()
                    .EnableEndSessionEndpointPassthrough()
                    .DisableTransportSecurityRequirement();
            })
            .AddValidation(options =>
            {
                options.AddAudiences(ApiScope);
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        return services;
    }

    private static X509Certificate2 LoadCertificate(string vaultUrl, string certificateName)
    {
        if (string.IsNullOrEmpty(vaultUrl) || string.IsNullOrEmpty(certificateName))
        {
            throw new InvalidOperationException("KeyVault:Url and the Identity certificate names are required outside Development.");
        }
        var client = new CertificateClient(new Uri(vaultUrl), new DefaultAzureCredential());
        return client.DownloadCertificate(certificateName).Value;
    }
}
