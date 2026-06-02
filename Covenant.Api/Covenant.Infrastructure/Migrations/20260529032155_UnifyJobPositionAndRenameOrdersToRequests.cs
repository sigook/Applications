using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UnifyJobPositionAndRenameOrdersToRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfileJobPositionRate_JobPosition_JobPositionId",
                table: "CompanyProfileJobPositionRate");

            migrationBuilder.DropTable(
                name: "JobPosition");

            migrationBuilder.DropIndex(
                name: "IX_CompanyProfileJobPositionRate_JobPositionId",
                table: "CompanyProfileJobPositionRate");

            migrationBuilder.DropColumn(
                name: "JobPositionId",
                table: "CompanyProfileJobPositionRate");

            migrationBuilder.RenameColumn(
                name: "OtherJobPosition",
                table: "CompanyProfileJobPositionRate",
                newName: "JobPosition");

            migrationBuilder.RenameColumn(
                name: "RequiresPermissionToSeeOrders",
                table: "CompanyProfile",
                newName: "RequiresPermissionToSeeRequests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JobPosition",
                table: "CompanyProfileJobPositionRate",
                newName: "OtherJobPosition");

            migrationBuilder.RenameColumn(
                name: "RequiresPermissionToSeeRequests",
                table: "CompanyProfile",
                newName: "RequiresPermissionToSeeOrders");

            migrationBuilder.AddColumn<Guid>(
                name: "JobPositionId",
                table: "CompanyProfileJobPositionRate",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "JobPosition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IndustryId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPosition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobPosition_Industry_IndustryId",
                        column: x => x.IndustryId,
                        principalTable: "Industry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfileJobPositionRate_JobPositionId",
                table: "CompanyProfileJobPositionRate",
                column: "JobPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPosition_IndustryId",
                table: "JobPosition",
                column: "IndustryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfileJobPositionRate_JobPosition_JobPositionId",
                table: "CompanyProfileJobPositionRate",
                column: "JobPositionId",
                principalTable: "JobPosition",
                principalColumn: "Id");
        }
    }
}
