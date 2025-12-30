using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    public partial class RemoveRoundingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyTimeSheetRoundingConfiguration");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyTimeSheetRoundingConfiguration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoundTo = table.Column<int>(type: "integer", nullable: true),
                    RoundToShift = table.Column<bool>(type: "boolean", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyTimeSheetRoundingConfiguration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyTimeSheetRoundingConfiguration_User_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyTimeSheetRoundingConfiguration_CompanyId",
                table: "CompanyTimeSheetRoundingConfiguration",
                column: "CompanyId",
                unique: true);
        }
    }
}
