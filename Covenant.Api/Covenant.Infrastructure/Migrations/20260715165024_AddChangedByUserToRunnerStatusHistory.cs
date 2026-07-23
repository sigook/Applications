using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChangedByUserToRunnerStatusHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RunnerStatusHistories_ChangedBy",
                table: "RunnerStatusHistories",
                column: "ChangedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_RunnerStatusHistories_User_ChangedBy",
                table: "RunnerStatusHistories",
                column: "ChangedBy",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RunnerStatusHistories_User_ChangedBy",
                table: "RunnerStatusHistories");

            migrationBuilder.DropIndex(
                name: "IX_RunnerStatusHistories_ChangedBy",
                table: "RunnerStatusHistories");
        }
    }
}
