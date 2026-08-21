using Microsoft.EntityFrameworkCore.Migrations;

namespace Covenant.IdentityServer.Data.Migrations.IdentityServer.ConfigurationDb
{
    public partial class AddPasswordGrantForNativeLogin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO ""ClientGrantTypes"" (""GrantType"", ""ClientId"")
                SELECT 'password', c.""Id""
                FROM ""Clients"" c
                WHERE c.""ClientId"" IN ('all2job', 'sigook.com')
                  AND NOT EXISTS (
                      SELECT 1 FROM ""ClientGrantTypes"" g
                      WHERE g.""ClientId"" = c.""Id""
                        AND g.""GrantType"" = 'password');
            ");

            migrationBuilder.Sql(@"
                INSERT INTO ""ClientGrantTypes"" (""GrantType"", ""ClientId"")
                SELECT 'password', c.""Id""
                FROM ""Clients"" c
                WHERE (c.""ClientId"" IN ('android', 'ios')
                       OR EXISTS (
                           SELECT 1 FROM ""ClientRedirectUris"" r
                           WHERE r.""ClientId"" = c.""Id""
                             AND (r.""RedirectUri"" LIKE 'com.sigook:%' OR r.""RedirectUri"" LIKE 'com.all2job:%')))
                  AND NOT EXISTS (
                      SELECT 1 FROM ""ClientGrantTypes"" g
                      WHERE g.""ClientId"" = c.""Id""
                        AND g.""GrantType"" = 'password');
            ");

            migrationBuilder.Sql(@"
                UPDATE ""Clients""
                SET ""AllowOfflineAccess"" = true
                WHERE ""ClientId"" IN ('android', 'ios')
                   OR EXISTS (
                      SELECT 1 FROM ""ClientRedirectUris"" r
                      WHERE r.""ClientId"" = ""Clients"".""Id""
                        AND (r.""RedirectUri"" LIKE 'com.sigook:%' OR r.""RedirectUri"" LIKE 'com.all2job:%'));
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM ""ClientGrantTypes"" g
                USING ""Clients"" c
                WHERE g.""ClientId"" = c.""Id""
                  AND g.""GrantType"" = 'password'
                  AND (c.""ClientId"" IN ('all2job', 'sigook.com', 'android', 'ios')
                       OR EXISTS (
                           SELECT 1 FROM ""ClientRedirectUris"" r
                           WHERE r.""ClientId"" = c.""Id""
                             AND (r.""RedirectUri"" LIKE 'com.sigook:%' OR r.""RedirectUri"" LIKE 'com.all2job:%')));
            ");
        }
    }
}
