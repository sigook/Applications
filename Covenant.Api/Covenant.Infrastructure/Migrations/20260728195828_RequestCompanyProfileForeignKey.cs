using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RequestCompanyProfileForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_User_CompanyId",
                table: "Request");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "Request",
                newName: "CompanyProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Request_CompanyId",
                table: "Request",
                newName: "IX_Request_CompanyProfileId");

            migrationBuilder.Sql("""
                UPDATE "Request" r
                SET "CompanyProfileId" = cp."Id"
                FROM "CompanyProfile" cp
                WHERE cp."CompanyId" = r."CompanyProfileId"
                  AND cp."AgencyId" = r."AgencyId";
                """);

            migrationBuilder.Sql("""
                DO $$
                DECLARE orphans bigint;
                BEGIN
                    SELECT COUNT(*) INTO orphans
                    FROM "Request" r
                    WHERE NOT EXISTS (
                        SELECT 1 FROM "CompanyProfile" cp WHERE cp."Id" = r."CompanyProfileId");
                    IF orphans > 0 THEN
                        RAISE EXCEPTION
                            'Request.CompanyId -> CompanyProfileId migration aborted: % row(s) have no CompanyProfile matching (CompanyId, AgencyId)', orphans;
                    END IF;
                END $$;
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_CompanyProfile_CompanyProfileId",
                table: "Request",
                column: "CompanyProfileId",
                principalTable: "CompanyProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_CompanyProfile_CompanyProfileId",
                table: "Request");

            migrationBuilder.RenameColumn(
                name: "CompanyProfileId",
                table: "Request",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Request_CompanyProfileId",
                table: "Request",
                newName: "IX_Request_CompanyId");

            migrationBuilder.Sql("""
                UPDATE "Request" r
                SET "CompanyId" = cp."CompanyId"
                FROM "CompanyProfile" cp
                WHERE cp."Id" = r."CompanyId";
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_User_CompanyId",
                table: "Request",
                column: "CompanyId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
