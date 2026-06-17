using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWeeklyBoardRecruiterAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestRecruiter",
                table: "RequestRecruiter");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "RequestRecruiter",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<DateTime>(
                name: "WorkDate",
                table: "RequestRecruiter",
                type: "date",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestRecruiter",
                table: "RequestRecruiter",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "WorkerDispatch",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestRecruiterId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkerProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                        name: "FK_WorkerDispatch_WorkerProfile_WorkerProfileId",
                        column: x => x.WorkerProfileId,
                        principalTable: "WorkerProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestRecruiter_RequestId_RecruiterId_WorkDate",
                table: "RequestRecruiter",
                columns: new[] { "RequestId", "RecruiterId", "WorkDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkerDispatch_RequestRecruiterId_WorkerProfileId",
                table: "WorkerDispatch",
                columns: new[] { "RequestRecruiterId", "WorkerProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkerDispatch_WorkerProfileId",
                table: "WorkerDispatch",
                column: "WorkerProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkerDispatch");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestRecruiter",
                table: "RequestRecruiter");

            migrationBuilder.DropIndex(
                name: "IX_RequestRecruiter_RequestId_RecruiterId_WorkDate",
                table: "RequestRecruiter");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "RequestRecruiter");

            migrationBuilder.DropColumn(
                name: "WorkDate",
                table: "RequestRecruiter");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestRecruiter",
                table: "RequestRecruiter",
                columns: new[] { "RequestId", "RecruiterId" });
        }
    }
}
