using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RunnersWorkerOnlyAndBoardRunners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM ""Runners""
                WHERE ""CandidateId"" IS NOT NULL OR ""WorkerProfileId"" IS NULL;
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_Runners_Candidates_CandidateId",
                table: "Runners");

            migrationBuilder.DropIndex(
                name: "IX_Runners_CandidateId",
                table: "Runners");

            migrationBuilder.DropColumn(
                name: "CandidateId",
                table: "Runners");

            migrationBuilder.AddColumn<Guid>(
                name: "RequestRecruiterId",
                table: "Runners",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Runners_RequestRecruiterId",
                table: "Runners",
                column: "RequestRecruiterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Runners_RequestRecruiter_RequestRecruiterId",
                table: "Runners",
                column: "RequestRecruiterId",
                principalTable: "RequestRecruiter",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddColumn<bool>(
                name: "UsesRunners",
                table: "Request",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.Sql(@"
                INSERT INTO ""Runners"" (""Id"", ""AgencyId"", ""RequestId"", ""WorkerProfileId"", ""RequestRecruiterId"", ""Type"", ""Status"", ""CreatedAt"", ""CreatedBy"")
                SELECT gen_random_uuid(), req.""AgencyId"", rr.""RequestId"", d.""WorkerProfileId"", d.""RequestRecruiterId"",
                       'Passive', 'SentToClient', d.""CreatedAt"", COALESCE(d.""CreatedBy"", ap.""UserId"")
                FROM (
                    SELECT DISTINCT ON (rr2.""RequestId"", wd.""WorkerProfileId"") wd.*
                    FROM ""WorkerDispatch"" wd
                    INNER JOIN ""RequestRecruiter"" rr2 ON rr2.""Id"" = wd.""RequestRecruiterId""
                    ORDER BY rr2.""RequestId"", wd.""WorkerProfileId"", wd.""CreatedAt""
                ) d
                INNER JOIN ""RequestRecruiter"" rr ON rr.""Id"" = d.""RequestRecruiterId""
                INNER JOIN ""Request"" req ON req.""Id"" = rr.""RequestId""
                LEFT JOIN ""AgencyPersonnel"" ap ON ap.""Id"" = rr.""RecruiterId""
                WHERE COALESCE(d.""CreatedBy"", ap.""UserId"") IS NOT NULL
                  AND NOT EXISTS (
                      SELECT 1 FROM ""Runners"" ex
                      WHERE ex.""RequestId"" = rr.""RequestId"" AND ex.""WorkerProfileId"" = d.""WorkerProfileId"");
            ");

            migrationBuilder.Sql(@"
                INSERT INTO ""RunnerStatusHistories"" (""Id"", ""RunnerId"", ""PreviousStatus"", ""NewStatus"", ""ChangedBy"", ""ChangedAt"")
                SELECT gen_random_uuid(), r.""Id"", NULL, 'SentToClient', r.""CreatedBy"", r.""CreatedAt""
                FROM ""Runners"" r
                WHERE NOT EXISTS (SELECT 1 FROM ""RunnerStatusHistories"" h WHERE h.""RunnerId"" = r.""Id"");
            ");

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkerProfileId",
                table: "Runners",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.DropTable(
                name: "WorkerDispatch");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkerDispatch",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestRecruiterId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkerProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerDispatch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkerDispatch_RequestRecruiter_RequestRecruiterId",
                        column: x => x.RequestRecruiterId,
                        principalTable: "RequestRecruiter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkerDispatch_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkerDispatch_WorkerProfile_WorkerProfileId",
                        column: x => x.WorkerProfileId,
                        principalTable: "WorkerProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkerDispatch_CreatedBy",
                table: "WorkerDispatch",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerDispatch_RequestRecruiterId_WorkerProfileId",
                table: "WorkerDispatch",
                columns: new[] { "RequestRecruiterId", "WorkerProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkerDispatch_WorkerProfileId",
                table: "WorkerDispatch",
                column: "WorkerProfileId");

            migrationBuilder.Sql(@"
                INSERT INTO ""WorkerDispatch"" (""Id"", ""RequestRecruiterId"", ""WorkerProfileId"", ""CreatedAt"", ""CreatedBy"")
                SELECT gen_random_uuid(), r.""RequestRecruiterId"", r.""WorkerProfileId"", r.""CreatedAt"", r.""CreatedBy""
                FROM ""Runners"" r
                WHERE r.""RequestRecruiterId"" IS NOT NULL;
            ");

            migrationBuilder.Sql(@"DELETE FROM ""Runners"" WHERE ""RequestRecruiterId"" IS NOT NULL;");

            migrationBuilder.DropForeignKey(
                name: "FK_Runners_RequestRecruiter_RequestRecruiterId",
                table: "Runners");

            migrationBuilder.DropIndex(
                name: "IX_Runners_RequestRecruiterId",
                table: "Runners");

            migrationBuilder.DropColumn(
                name: "RequestRecruiterId",
                table: "Runners");

            migrationBuilder.DropColumn(
                name: "UsesRunners",
                table: "Request");

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkerProfileId",
                table: "Runners",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "CandidateId",
                table: "Runners",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Runners_CandidateId",
                table: "Runners",
                column: "CandidateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Runners_Candidates_CandidateId",
                table: "Runners",
                column: "CandidateId",
                principalTable: "Candidates",
                principalColumn: "Id");
        }
    }
}
