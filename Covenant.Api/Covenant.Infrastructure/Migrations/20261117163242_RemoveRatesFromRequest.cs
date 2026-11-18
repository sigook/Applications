using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRatesFromRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgencyRate",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "WorkerRate",
                table: "Requests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AgencyRate",
                table: "Requests",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WorkerRate",
                table: "Requests",
                type: "numeric",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "Requests" r
                SET "AgencyRate" = j."Rate", "WorkerRate" = j."WorkerRate"
                FROM "CompanyProfileJobPositionRates" j
                WHERE j."Id" = r."JobPositionRateId" AND r."WorkerSalary" IS NULL
                """);
        }
    }
}
