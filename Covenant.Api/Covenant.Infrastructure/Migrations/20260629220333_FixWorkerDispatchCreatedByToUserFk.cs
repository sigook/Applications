using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixWorkerDispatchCreatedByToUserFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""WorkerDispatch""
                ALTER COLUMN ""CreatedBy"" TYPE uuid
                USING CASE
                    WHEN ""CreatedBy"" ~ '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$'
                    THEN ""CreatedBy""::uuid
                    ELSE NULL
                END;");

            migrationBuilder.Sql(@"
                UPDATE ""WorkerDispatch""
                SET ""CreatedBy"" = NULL
                WHERE ""CreatedBy"" IS NOT NULL
                  AND ""CreatedBy"" NOT IN (SELECT ""Id"" FROM ""User"");");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerDispatch_CreatedBy",
                table: "WorkerDispatch",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerDispatch_User_CreatedBy",
                table: "WorkerDispatch",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkerDispatch_User_CreatedBy",
                table: "WorkerDispatch");

            migrationBuilder.DropIndex(
                name: "IX_WorkerDispatch_CreatedBy",
                table: "WorkerDispatch");

            migrationBuilder.Sql(@"
                ALTER TABLE ""WorkerDispatch""
                ALTER COLUMN ""CreatedBy"" TYPE text
                USING ""CreatedBy""::text;");
        }
    }
}
