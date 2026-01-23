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
            // Step 1: Transform Status values directly
            // Old enum: Requested, InProcess, Cancelled (stored as strings)
            // New enum: Open, InProgress, Filled, Cancelled (stored as strings)
            migrationBuilder.Sql(@"
                UPDATE ""Request""
                SET ""Status"" =
                  CASE
                    -- Requested without workers → Open
                    WHEN ""Status"" = 'Requested' AND ""WorkersQuantityWorking"" = 0 THEN 'Open'

                    -- InProcess without workers → Open
                    WHEN ""Status"" = 'InProcess' AND ""WorkersQuantityWorking"" = 0 THEN 'Open'

                    -- InProcess with workers but not full → InProgress
                    WHEN ""Status"" = 'InProcess'
                      AND ""WorkersQuantityWorking"" > 0
                      AND ""WorkersQuantityWorking"" < ""WorkersQuantity"" THEN 'InProgress'

                    -- InProcess filled → Filled
                    WHEN ""Status"" = 'InProcess'
                      AND ""WorkersQuantityWorking"" >= ""WorkersQuantity"" THEN 'Filled'

                    -- Cancelled → Cancelled (no change)
                    WHEN ""Status"" = 'Cancelled' THEN 'Cancelled'

                    -- Edge case: if status was already 'Open'
                    WHEN ""Status"" = 'Open' THEN 'Open'

                    -- Any other case (shouldn't exist, keep current value)
                    ELSE ""Status""
                  END;
            ");

            // Step 2: Drop IsOpen column
            migrationBuilder.DropColumn(
                name: "IsOpen",
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
