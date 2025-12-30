using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    public partial class RemoveProvinceProfit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProvinceProfit",
                table: "ProvinceTaxes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ProvinceProfit",
                table: "ProvinceTaxes",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
