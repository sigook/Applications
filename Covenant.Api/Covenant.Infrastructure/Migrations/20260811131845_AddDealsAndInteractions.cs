using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDealsAndInteractions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "FK_Deal_CovenantFiles_DocumentId",
                table: "Deal");

            migrationBuilder.DropForeignKey(
                name: "FK_Deal_Users_UserId",
                table: "Deal");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Deal",
                table: "Deal");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyInteraction",
                table: "CompanyInteraction");

            migrationBuilder.RenameTable(
                name: "Deal",
                newName: "Deals");

            migrationBuilder.RenameTable(
                name: "CompanyInteraction",
                newName: "CompanyInteractions");

            migrationBuilder.RenameIndex(
                name: "IX_Deal_UserId",
                table: "Deals",
                newName: "IX_Deals_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Deal_DocumentId",
                table: "Deals",
                newName: "IX_Deals_DocumentId");

            migrationBuilder.RenameIndex(
                name: "IX_Deal_CompanyProfileId",
                table: "Deals",
                newName: "IX_Deals_CompanyProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyInteraction_UserId",
                table: "CompanyInteractions",
                newName: "IX_CompanyInteractions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyInteraction_CompanyProfileId",
                table: "CompanyInteractions",
                newName: "IX_CompanyInteractions_CompanyProfileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Deals",
                table: "Deals",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyInteractions",
                table: "CompanyInteractions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyInteractions_CompanyProfiles_CompanyProfileId",
                table: "CompanyInteractions",
                column: "CompanyProfileId",
                principalTable: "CompanyProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyInteractions_Users_UserId",
                table: "CompanyInteractions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_CompanyProfiles_CompanyProfileId",
                table: "Deals",
                column: "CompanyProfileId",
                principalTable: "CompanyProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_CovenantFiles_DocumentId",
                table: "Deals",
                column: "DocumentId",
                principalTable: "CovenantFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_Users_UserId",
                table: "Deals",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyInteractions_CompanyProfiles_CompanyProfileId",
                table: "CompanyInteractions");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyInteractions_Users_UserId",
                table: "CompanyInteractions");

            migrationBuilder.DropForeignKey(
                name: "FK_Deals_CompanyProfiles_CompanyProfileId",
                table: "Deals");

            migrationBuilder.DropForeignKey(
                name: "FK_Deals_CovenantFiles_DocumentId",
                table: "Deals");

            migrationBuilder.DropForeignKey(
                name: "FK_Deals_Users_UserId",
                table: "Deals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Deals",
                table: "Deals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyInteractions",
                table: "CompanyInteractions");

            migrationBuilder.RenameTable(
                name: "Deals",
                newName: "Deal");

            migrationBuilder.RenameTable(
                name: "CompanyInteractions",
                newName: "CompanyInteraction");

            migrationBuilder.RenameIndex(
                name: "IX_Deals_UserId",
                table: "Deal",
                newName: "IX_Deal_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Deals_DocumentId",
                table: "Deal",
                newName: "IX_Deal_DocumentId");

            migrationBuilder.RenameIndex(
                name: "IX_Deals_CompanyProfileId",
                table: "Deal",
                newName: "IX_Deal_CompanyProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyInteractions_UserId",
                table: "CompanyInteraction",
                newName: "IX_CompanyInteraction_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyInteractions_CompanyProfileId",
                table: "CompanyInteraction",
                newName: "IX_CompanyInteraction_CompanyProfileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Deal",
                table: "Deal",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyInteraction",
                table: "CompanyInteraction",
                column: "Id");

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
                name: "FK_Deal_CovenantFiles_DocumentId",
                table: "Deal",
                column: "DocumentId",
                principalTable: "CovenantFiles",
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
    }
}
