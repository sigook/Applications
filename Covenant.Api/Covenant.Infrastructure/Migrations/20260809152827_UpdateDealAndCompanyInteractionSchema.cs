using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDealAndCompanyInteractionSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyInteraction_CompanyProfiles_CompanyId",
                table: "CompanyInteraction");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyInteraction_Users_OwnerId",
                table: "CompanyInteraction");

            migrationBuilder.DropForeignKey(
                name: "FK_Deal_CompanyProfiles_CompanyId",
                table: "Deal");

            migrationBuilder.DropForeignKey(
                name: "FK_Deal_Users_OwnerId",
                table: "Deal");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Deal",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "Deal",
                newName: "CompanyProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Deal_OwnerId",
                table: "Deal",
                newName: "IX_Deal_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Deal_CompanyId",
                table: "Deal",
                newName: "IX_Deal_CompanyProfileId");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "CompanyInteraction",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "CompanyInteraction",
                newName: "CompanyProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyInteraction_OwnerId",
                table: "CompanyInteraction",
                newName: "IX_CompanyInteraction_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyInteraction_CompanyId",
                table: "CompanyInteraction",
                newName: "IX_CompanyInteraction_CompanyProfileId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Deal",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Deal",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyInteraction_CompanyProfiles_CompanyProfileId",
                table: "CompanyInteraction",
                column: "CompanyProfileId",
                principalTable: "CompanyProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyInteraction_Users_UserId",
                table: "CompanyInteraction",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deal_CompanyProfiles_CompanyProfileId",
                table: "Deal",
                column: "CompanyProfileId",
                principalTable: "CompanyProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deal_Users_UserId",
                table: "Deal",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyInteraction_CompanyProfiles_CompanyProfileId",
                table: "CompanyInteraction");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyInteraction_Users_UserId",
                table: "CompanyInteraction");

            migrationBuilder.DropForeignKey(
                name: "FK_Deal_CompanyProfiles_CompanyProfileId",
                table: "Deal");

            migrationBuilder.DropForeignKey(
                name: "FK_Deal_Users_UserId",
                table: "Deal");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Deal");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Deal");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Deal",
                newName: "OwnerId");

            migrationBuilder.RenameColumn(
                name: "CompanyProfileId",
                table: "Deal",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Deal_UserId",
                table: "Deal",
                newName: "IX_Deal_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Deal_CompanyProfileId",
                table: "Deal",
                newName: "IX_Deal_CompanyId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "CompanyInteraction",
                newName: "OwnerId");

            migrationBuilder.RenameColumn(
                name: "CompanyProfileId",
                table: "CompanyInteraction",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyInteraction_UserId",
                table: "CompanyInteraction",
                newName: "IX_CompanyInteraction_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyInteraction_CompanyProfileId",
                table: "CompanyInteraction",
                newName: "IX_CompanyInteraction_CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyInteraction_CompanyProfiles_CompanyId",
                table: "CompanyInteraction",
                column: "CompanyId",
                principalTable: "CompanyProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyInteraction_Users_OwnerId",
                table: "CompanyInteraction",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deal_CompanyProfiles_CompanyId",
                table: "Deal",
                column: "CompanyId",
                principalTable: "CompanyProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deal_Users_OwnerId",
                table: "Deal",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
