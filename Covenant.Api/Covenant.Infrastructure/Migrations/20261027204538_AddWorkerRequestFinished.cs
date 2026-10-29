using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkerRequestFinished : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FinishComments",
                table: "WorkerRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FinishReason",
                table: "WorkerRequests",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FinishedAt",
                table: "WorkerRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinishedBy",
                table: "WorkerRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PdfGeneratedAt",
                table: "PayStubs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PdfGeneratedAt",
                table: "Invoices",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "WorkerRequests" wr
                SET "WorkerRequestStatus" = 4,
                    "FinishedAt" = COALESCE((SELECT MAX(t."Date") FROM "TimeSheets" t WHERE t."WorkerRequestId" = wr."Id"), wr."RejectedAt"),
                    "FinishedBy" = wr."RejectedBy",
                    "FinishReason" = CASE
                        WHEN wr."RejectComments" ~* '(finish|ended|end of|no longer required|order cancel|cancelled\.|finaliz|completed|temporal|order changed|moved to order)' THEN 1
                        ELSE 4 END,
                    "FinishComments" = wr."RejectComments",
                    "RejectedAt" = NULL,
                    "RejectedBy" = NULL,
                    "RejectComments" = NULL
                WHERE wr."WorkerRequestStatus" = 2
                  AND EXISTS (SELECT 1 FROM "TimeSheets" t WHERE t."WorkerRequestId" = wr."Id")
                  AND COALESCE(wr."RejectComments", '') !~* '(attendance|no show|no-show|stopped going|fired|performance|late|terminated|behavio|complain|dnu|do not use)';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE "WorkerRequests"
                SET "WorkerRequestStatus" = 2,
                    "RejectedAt" = "FinishedAt",
                    "RejectedBy" = "FinishedBy",
                    "RejectComments" = "FinishComments"
                WHERE "WorkerRequestStatus" = 4;
                """);

            migrationBuilder.DropColumn(
                name: "FinishComments",
                table: "WorkerRequests");

            migrationBuilder.DropColumn(
                name: "FinishReason",
                table: "WorkerRequests");

            migrationBuilder.DropColumn(
                name: "FinishedAt",
                table: "WorkerRequests");

            migrationBuilder.DropColumn(
                name: "FinishedBy",
                table: "WorkerRequests");

            migrationBuilder.DropColumn(
                name: "PdfGeneratedAt",
                table: "PayStubs");

            migrationBuilder.DropColumn(
                name: "PdfGeneratedAt",
                table: "Invoices");
        }
    }
}
