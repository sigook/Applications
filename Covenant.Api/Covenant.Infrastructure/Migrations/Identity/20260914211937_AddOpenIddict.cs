using System;
using Microsoft.EntityFrameworkCore.Migrations;
using OpenIddict.Abstractions;

#nullable disable

namespace Covenant.Infrastructure.Migrations.Identity
{
    /// <inheritdoc />
    public partial class AddOpenIddict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OpenIddictApplications",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ApplicationType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ClientId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ClientSecret = table.Column<string>(type: "text", nullable: true),
                    ClientType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ConcurrencyToken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ConsentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    DisplayNames = table.Column<string>(type: "text", nullable: true),
                    JsonWebKeySet = table.Column<string>(type: "text", nullable: true),
                    Permissions = table.Column<string>(type: "text", nullable: true),
                    PostLogoutRedirectUris = table.Column<string>(type: "text", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    RedirectUris = table.Column<string>(type: "text", nullable: true),
                    Requirements = table.Column<string>(type: "text", nullable: true),
                    Settings = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIddictApplications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OpenIddictScopes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyToken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Descriptions = table.Column<string>(type: "text", nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    DisplayNames = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    Resources = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIddictScopes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OpenIddictAuthorizations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ApplicationId = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyToken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    Scopes = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIddictAuthorizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpenIddictAuthorizations_OpenIddictApplications_Application~",
                        column: x => x.ApplicationId,
                        principalTable: "OpenIddictApplications",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OpenIddictTokens",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ApplicationId = table.Column<string>(type: "text", nullable: true),
                    AuthorizationId = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyToken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Payload = table.Column<string>(type: "text", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    RedemptionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ReferenceId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    Type = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIddictTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpenIddictTokens_OpenIddictApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "OpenIddictApplications",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OpenIddictTokens_OpenIddictAuthorizations_AuthorizationId",
                        column: x => x.AuthorizationId,
                        principalTable: "OpenIddictAuthorizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictApplications_ClientId",
                table: "OpenIddictApplications",
                column: "ClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictAuthorizations_ApplicationId_Status_Subject_Type",
                table: "OpenIddictAuthorizations",
                columns: new[] { "ApplicationId", "Status", "Subject", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictScopes_Name",
                table: "OpenIddictScopes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictTokens_ApplicationId_Status_Subject_Type",
                table: "OpenIddictTokens",
                columns: new[] { "ApplicationId", "Status", "Subject", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictTokens_AuthorizationId",
                table: "OpenIddictTokens",
                column: "AuthorizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictTokens_ReferenceId",
                table: "OpenIddictTokens",
                column: "ReferenceId",
                unique: true);

            migrationBuilder.Sql(CopyIdentityServer4ClientsAndScopes);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpenIddictScopes");

            migrationBuilder.DropTable(
                name: "OpenIddictTokens");

            migrationBuilder.DropTable(
                name: "OpenIddictAuthorizations");

            migrationBuilder.DropTable(
                name: "OpenIddictApplications");
        }

        private static readonly string CopyIdentityServer4ClientsAndScopes = $$"""
            DO $$
            BEGIN
            IF to_regclass('"Clients"') IS NOT NULL THEN
                INSERT INTO "OpenIddictApplications"
                    ("Id", "ApplicationType", "ClientId", "ClientType", "ConcurrencyToken", "ConsentType", "DisplayName",
                     "Permissions", "PostLogoutRedirectUris", "RedirectUris", "Requirements", "Properties", "Settings")
                SELECT
                    gen_random_uuid()::text,
                    CASE WHEN EXISTS (SELECT 1 FROM "ClientRedirectUris" r WHERE r."ClientId" = c."Id" AND r."RedirectUri" NOT ILIKE 'http%')
                         THEN '{{OpenIddictConstants.ApplicationTypes.Native}}' ELSE '{{OpenIddictConstants.ApplicationTypes.Web}}' END,
                    c."ClientId",
                    '{{OpenIddictConstants.ClientTypes.Public}}',
                    gen_random_uuid()::text,
                    CASE WHEN c."RequireConsent" THEN '{{OpenIddictConstants.ConsentTypes.Explicit}}' ELSE '{{OpenIddictConstants.ConsentTypes.Implicit}}' END,
                    c."ClientName",
                    (SELECT json_agg(p)::text FROM (
                        SELECT '{{OpenIddictConstants.Permissions.Endpoints.Token}}' AS p
                        UNION ALL SELECT '{{OpenIddictConstants.Permissions.Endpoints.Revocation}}'
                        UNION ALL SELECT '{{OpenIddictConstants.Permissions.Endpoints.Authorization}}'
                            WHERE EXISTS (SELECT 1 FROM "ClientGrantTypes" g WHERE g."ClientId" = c."Id" AND g."GrantType" = '{{OpenIddictConstants.GrantTypes.AuthorizationCode}}')
                        UNION ALL SELECT '{{OpenIddictConstants.Permissions.Endpoints.EndSession}}'
                            WHERE EXISTS (SELECT 1 FROM "ClientGrantTypes" g WHERE g."ClientId" = c."Id" AND g."GrantType" = '{{OpenIddictConstants.GrantTypes.AuthorizationCode}}')
                        UNION ALL SELECT '{{OpenIddictConstants.Permissions.ResponseTypes.Code}}'
                            WHERE EXISTS (SELECT 1 FROM "ClientGrantTypes" g WHERE g."ClientId" = c."Id" AND g."GrantType" = '{{OpenIddictConstants.GrantTypes.AuthorizationCode}}')
                        UNION ALL SELECT '{{OpenIddictConstants.Permissions.Prefixes.GrantType}}' || g."GrantType"
                            FROM "ClientGrantTypes" g
                            WHERE g."ClientId" = c."Id"
                              AND g."GrantType" IN ('{{OpenIddictConstants.GrantTypes.AuthorizationCode}}', '{{OpenIddictConstants.GrantTypes.Password}}', '{{OpenIddictConstants.GrantTypes.ClientCredentials}}')
                        UNION ALL SELECT '{{OpenIddictConstants.Permissions.GrantTypes.RefreshToken}}' WHERE c."AllowOfflineAccess"
                        UNION ALL SELECT '{{OpenIddictConstants.Permissions.Prefixes.Scope}}' || s."Scope"
                            FROM "ClientScopes" s
                            WHERE s."ClientId" = c."Id"
                              AND s."Scope" NOT IN ('{{OpenIddictConstants.Scopes.OpenId}}', '{{OpenIddictConstants.Scopes.OfflineAccess}}')
                    ) permissions),
                    (SELECT json_agg(u."PostLogoutRedirectUri")::text FROM "ClientPostLogoutRedirectUris" u WHERE u."ClientId" = c."Id"),
                    (SELECT json_agg(u."RedirectUri")::text FROM "ClientRedirectUris" u WHERE u."ClientId" = c."Id"),
                    CASE WHEN c."RequirePkce" THEN '["{{OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange}}"]' END,
                    json_build_object('client_uri', c."ClientUri")::text,
                    json_build_object(
                        '{{OpenIddictConstants.Settings.TokenLifetimes.AccessToken}}',
                        (c."AccessTokenLifetime" / 86400)::text || '.' || to_char(make_interval(secs => c."AccessTokenLifetime" % 86400), 'HH24:MI:SS'),
                        '{{OpenIddictConstants.Settings.TokenLifetimes.RefreshToken}}',
                        (c."AbsoluteRefreshTokenLifetime" / 86400)::text || '.' || to_char(make_interval(secs => c."AbsoluteRefreshTokenLifetime" % 86400), 'HH24:MI:SS'))::text
                FROM "Clients" c
                WHERE c."Enabled"
                  AND c."RequireClientSecret" = false
                  AND NOT EXISTS (SELECT 1 FROM "OpenIddictApplications" a WHERE a."ClientId" = c."ClientId");

                INSERT INTO "OpenIddictScopes" ("Id", "ConcurrencyToken", "Description", "DisplayName", "Name", "Resources")
                SELECT
                    gen_random_uuid()::text,
                    gen_random_uuid()::text,
                    s."Description",
                    s."DisplayName",
                    s."Name",
                    (SELECT json_agg(ar."Name")::text
                     FROM "ApiResourceScopes" ars
                     JOIN "ApiResources" ar ON ar."Id" = ars."ApiResourceId"
                     WHERE ars."Scope" = s."Name")
                FROM "ApiScopes" s
                WHERE NOT EXISTS (SELECT 1 FROM "OpenIddictScopes" o WHERE o."Name" = s."Name");

                INSERT INTO "OpenIddictScopes" ("Id", "ConcurrencyToken", "Description", "DisplayName", "Name")
                SELECT
                    gen_random_uuid()::text,
                    gen_random_uuid()::text,
                    i."Description",
                    i."DisplayName",
                    i."Name"
                FROM "IdentityResources" i
                WHERE i."Name" NOT IN ('{{OpenIddictConstants.Scopes.OpenId}}', '{{OpenIddictConstants.Scopes.Profile}}', '{{OpenIddictConstants.Scopes.Email}}', '{{OpenIddictConstants.Scopes.Address}}', '{{OpenIddictConstants.Scopes.Phone}}')
                  AND NOT EXISTS (SELECT 1 FROM "OpenIddictScopes" o WHERE o."Name" = i."Name");
            END IF;
            END $$;
            """;
    }
}
