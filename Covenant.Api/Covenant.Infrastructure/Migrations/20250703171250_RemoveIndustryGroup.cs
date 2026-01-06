using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    public partial class RemoveIndustryGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Industry_Industry_ParentIndustryId",
                table: "Industry");

            migrationBuilder.DropForeignKey(
                name: "FK_Industry_IndustryGroup_IndustryGroupId",
                table: "Industry");

            migrationBuilder.DropTable(
                name: "IndustryGroup");

            migrationBuilder.DropIndex(
                name: "IX_Industry_IndustryGroupId",
                table: "Industry");

            migrationBuilder.DropIndex(
                name: "IX_Industry_ParentIndustryId",
                table: "Industry");

            migrationBuilder.DropColumn(
                name: "IndustryGroupId",
                table: "Industry");

            migrationBuilder.DropColumn(
                name: "ParentIndustryId",
                table: "Industry");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IndustryGroupId",
                table: "Industry",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ParentIndustryId",
                table: "Industry",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IndustryGroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndustryGroup", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Industry_IndustryGroupId",
                table: "Industry",
                column: "IndustryGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Industry_ParentIndustryId",
                table: "Industry",
                column: "ParentIndustryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Industry_Industry_ParentIndustryId",
                table: "Industry",
                column: "ParentIndustryId",
                principalTable: "Industry",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Industry_IndustryGroup_IndustryGroupId",
                table: "Industry",
                column: "IndustryGroupId",
                principalTable: "IndustryGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
