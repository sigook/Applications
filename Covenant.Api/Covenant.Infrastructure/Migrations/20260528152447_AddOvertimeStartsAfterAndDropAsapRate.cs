using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOvertimeStartsAfterAndDropAsapRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AsapRate",
                table: "CompanyProfileJobPositionRate");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "OvertimeStartsAfter",
                table: "CompanyProfileJobPositionRate",
                type: "interval",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OvertimeStartsAfter",
                table: "CompanyProfileJobPositionRate");

            migrationBuilder.AddColumn<decimal>(
                name: "AsapRate",
                table: "CompanyProfileJobPositionRate",
                type: "numeric",
                nullable: true);
        }
    }
}
