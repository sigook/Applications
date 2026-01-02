using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Covenant.IdentityServer.Migrations
{
    public partial class CreateAtColumnToInactiveUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "InactiveUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "InactiveUsers");
        }
    }
}
