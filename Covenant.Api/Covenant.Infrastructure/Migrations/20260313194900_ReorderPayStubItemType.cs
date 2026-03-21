using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReorderPayStubItemType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remap PayStubItemType enum values to match new display order.
            // Old values: Regular=0, Overtime=1, Missing=2, MissingOvertime=3, StatutoryWorkedHoliday=4,
            //             NightShift=5, Other=6, OtherRegular=7, Vacations=8, Reimbursement=9, StatutoryHoliday=10
            // New values: Regular=0, OtherRegular=1, Overtime=2, StatutoryHoliday=3, StatutoryWorkedHoliday=4,
            //             NightShift=5, Missing=6, MissingOvertime=7, Vacations=8, Other=9, Reimbursement=10

            // Step 1: Move conflicting values to temporary range (100+) to avoid collisions
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 101 WHERE "Type" = 1""");  // Overtime -> temp
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 102 WHERE "Type" = 2""");  // Missing -> temp
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 103 WHERE "Type" = 3""");  // MissingOvertime -> temp
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 106 WHERE "Type" = 6""");  // Other -> temp
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 107 WHERE "Type" = 7""");  // OtherRegular -> temp
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 109 WHERE "Type" = 9""");  // Reimbursement -> temp
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 110 WHERE "Type" = 10"""); // StatutoryHoliday -> temp

            // Step 2: Map from temporary values to final values
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 2 WHERE "Type" = 101""");  // Overtime: 1 -> 2
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 6 WHERE "Type" = 102""");  // Missing: 2 -> 6
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 7 WHERE "Type" = 103""");  // MissingOvertime: 3 -> 7
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 9 WHERE "Type" = 106""");  // Other: 6 -> 9
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 1 WHERE "Type" = 107""");  // OtherRegular: 7 -> 1
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 10 WHERE "Type" = 109"""); // Reimbursement: 9 -> 10
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 3 WHERE "Type" = 110""");  // StatutoryHoliday: 10 -> 3
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse the remap: restore original enum values
            // Step 1: Move to temporary range
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 101 WHERE "Type" = 1""");  // OtherRegular -> temp
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 102 WHERE "Type" = 2""");  // Overtime -> temp
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 103 WHERE "Type" = 3""");  // StatutoryHoliday -> temp
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 106 WHERE "Type" = 6""");  // Missing -> temp
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 107 WHERE "Type" = 7""");  // MissingOvertime -> temp
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 109 WHERE "Type" = 9""");  // Other -> temp
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 110 WHERE "Type" = 10"""); // Reimbursement -> temp

            // Step 2: Map back to original values
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 7 WHERE "Type" = 101""");  // OtherRegular: 1 -> 7
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 1 WHERE "Type" = 102""");  // Overtime: 2 -> 1
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 10 WHERE "Type" = 103"""); // StatutoryHoliday: 3 -> 10
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 2 WHERE "Type" = 106""");  // Missing: 6 -> 2
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 3 WHERE "Type" = 107""");  // MissingOvertime: 7 -> 3
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 6 WHERE "Type" = 109""");  // Other: 9 -> 6
            migrationBuilder.Sql("""UPDATE "PayStubItem" SET "Type" = 9 WHERE "Type" = 110""");  // Reimbursement: 10 -> 9
        }
    }
}
