using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UnifyInvoices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InvoiceNumber",
                table: "Invoices",
                newName: "InvoiceNumberId");

            migrationBuilder.RenameColumn(
                name: "Hst",
                table: "Invoices",
                newName: "Tax");

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "Invoices",
                type: "text",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "Invoices"
                SET "InvoiceNumber" = 'AI-' || lpad("InvoiceNumberId"::text, 4, '0') || '-' || to_char("CreatedAt", 'YY');
                """);

            migrationBuilder.Sql("""
                ALTER TABLE "Invoices" ALTER COLUMN "CreatedAt" TYPE timestamp with time zone USING "CreatedAt" AT TIME ZONE 'UTC';
                ALTER TABLE "Invoices" ALTER COLUMN "WeekEnding" TYPE timestamp with time zone USING "WeekEnding" AT TIME ZONE 'UTC';
                """);

            migrationBuilder.DropColumn(name: "NightShiftRate", table: "Invoices");
            migrationBuilder.DropColumn(name: "HolidayRate", table: "Invoices");
            migrationBuilder.DropColumn(name: "OverTimeRate", table: "Invoices");
            migrationBuilder.DropColumn(name: "VacationsRate", table: "Invoices");
            migrationBuilder.DropColumn(name: "HstRate", table: "Invoices");
            migrationBuilder.DropColumn(name: "BonusRate", table: "Invoices");

            migrationBuilder.Sql("""
                INSERT INTO "Invoices" ("Id", "InvoiceNumberId", "InvoiceNumber", "CompanyProfileId", "SubTotal", "Tax", "TotalNet",
                                        "CreatedAt", "WeekEnding", "Email", "Status", "UpdatedAt", "UpdatedBy")
                SELECT "Id", "InvoiceNumberId", "InvoiceNumber", "CompanyProfileId", "SubTotal", "Tax", "TotalNet",
                       "CreatedAt", "WeekEnding", "BillToEmail", "Status", "UpdatedAt", "UpdatedBy"
                FROM "InvoicesUSA"
                ORDER BY "NumberId";
                """);

            migrationBuilder.Sql("""
                INSERT INTO "InvoiceTotals" ("Id", "InvoiceId", "TimeSheetTotalId", "Quantity", "UnitPrice", "Description", "AgencyRate",
                                             "Regular", "OtherRegular", "Missing", "MissingOvertime", "NightShift", "Holiday", "Overtime",
                                             "TotalGross", "TotalNet", "Total")
                SELECT "Id", "InvoiceUSAId", "TimeSheetTotalId", "Quantity", "UnitPrice", "Description", "AgencyRate",
                       "Regular", "OtherRegular", "Missing", "MissingOvertime", "NightShift", "Holiday", "Overtime",
                       "TotalGross", "TotalNet", "Total"
                FROM "InvoiceUSAItems"
                WHERE "TimeSheetTotalId" IS NOT NULL;
                """);

            migrationBuilder.Sql("""
                INSERT INTO "InvoiceTotals" ("Id", "InvoiceId", "TimeSheetTotalId", "Quantity", "UnitPrice", "Description", "AgencyRate",
                                             "Regular", "OtherRegular", "Missing", "MissingOvertime", "NightShift", "Holiday", "Overtime",
                                             "TotalGross", "TotalNet", "Total")
                SELECT gen_random_uuid(), "InvoiceUSAId", "TimeSheetTotalId", 0, 0, NULL, 0,
                       0, 0, 0, 0, 0, 0, 0,
                       0, 0, 0
                FROM "InvoiceUSATimeSheetTotals";
                """);

            migrationBuilder.Sql("""
                INSERT INTO "InvoiceAdditionalItems" ("Id", "InvoiceId", "Quantity", "UnitPrice", "Description")
                SELECT "Id", "InvoiceUSAId", "Quantity", "UnitPrice", "Description"
                FROM "InvoiceUSAItems"
                WHERE "TimeSheetTotalId" IS NULL;
                """);

            migrationBuilder.Sql("""
                INSERT INTO "InvoiceDiscounts" ("Id", "InvoiceId", "Quantity", "UnitPrice", "Amount", "Description")
                SELECT "Id", "InvoiceUSAId", "Quantity", "UnitPrice", "Total", "Description"
                FROM "InvoiceUSADiscounts";
                """);

            migrationBuilder.AddColumn<Guid>(
                name: "InvoiceId",
                table: "InvoiceAdditionalDetails",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "InvoiceAdditionalDetails"
                SET "InvoiceId" = coalesce("CanadaInvoiceId", "UsaInvoiceId");
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceAdditionalDetails_InvoicesUSA_UsaInvoiceId",
                table: "InvoiceAdditionalDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceAdditionalDetails_Invoices_CanadaInvoiceId",
                table: "InvoiceAdditionalDetails");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceAdditionalDetails_CanadaInvoiceId",
                table: "InvoiceAdditionalDetails");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceAdditionalDetails_UsaInvoiceId",
                table: "InvoiceAdditionalDetails");

            migrationBuilder.DropColumn(
                name: "CanadaInvoiceId",
                table: "InvoiceAdditionalDetails");

            migrationBuilder.DropColumn(
                name: "UsaInvoiceId",
                table: "InvoiceAdditionalDetails");

            migrationBuilder.AlterColumn<Guid>(
                name: "InvoiceId",
                table: "InvoiceAdditionalDetails",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceAdditionalDetails_InvoiceId",
                table: "InvoiceAdditionalDetails",
                column: "InvoiceId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceAdditionalDetails_Invoices_InvoiceId",
                table: "InvoiceAdditionalDetails",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceNumber",
                table: "Invoices",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices",
                column: "InvoiceNumber");

            migrationBuilder.DropTable(name: "InvoiceUSATimeSheetTotals");
            migrationBuilder.DropTable(name: "InvoiceUSAItems");
            migrationBuilder.DropTable(name: "InvoiceUSADiscounts");
            migrationBuilder.DropTable(name: "InvoicesUSA");

            migrationBuilder.Sql("""
                DROP VIEW IF EXISTS "PayStubHistory";
                DROP VIEW IF EXISTS "TimesheetHistory";
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE OR REPLACE VIEW "PayStubHistory" AS
                SELECT
                    ROW_NUMBER() OVER (ORDER BY "NumberId" DESC) AS "RowNumber",
                    "Id",
                    "WorkerProfileId",
                    "NumberId",
                    "PayStubNumber",
                    "WeekEnding",
                    "TotalEarnings",
                    "Vacations",
                    "TotalPaid",
                    "DateWorkBegins",
                    "DateWorkEnd"
                FROM "PayStubs";

                CREATE OR REPLACE VIEW "TimesheetHistory" AS
                SELECT
                    ROW_NUMBER() OVER (ORDER BY ts."Date" DESC) AS "RowNumber",
                    r."NumberId",
                    cp."FullName" AS "CompanyName",
                    r."JobTitle",
                    ts."Date",
                    ts."IsHoliday",
                    tst."RegularHours",
                    tst."HolidayHours",
                    tst."OvertimeHours",
                    ts."MissingHours",
                    ts."MissingHoursOvertime",
                    wp."Id" AS "WorkerProfileId"
                FROM "WorkerProfiles" wp
                JOIN "WorkerRequests" wr ON wp."Id" = wr."WorkerProfileId"
                JOIN "TimeSheets" ts ON wr."Id" = ts."WorkerRequestId"
                LEFT JOIN "TimeSheetTotals" tst ON ts."Id" = tst."TimeSheetId"
                JOIN "Requests" r ON wr."RequestId" = r."Id"
                JOIN "CompanyProfiles" cp ON r."CompanyProfileId" = cp."Id";
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceAdditionalDetails_Invoices_InvoiceId",
                table: "InvoiceAdditionalDetails");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceAdditionalDetails_InvoiceId",
                table: "InvoiceAdditionalDetails");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "InvoiceAdditionalDetails");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "InvoiceNumberId",
                table: "Invoices",
                newName: "InvoiceNumber");

            migrationBuilder.RenameColumn(
                name: "Tax",
                table: "Invoices",
                newName: "Hst");

            migrationBuilder.Sql("""
                ALTER TABLE "Invoices" ALTER COLUMN "CreatedAt" TYPE timestamp without time zone USING "CreatedAt" AT TIME ZONE 'UTC';
                ALTER TABLE "Invoices" ALTER COLUMN "WeekEnding" TYPE timestamp without time zone USING "WeekEnding" AT TIME ZONE 'UTC';
                """);

            migrationBuilder.AddColumn<decimal>(name: "BonusRate", table: "Invoices", type: "numeric", nullable: false, defaultValue: 0m);
            migrationBuilder.AddColumn<decimal>(name: "HolidayRate", table: "Invoices", type: "numeric", nullable: false, defaultValue: 0m);
            migrationBuilder.AddColumn<decimal>(name: "HstRate", table: "Invoices", type: "numeric", nullable: false, defaultValue: 0m);
            migrationBuilder.AddColumn<decimal>(name: "NightShiftRate", table: "Invoices", type: "numeric", nullable: false, defaultValue: 0m);
            migrationBuilder.AddColumn<decimal>(name: "OverTimeRate", table: "Invoices", type: "numeric", nullable: false, defaultValue: 0m);
            migrationBuilder.AddColumn<decimal>(name: "VacationsRate", table: "Invoices", type: "numeric", nullable: false, defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "CanadaInvoiceId",
                table: "InvoiceAdditionalDetails",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsaInvoiceId",
                table: "InvoiceAdditionalDetails",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InvoicesUSA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    BillFromAddress = table.Column<string>(type: "text", nullable: true),
                    BillFromFax = table.Column<string>(type: "text", nullable: true),
                    BillFromPhone = table.Column<string>(type: "text", nullable: true),
                    BillToAddress = table.Column<string>(type: "text", nullable: true),
                    BillToEmail = table.Column<string>(type: "text", nullable: true),
                    BillToFax = table.Column<string>(type: "text", nullable: true),
                    BillToPhone = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HtmlNotes = table.Column<string>(type: "text", nullable: true),
                    InvoiceNumber = table.Column<string>(type: "text", nullable: false),
                    InvoiceNumberId = table.Column<long>(type: "bigint", nullable: false),
                    NumberId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    SubTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    Tax = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalNet = table.Column<decimal>(type: "numeric", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    WeekEnding = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicesUSA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoicesUSA_CompanyProfiles_CompanyProfileId",
                        column: x => x.CompanyProfileId,
                        principalTable: "CompanyProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoicesUSA_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvoiceUSADiscounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceUSAId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<double>(type: "double precision", nullable: false),
                    Total = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceUSADiscounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceUSADiscounts_InvoicesUSA_InvoiceUSAId",
                        column: x => x.InvoiceUSAId,
                        principalTable: "InvoicesUSA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceUSAItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceUSAId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeSheetTotalId = table.Column<Guid>(type: "uuid", nullable: true),
                    AgencyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Holiday = table.Column<decimal>(type: "numeric", nullable: false),
                    Missing = table.Column<decimal>(type: "numeric", nullable: false),
                    MissingOvertime = table.Column<decimal>(type: "numeric", nullable: false),
                    NightShift = table.Column<decimal>(type: "numeric", nullable: false),
                    OtherRegular = table.Column<decimal>(type: "numeric", nullable: false),
                    Overtime = table.Column<decimal>(type: "numeric", nullable: false),
                    Quantity = table.Column<double>(type: "double precision", nullable: false),
                    Regular = table.Column<decimal>(type: "numeric", nullable: false),
                    Total = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalGross = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalNet = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceUSAItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceUSAItems_InvoicesUSA_InvoiceUSAId",
                        column: x => x.InvoiceUSAId,
                        principalTable: "InvoicesUSA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceUSAItems_TimeSheetTotals_TimeSheetTotalId",
                        column: x => x.TimeSheetTotalId,
                        principalTable: "TimeSheetTotals",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvoiceUSATimeSheetTotals",
                columns: table => new
                {
                    InvoiceUSAId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeSheetTotalId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceUSATimeSheetTotals", x => new { x.InvoiceUSAId, x.TimeSheetTotalId });
                    table.ForeignKey(
                        name: "FK_InvoiceUSATimeSheetTotals_InvoicesUSA_InvoiceUSAId",
                        column: x => x.InvoiceUSAId,
                        principalTable: "InvoicesUSA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceUSATimeSheetTotals_TimeSheetTotals_TimeSheetTotalId",
                        column: x => x.TimeSheetTotalId,
                        principalTable: "TimeSheetTotals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceAdditionalDetails_CanadaInvoiceId",
                table: "InvoiceAdditionalDetails",
                column: "CanadaInvoiceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceAdditionalDetails_UsaInvoiceId",
                table: "InvoiceAdditionalDetails",
                column: "UsaInvoiceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoicesUSA_CompanyProfileId",
                table: "InvoicesUSA",
                column: "CompanyProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoicesUSA_InvoiceNumber",
                table: "InvoicesUSA",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoicesUSA_InvoiceNumberId",
                table: "InvoicesUSA",
                column: "InvoiceNumberId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoicesUSA_UpdatedBy",
                table: "InvoicesUSA",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceUSADiscounts_InvoiceUSAId",
                table: "InvoiceUSADiscounts",
                column: "InvoiceUSAId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceUSAItems_InvoiceUSAId",
                table: "InvoiceUSAItems",
                column: "InvoiceUSAId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceUSAItems_TimeSheetTotalId",
                table: "InvoiceUSAItems",
                column: "TimeSheetTotalId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceUSATimeSheetTotals_TimeSheetTotalId",
                table: "InvoiceUSATimeSheetTotals",
                column: "TimeSheetTotalId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceAdditionalDetails_InvoicesUSA_UsaInvoiceId",
                table: "InvoiceAdditionalDetails",
                column: "UsaInvoiceId",
                principalTable: "InvoicesUSA",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceAdditionalDetails_Invoices_CanadaInvoiceId",
                table: "InvoiceAdditionalDetails",
                column: "CanadaInvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
