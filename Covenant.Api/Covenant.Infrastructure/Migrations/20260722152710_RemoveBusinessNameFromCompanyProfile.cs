using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBusinessNameFromCompanyProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE "CompanyProfile"
                SET "FullName" = "BusinessName"
                WHERE "BusinessName" IS NOT NULL AND "BusinessName" <> '';
                """);

            migrationBuilder.DropColumn(
                name: "BusinessName",
                table: "CompanyProfile");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BusinessName",
                table: "CompanyProfile",
                type: "text",
                nullable: true);
        }
    }
}
