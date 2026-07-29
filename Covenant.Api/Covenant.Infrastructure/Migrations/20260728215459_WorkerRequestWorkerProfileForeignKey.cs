using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class WorkerRequestWorkerProfileForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkerRequest_User_WorkerId",
                table: "WorkerRequest");

            migrationBuilder.RenameColumn(
                name: "WorkerId",
                table: "WorkerRequest",
                newName: "WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerRequest_WorkerId",
                table: "WorkerRequest",
                newName: "IX_WorkerRequest_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerRequest_RequestId_WorkerId",
                table: "WorkerRequest",
                newName: "IX_WorkerRequest_RequestId_WorkerProfileId");

            migrationBuilder.Sql("""
                UPDATE "WorkerRequest" wr
                SET "WorkerProfileId" = wp."Id"
                FROM "WorkerProfile" wp, "Request" r
                WHERE r."Id" = wr."RequestId"
                  AND wp."WorkerId" = wr."WorkerProfileId"
                  AND wp."AgencyId" = r."AgencyId";
                """);

            migrationBuilder.Sql("""
                DO $$
                DECLARE orphans bigint;
                BEGIN
                    SELECT COUNT(*) INTO orphans
                    FROM "WorkerRequest" wr
                    WHERE NOT EXISTS (
                        SELECT 1 FROM "WorkerProfile" wp WHERE wp."Id" = wr."WorkerProfileId");
                    IF orphans > 0 THEN
                        RAISE EXCEPTION
                            'WorkerRequest.WorkerId -> WorkerProfileId migration aborted: % row(s) have no WorkerProfile matching (WorkerId, Request.AgencyId)', orphans;
                    END IF;
                END $$;
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerRequest_WorkerProfile_WorkerProfileId",
                table: "WorkerRequest",
                column: "WorkerProfileId",
                principalTable: "WorkerProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkerRequest_WorkerProfile_WorkerProfileId",
                table: "WorkerRequest");

            migrationBuilder.RenameColumn(
                name: "WorkerProfileId",
                table: "WorkerRequest",
                newName: "WorkerId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerRequest_WorkerProfileId",
                table: "WorkerRequest",
                newName: "IX_WorkerRequest_WorkerId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerRequest_RequestId_WorkerProfileId",
                table: "WorkerRequest",
                newName: "IX_WorkerRequest_RequestId_WorkerId");

            migrationBuilder.Sql("""
                UPDATE "WorkerRequest" wr
                SET "WorkerId" = wp."WorkerId"
                FROM "WorkerProfile" wp
                WHERE wp."Id" = wr."WorkerId";
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerRequest_User_WorkerId",
                table: "WorkerRequest",
                column: "WorkerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
