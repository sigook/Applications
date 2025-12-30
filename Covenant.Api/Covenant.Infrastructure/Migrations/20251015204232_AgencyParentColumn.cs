using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    public partial class AgencyParentColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE ""Agency"" SET ""CityOfIncorporationId"" = NULL");

            migrationBuilder.DropForeignKey(
                name: "FK_Agency_City_CityOfIncorporationId",
                table: "Agency");

            migrationBuilder.DropColumn(
                name: "AgreeTermsAndConditions",
                table: "Agency");

            migrationBuilder.DropColumn(
                name: "BusinessName",
                table: "Agency");

            migrationBuilder.DropColumn(
                name: "DateOfIncorporation",
                table: "Agency");

            migrationBuilder.DropColumn(
                name: "Fax",
                table: "Agency");

            migrationBuilder.DropColumn(
                name: "FaxExt",
                table: "Agency");

            migrationBuilder.DropColumn(
                name: "Wsib",
                table: "Agency");

            migrationBuilder.RenameColumn(
                name: "CityOfIncorporationId",
                table: "Agency",
                newName: "AgencyParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Agency_CityOfIncorporationId",
                table: "Agency",
                newName: "IX_Agency_AgencyParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agency_Agency_AgencyParentId",
                table: "Agency",
                column: "AgencyParentId",
                principalTable: "Agency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agency_Agency_AgencyParentId",
                table: "Agency");

            migrationBuilder.RenameColumn(
                name: "AgencyParentId",
                table: "Agency",
                newName: "CityOfIncorporationId");

            migrationBuilder.RenameIndex(
                name: "IX_Agency_AgencyParentId",
                table: "Agency",
                newName: "IX_Agency_CityOfIncorporationId");

            migrationBuilder.AddColumn<bool>(
                name: "AgreeTermsAndConditions",
                table: "Agency",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "BusinessName",
                table: "Agency",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfIncorporation",
                table: "Agency",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Fax",
                table: "Agency",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FaxExt",
                table: "Agency",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Wsib",
                table: "Agency",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Agency_City_CityOfIncorporationId",
                table: "Agency",
                column: "CityOfIncorporationId",
                principalTable: "City",
                principalColumn: "Id");
        }
    }
}
