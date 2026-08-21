using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestComplianceAndApplicantStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "RequestApplicants",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.CreateTable(
                name: "RequestComplianceItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsMandatory = table.Column<bool>(type: "boolean", nullable: false),
                    DocumentTarget = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RequestId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestComplianceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestComplianceItems_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestApplicantComplianceItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestApplicantId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestComplianceItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestApplicantComplianceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestApplicantComplianceItems_RequestApplicants_RequestAp~",
                        column: x => x.RequestApplicantId,
                        principalTable: "RequestApplicants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestApplicantComplianceItems_RequestComplianceItems_Requ~",
                        column: x => x.RequestComplianceItemId,
                        principalTable: "RequestComplianceItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestApplicantComplianceItems_RequestApplicantId_RequestC~",
                table: "RequestApplicantComplianceItems",
                columns: new[] { "RequestApplicantId", "RequestComplianceItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestApplicantComplianceItems_RequestComplianceItemId",
                table: "RequestApplicantComplianceItems",
                column: "RequestComplianceItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestComplianceItems_RequestId",
                table: "RequestComplianceItems",
                column: "RequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestApplicantComplianceItems");

            migrationBuilder.DropTable(
                name: "RequestComplianceItems");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "RequestApplicants");
        }
    }
}
