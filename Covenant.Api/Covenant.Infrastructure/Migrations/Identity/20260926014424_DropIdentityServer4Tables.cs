using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations.Identity
{
    /// <inheritdoc />
    public partial class DropIdentityServer4Tables : Migration
    {
        private static readonly string[] IdentityServer4Tables =
        [
            "ApiResourceClaims",
            "ApiResourceProperties",
            "ApiResourceScopes",
            "ApiResourceSecrets",
            "ApiResources",
            "ApiScopeClaims",
            "ApiScopeProperties",
            "ApiScopes",
            "ClientClaims",
            "ClientCorsOrigins",
            "ClientGrantTypes",
            "ClientIdPRestrictions",
            "ClientPostLogoutRedirectUris",
            "ClientProperties",
            "ClientRedirectUris",
            "ClientScopes",
            "ClientSecrets",
            "Clients",
            "IdentityResourceClaims",
            "IdentityResourceProperties",
            "IdentityResources",
            "DeviceCodes",
            "PersistedGrants",
            "DataProtectionKeys"
        ];

        private static readonly string[] IdentityServer4Migrations =
        [
            "20180720143541_InitialIdentityServerConfigurationDbMigration",
            "20230801225401_Net6Migration",
            "20260416000000_MigrateClientsToAuthCodePkce",
            "20260416010000_EnableOfflineAccessForWebClients",
            "20260708000000_ReuseRefreshTokensForWebClients",
            "20260820000000_AddPasswordGrantForNativeLogin",
            "20180720143521_InitialIdentityServerPersistedGrantDbMigration",
            "20230801225200_Net6Migration",
            "20191011135336_AddDataProtectionKeys",
            "20230801223345_Net6Migration"
        ];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var table in IdentityServer4Tables)
            {
                migrationBuilder.Sql($"""DROP TABLE IF EXISTS "{table}" CASCADE;""");
            }

            var migrationIds = string.Join(", ", IdentityServer4Migrations.Select(id => $"'{id}'"));
            migrationBuilder.Sql($"""DELETE FROM "__EFMigrationsHistory" WHERE "MigrationId" IN ({migrationIds});""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
