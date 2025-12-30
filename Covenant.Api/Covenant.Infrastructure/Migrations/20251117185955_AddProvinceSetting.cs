using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    public partial class AddProvinceSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProvinceSettings",
                columns: table => new
                {
                    ProvinceId = table.Column<Guid>(type: "uuid", nullable: false),
                    PaidHolidays = table.Column<bool>(type: "boolean", nullable: true),
                    OvertimeStartsAfter = table.Column<TimeSpan>(type: "interval", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProvinceSettings", x => x.ProvinceId);
                    table.ForeignKey(
                        name: "FK_ProvinceSettings_Province_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Province",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProvinceSettings");
        }
    }
}
