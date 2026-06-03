using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceProvinceTaxesWithLocationTaxes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProvinceTaxes");

            migrationBuilder.CreateTable(
                name: "LocationTaxes",
                columns: table => new
                {
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tax1 = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationTaxes", x => x.LocationId);
                    table.ForeignKey(
                        name: "FK_LocationTaxes_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationTaxes");

            migrationBuilder.CreateTable(
                name: "ProvinceTaxes",
                columns: table => new
                {
                    ProvinceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tax1 = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProvinceTaxes", x => x.ProvinceId);
                    table.ForeignKey(
                        name: "FK_ProvinceTaxes_Province_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Province",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
