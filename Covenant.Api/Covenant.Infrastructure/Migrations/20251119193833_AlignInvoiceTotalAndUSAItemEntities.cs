using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    public partial class AlignInvoiceTotalAndUSAItemEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AgencyRate",
                table: "InvoiceUSAItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Holiday",
                table: "InvoiceUSAItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Missing",
                table: "InvoiceUSAItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MissingOvertime",
                table: "InvoiceUSAItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NightShift",
                table: "InvoiceUSAItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherRegular",
                table: "InvoiceUSAItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Overtime",
                table: "InvoiceUSAItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Regular",
                table: "InvoiceUSAItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "TimeSheetTotalId",
                table: "InvoiceUSAItem",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalGross",
                table: "InvoiceUSAItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalNet",
                table: "InvoiceUSAItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "InvoiceTotal",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Quantity",
                table: "InvoiceTotal",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "InvoiceTotal",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "InvoiceTotal",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceUSAItem_TimeSheetTotalId",
                table: "InvoiceUSAItem",
                column: "TimeSheetTotalId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceUSAItem_TimeSheetTotal_TimeSheetTotalId",
                table: "InvoiceUSAItem",
                column: "TimeSheetTotalId",
                principalTable: "TimeSheetTotal",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceUSAItem_TimeSheetTotal_TimeSheetTotalId",
                table: "InvoiceUSAItem");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceUSAItem_TimeSheetTotalId",
                table: "InvoiceUSAItem");

            migrationBuilder.DropColumn(
                name: "AgencyRate",
                table: "InvoiceUSAItem");

            migrationBuilder.DropColumn(
                name: "Holiday",
                table: "InvoiceUSAItem");

            migrationBuilder.DropColumn(
                name: "Missing",
                table: "InvoiceUSAItem");

            migrationBuilder.DropColumn(
                name: "MissingOvertime",
                table: "InvoiceUSAItem");

            migrationBuilder.DropColumn(
                name: "NightShift",
                table: "InvoiceUSAItem");

            migrationBuilder.DropColumn(
                name: "OtherRegular",
                table: "InvoiceUSAItem");

            migrationBuilder.DropColumn(
                name: "Overtime",
                table: "InvoiceUSAItem");

            migrationBuilder.DropColumn(
                name: "Regular",
                table: "InvoiceUSAItem");

            migrationBuilder.DropColumn(
                name: "TimeSheetTotalId",
                table: "InvoiceUSAItem");

            migrationBuilder.DropColumn(
                name: "TotalGross",
                table: "InvoiceUSAItem");

            migrationBuilder.DropColumn(
                name: "TotalNet",
                table: "InvoiceUSAItem");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "InvoiceTotal");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "InvoiceTotal");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "InvoiceTotal");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "InvoiceTotal");
        }
    }
}
