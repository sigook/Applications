using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    public partial class CompanyProfileDocumentTypeColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyProfileDocument",
                table: "CompanyProfileDocument");

            migrationBuilder.DropIndex(
                name: "IX_CompanyProfileDocument_DocumentId",
                table: "CompanyProfileDocument");

            migrationBuilder.AddColumn<byte>(
                name: "DocumentType",
                table: "CompanyProfileDocument",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyProfileDocument",
                table: "CompanyProfileDocument",
                columns: new[] { "DocumentId", "CompanyProfileId" });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfileDocument_CompanyProfileId",
                table: "CompanyProfileDocument",
                column: "CompanyProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyProfileDocument",
                table: "CompanyProfileDocument");

            migrationBuilder.DropIndex(
                name: "IX_CompanyProfileDocument_CompanyProfileId",
                table: "CompanyProfileDocument");

            migrationBuilder.DropColumn(
                name: "DocumentType",
                table: "CompanyProfileDocument");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyProfileDocument",
                table: "CompanyProfileDocument",
                columns: new[] { "CompanyProfileId", "DocumentId" });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfileDocument_DocumentId",
                table: "CompanyProfileDocument",
                column: "DocumentId");
        }
    }
}
