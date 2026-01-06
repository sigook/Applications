using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    public partial class CppEiColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TimeSheetTotal_TimeSheetId",
                table: "TimeSheetTotal");

            migrationBuilder.AddColumn<decimal>(
                name: "Cpp",
                table: "WorkerProfileTaxCategories",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Ei",
                table: "WorkerProfileTaxCategories",
                type: "numeric",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PayrollNumber",
                table: "SkipPayrollNumber",
                type: "text",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSheetTotal_TimeSheetId",
                table: "TimeSheetTotal",
                column: "TimeSheetId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TimeSheetTotal_TimeSheetId",
                table: "TimeSheetTotal");

            migrationBuilder.DropColumn(
                name: "Cpp",
                table: "WorkerProfileTaxCategories");

            migrationBuilder.DropColumn(
                name: "Ei",
                table: "WorkerProfileTaxCategories");

            migrationBuilder.AlterColumn<long>(
                name: "PayrollNumber",
                table: "SkipPayrollNumber",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimeSheetTotal_TimeSheetId",
                table: "TimeSheetTotal",
                column: "TimeSheetId");
        }
    }
}
