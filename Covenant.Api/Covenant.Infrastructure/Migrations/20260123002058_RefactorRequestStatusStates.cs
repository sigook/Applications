using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorRequestStatusStates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add temporary column to preserve old status
            migrationBuilder.AddColumn<int>(
                name: "OldStatus",
                table: "Request",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Step 2: Copy current status to OldStatus
            migrationBuilder.Sql(@"UPDATE ""Request"" SET ""OldStatus"" = ""Status"";");

            // Step 3: Migrate data to new status values
            // Old enum: Requested=0, InProcess=1, Cancelled=2
            // New enum: Open=1, InProgress=2, Filled=3, Cancelled=4
            migrationBuilder.Sql(@"
                UPDATE ""Request""
                SET ""Status"" =
                  CASE
                    -- Requested (0) without workers → Open (1)
                    WHEN ""OldStatus"" = 0 AND ""WorkersQuantityWorking"" = 0 THEN 1

                    -- InProcess (1) without workers → Open (1)
                    WHEN ""OldStatus"" = 1 AND ""WorkersQuantityWorking"" = 0 THEN 1

                    -- InProcess (1) with workers but not full → InProgress (2)
                    WHEN ""OldStatus"" = 1
                      AND ""WorkersQuantityWorking"" > 0
                      AND ""WorkersQuantityWorking"" < ""WorkersQuantity"" THEN 2

                    -- InProcess (1) filled → Filled (3)
                    WHEN ""OldStatus"" = 1
                      AND ""WorkersQuantityWorking"" >= ""WorkersQuantity"" THEN 3

                    -- Cancelled (2) → Cancelled (4)
                    WHEN ""OldStatus"" = 2 THEN 4

                    -- Any other case (shouldn't exist)
                    ELSE ""Status""
                  END;
            ");

            // Step 4: Drop IsOpen column
            migrationBuilder.DropColumn(
                name: "IsOpen",
                table: "Request");

            // Step 5: Drop temporary OldStatus column
            migrationBuilder.DropColumn(
                name: "OldStatus",
                table: "Request");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOpen",
                table: "Request",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
