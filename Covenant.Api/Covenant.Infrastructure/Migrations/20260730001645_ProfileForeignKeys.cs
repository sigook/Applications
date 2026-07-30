using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProfileForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyUser_User_CompanyId",
                table: "CompanyUser");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_CompanyProfile_CompanyId",
                table: "Invoice");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_Agency_AgencyId",
                table: "Request");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_User_CompanyId",
                table: "Request");

            migrationBuilder.DropForeignKey(
                name: "FK_Runners_Agency_AgencyId",
                table: "Runners");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerComment_Agency_AgencyId",
                table: "WorkerComment");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerComment_User_CompanyId",
                table: "WorkerComment");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerComment_User_WorkerId",
                table: "WorkerComment");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerRequest_User_WorkerId",
                table: "WorkerRequest");

            migrationBuilder.DropIndex(
                name: "IX_WorkerProfile_WorkerId_AgencyId",
                table: "WorkerProfile");

            migrationBuilder.DropIndex(
                name: "IX_WorkerComment_AgencyId",
                table: "WorkerComment");

            migrationBuilder.DropIndex(
                name: "IX_Runners_AgencyId",
                table: "Runners");

            migrationBuilder.DropIndex(
                name: "IX_Request_AgencyId",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_CompanyProfile_CompanyId_AgencyId",
                table: "CompanyProfile");

            migrationBuilder.DropColumn(
                name: "AgencyId",
                table: "WorkerComment");

            migrationBuilder.DropColumn(
                name: "AgencyId",
                table: "Runners");

            migrationBuilder.DropColumn(
                name: "AgencyId",
                table: "Request");

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

            migrationBuilder.RenameColumn(
                name: "WorkerId",
                table: "WorkerComment",
                newName: "WorkerProfileId");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "WorkerComment",
                newName: "CompanyProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerComment_WorkerId",
                table: "WorkerComment",
                newName: "IX_WorkerComment_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerComment_CompanyId",
                table: "WorkerComment",
                newName: "IX_WorkerComment_CompanyProfileId");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "Request",
                newName: "CompanyProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Request_CompanyId",
                table: "Request",
                newName: "IX_Request_CompanyProfileId");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "Invoice",
                newName: "CompanyProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Invoice_CompanyId",
                table: "Invoice",
                newName: "IX_Invoice_CompanyProfileId");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "CompanyUser",
                newName: "CompanyProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyUser_CompanyId_UserId",
                table: "CompanyUser",
                newName: "IX_CompanyUser_CompanyProfileId_UserId");

            migrationBuilder.Sql("""
                DO $$
                DECLARE duplicates bigint;
                BEGIN
                    SELECT COUNT(*) INTO duplicates FROM (
                        SELECT "WorkerId" FROM "WorkerProfile" GROUP BY "WorkerId" HAVING COUNT(*) > 1) d;
                    IF duplicates > 0 THEN
                        RAISE EXCEPTION
                            'One profile per user migration aborted: % worker(s) hold a profile in more than one agency', duplicates;
                    END IF;
                    SELECT COUNT(*) INTO duplicates FROM (
                        SELECT "CompanyId" FROM "CompanyProfile" GROUP BY "CompanyId" HAVING COUNT(*) > 1) d;
                    IF duplicates > 0 THEN
                        RAISE EXCEPTION
                            'One profile per user migration aborted: % company(ies) hold a profile in more than one agency', duplicates;
                    END IF;
                END $$;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_WorkerProfile_WorkerId",
                table: "WorkerProfile",
                column: "WorkerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Runners_CreatedBy",
                table: "Runners",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Runners_UpdatedBy",
                table: "Runners",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RunnerInterviews_CreatedBy",
                table: "RunnerInterviews",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RunnerInterviews_RescheduledBy",
                table: "RunnerInterviews",
                column: "RescheduledBy");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfile_CompanyId",
                table: "CompanyProfile",
                column: "CompanyId",
                unique: true);

            migrationBuilder.Sql("""
                UPDATE "Request" r
                SET "CompanyProfileId" = cp."Id"
                FROM "CompanyProfile" cp
                WHERE cp."CompanyId" = r."CompanyProfileId";
                """);

            migrationBuilder.Sql("""
                UPDATE "WorkerRequest" wr
                SET "WorkerProfileId" = wp."Id"
                FROM "WorkerProfile" wp
                WHERE wp."WorkerId" = wr."WorkerProfileId";
                """);

            migrationBuilder.Sql("""
                UPDATE "WorkerComment" wc
                SET "WorkerProfileId" = wp."Id"
                FROM "WorkerProfile" wp
                WHERE wp."WorkerId" = wc."WorkerProfileId";
                """);

            migrationBuilder.Sql("""
                UPDATE "WorkerComment" wc
                SET "CompanyProfileId" = cp."Id"
                FROM "CompanyProfile" cp
                WHERE wc."CompanyProfileId" IS NOT NULL
                  AND cp."CompanyId" = wc."CompanyProfileId";
                """);

            migrationBuilder.Sql("""
                UPDATE "CompanyUser" cu
                SET "CompanyProfileId" = cp."Id"
                FROM "CompanyProfile" cp
                WHERE cp."CompanyId" = cu."CompanyProfileId";
                """);

            migrationBuilder.Sql("""
                DO $$
                DECLARE orphans bigint;
                BEGIN
                    SELECT COUNT(*) INTO orphans FROM "Request" r
                    WHERE NOT EXISTS (SELECT 1 FROM "CompanyProfile" cp WHERE cp."Id" = r."CompanyProfileId");
                    IF orphans > 0 THEN
                        RAISE EXCEPTION
                            'Request.CompanyId -> CompanyProfileId aborted: % row(s) reference a company with no CompanyProfile', orphans;
                    END IF;

                    SELECT COUNT(*) INTO orphans FROM "WorkerRequest" wr
                    WHERE NOT EXISTS (SELECT 1 FROM "WorkerProfile" wp WHERE wp."Id" = wr."WorkerProfileId");
                    IF orphans > 0 THEN
                        RAISE EXCEPTION
                            'WorkerRequest.WorkerId -> WorkerProfileId aborted: % row(s) reference a worker with no WorkerProfile', orphans;
                    END IF;

                    SELECT COUNT(*) INTO orphans FROM "WorkerComment" wc
                    WHERE NOT EXISTS (SELECT 1 FROM "WorkerProfile" wp WHERE wp."Id" = wc."WorkerProfileId");
                    IF orphans > 0 THEN
                        RAISE EXCEPTION
                            'WorkerComment.WorkerId -> WorkerProfileId aborted: % row(s) reference a worker with no WorkerProfile', orphans;
                    END IF;

                    SELECT COUNT(*) INTO orphans FROM "WorkerComment" wc
                    WHERE wc."CompanyProfileId" IS NOT NULL
                      AND NOT EXISTS (SELECT 1 FROM "CompanyProfile" cp WHERE cp."Id" = wc."CompanyProfileId");
                    IF orphans > 0 THEN
                        RAISE EXCEPTION
                            'WorkerComment.CompanyId -> CompanyProfileId aborted: % row(s) reference a company with no CompanyProfile', orphans;
                    END IF;

                    SELECT COUNT(*) INTO orphans FROM "CompanyUser" cu
                    WHERE NOT EXISTS (SELECT 1 FROM "CompanyProfile" cp WHERE cp."Id" = cu."CompanyProfileId");
                    IF orphans > 0 THEN
                        RAISE EXCEPTION
                            'CompanyUser.CompanyId -> CompanyProfileId aborted: % row(s) reference a company with no CompanyProfile', orphans;
                    END IF;

                    SELECT COUNT(*) INTO orphans FROM "Runners" r
                    WHERE NOT EXISTS (SELECT 1 FROM "User" u WHERE u."Id" = r."CreatedBy")
                       OR (r."UpdatedBy" IS NOT NULL
                           AND NOT EXISTS (SELECT 1 FROM "User" u WHERE u."Id" = r."UpdatedBy"));
                    IF orphans > 0 THEN
                        RAISE EXCEPTION
                            'Runner audit foreign keys aborted: % runner(s) reference a user that no longer exists', orphans;
                    END IF;

                    SELECT COUNT(*) INTO orphans FROM "RunnerInterviews" i
                    WHERE NOT EXISTS (SELECT 1 FROM "User" u WHERE u."Id" = i."CreatedBy")
                       OR (i."RescheduledBy" IS NOT NULL
                           AND NOT EXISTS (SELECT 1 FROM "User" u WHERE u."Id" = i."RescheduledBy"));
                    IF orphans > 0 THEN
                        RAISE EXCEPTION
                            'RunnerInterview audit foreign keys aborted: % interview(s) reference a user that no longer exists', orphans;
                    END IF;
                END $$;
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyUser_CompanyProfile_CompanyProfileId",
                table: "CompanyUser",
                column: "CompanyProfileId",
                principalTable: "CompanyProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_CompanyProfile_CompanyProfileId",
                table: "Invoice",
                column: "CompanyProfileId",
                principalTable: "CompanyProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_CompanyProfile_CompanyProfileId",
                table: "Request",
                column: "CompanyProfileId",
                principalTable: "CompanyProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RunnerInterviews_User_CreatedBy",
                table: "RunnerInterviews",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RunnerInterviews_User_RescheduledBy",
                table: "RunnerInterviews",
                column: "RescheduledBy",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Runners_User_CreatedBy",
                table: "Runners",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Runners_User_UpdatedBy",
                table: "Runners",
                column: "UpdatedBy",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerComment_CompanyProfile_CompanyProfileId",
                table: "WorkerComment",
                column: "CompanyProfileId",
                principalTable: "CompanyProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerComment_WorkerProfile_WorkerProfileId",
                table: "WorkerComment",
                column: "WorkerProfileId",
                principalTable: "WorkerProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_CompanyUser_CompanyProfile_CompanyProfileId",
                table: "CompanyUser");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_CompanyProfile_CompanyProfileId",
                table: "Invoice");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_CompanyProfile_CompanyProfileId",
                table: "Request");

            migrationBuilder.DropForeignKey(
                name: "FK_RunnerInterviews_User_CreatedBy",
                table: "RunnerInterviews");

            migrationBuilder.DropForeignKey(
                name: "FK_RunnerInterviews_User_RescheduledBy",
                table: "RunnerInterviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Runners_User_CreatedBy",
                table: "Runners");

            migrationBuilder.DropForeignKey(
                name: "FK_Runners_User_UpdatedBy",
                table: "Runners");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerComment_CompanyProfile_CompanyProfileId",
                table: "WorkerComment");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerComment_WorkerProfile_WorkerProfileId",
                table: "WorkerComment");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerRequest_WorkerProfile_WorkerProfileId",
                table: "WorkerRequest");

            migrationBuilder.DropIndex(
                name: "IX_WorkerProfile_WorkerId",
                table: "WorkerProfile");

            migrationBuilder.DropIndex(
                name: "IX_Runners_CreatedBy",
                table: "Runners");

            migrationBuilder.DropIndex(
                name: "IX_Runners_UpdatedBy",
                table: "Runners");

            migrationBuilder.DropIndex(
                name: "IX_RunnerInterviews_CreatedBy",
                table: "RunnerInterviews");

            migrationBuilder.DropIndex(
                name: "IX_RunnerInterviews_RescheduledBy",
                table: "RunnerInterviews");

            migrationBuilder.DropIndex(
                name: "IX_CompanyProfile_CompanyId",
                table: "CompanyProfile");

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

            migrationBuilder.RenameColumn(
                name: "WorkerProfileId",
                table: "WorkerComment",
                newName: "WorkerId");

            migrationBuilder.RenameColumn(
                name: "CompanyProfileId",
                table: "WorkerComment",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerComment_WorkerProfileId",
                table: "WorkerComment",
                newName: "IX_WorkerComment_WorkerId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerComment_CompanyProfileId",
                table: "WorkerComment",
                newName: "IX_WorkerComment_CompanyId");

            migrationBuilder.RenameColumn(
                name: "CompanyProfileId",
                table: "Request",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Request_CompanyProfileId",
                table: "Request",
                newName: "IX_Request_CompanyId");

            migrationBuilder.RenameColumn(
                name: "CompanyProfileId",
                table: "Invoice",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Invoice_CompanyProfileId",
                table: "Invoice",
                newName: "IX_Invoice_CompanyId");

            migrationBuilder.RenameColumn(
                name: "CompanyProfileId",
                table: "CompanyUser",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyUser_CompanyProfileId_UserId",
                table: "CompanyUser",
                newName: "IX_CompanyUser_CompanyId_UserId");

            migrationBuilder.AddColumn<Guid>(
                name: "AgencyId",
                table: "WorkerComment",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AgencyId",
                table: "Runners",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "AgencyId",
                table: "Request",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_WorkerProfile_WorkerId_AgencyId",
                table: "WorkerProfile",
                columns: new[] { "WorkerId", "AgencyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkerComment_AgencyId",
                table: "WorkerComment",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Runners_AgencyId",
                table: "Runners",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_AgencyId",
                table: "Request",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfile_CompanyId_AgencyId",
                table: "CompanyProfile",
                columns: new[] { "CompanyId", "AgencyId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyUser_User_CompanyId",
                table: "CompanyUser",
                column: "CompanyId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_CompanyProfile_CompanyId",
                table: "Invoice",
                column: "CompanyId",
                principalTable: "CompanyProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_Agency_AgencyId",
                table: "Request",
                column: "AgencyId",
                principalTable: "Agency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_User_CompanyId",
                table: "Request",
                column: "CompanyId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Runners_Agency_AgencyId",
                table: "Runners",
                column: "AgencyId",
                principalTable: "Agency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerComment_Agency_AgencyId",
                table: "WorkerComment",
                column: "AgencyId",
                principalTable: "Agency",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerComment_User_CompanyId",
                table: "WorkerComment",
                column: "CompanyId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerComment_User_WorkerId",
                table: "WorkerComment",
                column: "WorkerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
