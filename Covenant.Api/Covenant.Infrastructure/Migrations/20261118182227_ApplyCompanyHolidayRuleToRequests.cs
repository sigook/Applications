using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ApplyCompanyHolidayRuleToRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE "Requests" r
                SET "HolidayIsPaid" = FALSE
                FROM "CompanyProfiles" cp
                WHERE cp."Id" = r."CompanyProfileId"
                  AND cp."PaidHolidays" = FALSE
                  AND r."HolidayIsPaid" = TRUE;
                """);

            migrationBuilder.DropColumn(
                name: "PaidHolidays",
                table: "ProvinceSettings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PaidHolidays",
                table: "ProvinceSettings",
                type: "boolean",
                nullable: true);
        }
    }
}
