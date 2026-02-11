using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceAdditionalDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvoiceAdditionalDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientSiteAddress = table.Column<string>(type: "text", nullable: true),
                    CanadaInvoiceId = table.Column<Guid>(type: "uuid", nullable: true),
                    UsaInvoiceId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceAdditionalDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceAdditionalDetail_InvoiceUSA_UsaInvoiceId",
                        column: x => x.UsaInvoiceId,
                        principalTable: "InvoiceUSA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceAdditionalDetail_Invoice_CanadaInvoiceId",
                        column: x => x.CanadaInvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceAdditionalDetail_CanadaInvoiceId",
                table: "InvoiceAdditionalDetail",
                column: "CanadaInvoiceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceAdditionalDetail_UsaInvoiceId",
                table: "InvoiceAdditionalDetail",
                column: "UsaInvoiceId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoiceAdditionalDetail");
        }
    }
}
