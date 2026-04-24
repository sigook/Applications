using Microsoft.EntityFrameworkCore.Migrations;

namespace Covenant.IdentityServer.Data.Migrations.IdentityServer.ConfigurationDb
{
    public partial class EnableOfflineAccessForWebClients : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE ""Clients""
                SET ""AllowOfflineAccess"" = true
                WHERE ""ClientId"" IN ('all2job', 'sigook.com');
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE ""Clients""
                SET ""AllowOfflineAccess"" = false
                WHERE ""ClientId"" IN ('all2job', 'sigook.com');
            ");
        }
    }
}
