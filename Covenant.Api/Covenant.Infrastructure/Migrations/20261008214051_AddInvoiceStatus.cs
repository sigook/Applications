using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "InvoicesUSA",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "InvoicesUSA",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "InvoicesUSA",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Invoices",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Invoices",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "Invoices",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoicesUSA_UpdatedBy",
                table: "InvoicesUSA",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_UpdatedBy",
                table: "Invoices",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Users_UpdatedBy",
                table: "Invoices",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoicesUSA_Users_UpdatedBy",
                table: "InvoicesUSA",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Users_UpdatedBy",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoicesUSA_Users_UpdatedBy",
                table: "InvoicesUSA");

            migrationBuilder.DropIndex(
                name: "IX_InvoicesUSA_UpdatedBy",
                table: "InvoicesUSA");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_UpdatedBy",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "InvoicesUSA");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "InvoicesUSA");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "InvoicesUSA");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Invoices");
        }
    }
}
