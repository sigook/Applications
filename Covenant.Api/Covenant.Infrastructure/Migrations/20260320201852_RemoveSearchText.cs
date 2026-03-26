using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSearchText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkerProfile_PunchCardId",
                table: "WorkerProfile");

            migrationBuilder.DropIndex(
                name: "IX_WorkerProfile_TextSearch",
                table: "WorkerProfile");

            migrationBuilder.DropColumn(
                name: "TextSearch",
                table: "WorkerProfile");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TextSearch",
                table: "WorkerProfile",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkerProfile_PunchCardId",
                table: "WorkerProfile",
                column: "PunchCardId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkerProfile_TextSearch",
                table: "WorkerProfile",
                column: "TextSearch");
        }
    }
}
