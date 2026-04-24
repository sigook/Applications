using Microsoft.EntityFrameworkCore.Migrations;

namespace Covenant.IdentityServer.Data.Migrations.IdentityServer.ConfigurationDb
{
    public partial class MigrateClientsToAuthCodePkce : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE ""ClientGrantTypes""
                SET ""GrantType"" = 'authorization_code'
                FROM ""Clients""
                WHERE ""ClientGrantTypes"".""ClientId"" = ""Clients"".""Id""
                  AND ""Clients"".""ClientId"" IN ('all2job', 'sigook.com')
                  AND ""ClientGrantTypes"".""GrantType"" = 'implicit';
            ");

            migrationBuilder.Sql(@"
                UPDATE ""Clients""
                SET ""RequireClientSecret"" = false,
                    ""RequirePkce"" = true
                WHERE ""ClientId"" IN ('all2job', 'sigook.com');
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE ""ClientGrantTypes""
                SET ""GrantType"" = 'implicit'
                FROM ""Clients""
                WHERE ""ClientGrantTypes"".""ClientId"" = ""Clients"".""Id""
                  AND ""Clients"".""ClientId"" IN ('all2job', 'sigook.com')
                  AND ""ClientGrantTypes"".""GrantType"" = 'authorization_code';
            ");

            migrationBuilder.Sql(@"
                UPDATE ""Clients""
                SET ""RequireClientSecret"" = true,
                    ""RequirePkce"" = true
                WHERE ""ClientId"" IN ('all2job', 'sigook.com');
            ");
        }
    }
}
