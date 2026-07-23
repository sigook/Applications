using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWcCodeToWorkerProfileAndJobCostingToRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WcCode",
                table: "WorkerProfile",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobCosting",
                table: "Request",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WcCode",
                table: "WorkerProfile");

            migrationBuilder.DropColumn(
                name: "JobCosting",
                table: "Request");
        }
    }
}
