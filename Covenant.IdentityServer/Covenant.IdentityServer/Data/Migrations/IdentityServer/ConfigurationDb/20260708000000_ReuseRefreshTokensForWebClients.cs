using Microsoft.EntityFrameworkCore.Migrations;

namespace Covenant.IdentityServer.Data.Migrations.IdentityServer.ConfigurationDb
{
    public partial class ReuseRefreshTokensForWebClients : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE ""Clients""
                SET ""RefreshTokenUsage"" = 0
                WHERE ""ClientId"" IN ('all2job', 'sigook.com');
            ");

            migrationBuilder.Sql(@"
                INSERT INTO ""ClientPostLogoutRedirectUris"" (""PostLogoutRedirectUri"", ""ClientId"")
                SELECT REPLACE(u.""PostLogoutRedirectUri"", '/callback', '/'), u.""ClientId""
                FROM ""ClientPostLogoutRedirectUris"" u
                JOIN ""Clients"" c ON c.""Id"" = u.""ClientId""
                WHERE c.""ClientId"" IN ('all2job', 'sigook.com')
                  AND u.""PostLogoutRedirectUri"" LIKE '%/callback'
                  AND NOT EXISTS (
                      SELECT 1
                      FROM ""ClientPostLogoutRedirectUris"" e
                      WHERE e.""ClientId"" = u.""ClientId""
                        AND e.""PostLogoutRedirectUri"" = REPLACE(u.""PostLogoutRedirectUri"", '/callback', '/')
                  );
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE ""Clients""
                SET ""RefreshTokenUsage"" = 1
                WHERE ""ClientId"" IN ('all2job', 'sigook.com');
            ");

            migrationBuilder.Sql(@"
                DELETE FROM ""ClientPostLogoutRedirectUris"" u
                USING ""Clients"" c
                WHERE c.""Id"" = u.""ClientId""
                  AND c.""ClientId"" IN ('all2job', 'sigook.com')
                  AND u.""PostLogoutRedirectUri"" LIKE '%/'
                  AND EXISTS (
                      SELECT 1
                      FROM ""ClientPostLogoutRedirectUris"" e
                      WHERE e.""ClientId"" = u.""ClientId""
                        AND e.""PostLogoutRedirectUri"" = u.""PostLogoutRedirectUri"" || 'callback'
                  );
            ");
        }
    }
}
