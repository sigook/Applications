using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoveHolidayToPayStubItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Insert PayStubItem "Statutory holiday" for each PayStub with PublicHolidayPay > 0
            migrationBuilder.Sql("""
                INSERT INTO "PayStubItem" ("Id", "Description", "Quantity", "UnitPrice", "Total", "Type", "PayStubId")
                SELECT gen_random_uuid(), 'Statutory holiday', 1, "PublicHolidayPay", "PublicHolidayPay", 10, "Id"
                FROM "PayStub"
                WHERE "PublicHolidayPay" > 0;
                """);

            // 2. Update GrossPayment to include the holiday amount
            migrationBuilder.Sql("""
                UPDATE "PayStub"
                SET "GrossPayment" = "GrossPayment" + "PublicHolidayPay"
                WHERE "PublicHolidayPay" > 0;
                """);

            // 3. Drop the view that depends on the column
            migrationBuilder.Sql("""DROP VIEW IF EXISTS "PayStubHistory";""");

            // 4. Drop the PublicHolidayPay column
            migrationBuilder.DropColumn(
                name: "PublicHolidayPay",
                table: "PayStub");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. Re-add the column
            migrationBuilder.AddColumn<decimal>(
                name: "PublicHolidayPay",
                table: "PayStub",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            // 2. Restore PublicHolidayPay from PayStubItem "Statutory holiday"
            migrationBuilder.Sql("""
                UPDATE "PayStub" ps
                SET "PublicHolidayPay" = COALESCE((
                    SELECT SUM(i."Total")
                    FROM "PayStubItem" i
                    WHERE i."PayStubId" = ps."Id" AND i."Type" = 10
                ), 0);
                """);

            // 3. Subtract holiday from GrossPayment
            migrationBuilder.Sql("""
                UPDATE "PayStub"
                SET "GrossPayment" = "GrossPayment" - "PublicHolidayPay"
                WHERE "PublicHolidayPay" > 0;
                """);

            // 4. Remove the Statutory holiday items
            migrationBuilder.Sql("""
                DELETE FROM "PayStubItem" WHERE "Type" = 10;
                """);
        }
    }
}
