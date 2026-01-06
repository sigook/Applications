using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    public partial class AgencyTypeColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMasterAgency",
                table: "Agency");

            migrationBuilder.AddColumn<byte>(
                name: "AgencyType",
                table: "Agency",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.Sql(
                @"
                UPDATE ""Agency""
                SET ""AgencyType"" = 1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgencyType",
                table: "Agency");

            migrationBuilder.AddColumn<bool>(
                name: "IsMasterAgency",
                table: "Agency",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
