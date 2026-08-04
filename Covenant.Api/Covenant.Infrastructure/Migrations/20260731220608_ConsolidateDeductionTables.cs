using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConsolidateDeductionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "CppDeductions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PayPeriod = table.Column<byte>(type: "smallint", nullable: false),
                    From = table.Column<decimal>(type: "numeric", nullable: false),
                    To = table.Column<decimal>(type: "numeric", nullable: false),
                    Cpp = table.Column<decimal>(type: "numeric", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CppDeductions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxDeductions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PayPeriod = table.Column<byte>(type: "smallint", nullable: false),
                    TaxType = table.Column<byte>(type: "smallint", nullable: false),
                    From = table.Column<decimal>(type: "numeric", nullable: false),
                    To = table.Column<decimal>(type: "numeric", nullable: false),
                    Cc0 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc1 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc2 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc3 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc4 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc5 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc6 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc7 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc8 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc9 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc10 = table.Column<decimal>(type: "numeric", nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxDeductions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CppDeductions_Year_PayPeriod_From_To",
                table: "CppDeductions",
                columns: new[] { "Year", "PayPeriod", "From", "To" });

            migrationBuilder.CreateIndex(
                name: "IX_TaxDeductions_Year_PayPeriod_TaxType_From_To",
                table: "TaxDeductions",
                columns: new[] { "Year", "PayPeriod", "TaxType", "From", "To" });

            migrationBuilder.Sql("""
                INSERT INTO "CppDeductions" ("Id", "PayPeriod", "From", "To", "Cpp", "Year")
                SELECT "Id", 1, "From", "To", "Cpp", "Year" FROM "CppWeekly";
                """);

            migrationBuilder.Sql("""
                INSERT INTO "CppDeductions" ("Id", "PayPeriod", "From", "To", "Cpp", "Year")
                SELECT "Id", 2, "From", "To", "Cpp", "Year" FROM "CppBiWeekly";
                """);

            migrationBuilder.Sql("""
                INSERT INTO "CppDeductions" ("Id", "PayPeriod", "From", "To", "Cpp", "Year")
                SELECT "Id", 3, "From", "To", "Cpp", "Year" FROM "CppSemiMonthly";
                """);

            migrationBuilder.Sql("""
                INSERT INTO "CppDeductions" ("Id", "PayPeriod", "From", "To", "Cpp", "Year")
                SELECT "Id", 4, "From", "To", "Cpp", "Year" FROM "CppMonthly";
                """);

            migrationBuilder.Sql("""
                INSERT INTO "TaxDeductions" ("Id", "PayPeriod", "TaxType", "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year")
                SELECT "Id", 1, 1, "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year" FROM "TaxWeekly";
                """);

            migrationBuilder.Sql("""
                INSERT INTO "TaxDeductions" ("Id", "PayPeriod", "TaxType", "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year")
                SELECT "Id", 2, 1, "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year" FROM "FederalTaxBiWeekly";
                """);

            migrationBuilder.Sql("""
                INSERT INTO "TaxDeductions" ("Id", "PayPeriod", "TaxType", "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year")
                SELECT "Id", 3, 1, "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year" FROM "FederalTaxSemiMonthly";
                """);

            migrationBuilder.Sql("""
                INSERT INTO "TaxDeductions" ("Id", "PayPeriod", "TaxType", "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year")
                SELECT "Id", 4, 1, "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year" FROM "FederalTaxMonthly";
                """);

            migrationBuilder.Sql("""
                INSERT INTO "TaxDeductions" ("Id", "PayPeriod", "TaxType", "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year")
                SELECT "Id", 1, 2, "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year" FROM "ProvincialTaxWeekly";
                """);

            migrationBuilder.Sql("""
                INSERT INTO "TaxDeductions" ("Id", "PayPeriod", "TaxType", "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year")
                SELECT "Id", 2, 2, "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year" FROM "ProvincialTaxBiWeekly";
                """);

            migrationBuilder.Sql("""
                INSERT INTO "TaxDeductions" ("Id", "PayPeriod", "TaxType", "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year")
                SELECT "Id", 3, 2, "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year" FROM "ProvincialTaxSemiMonthly";
                """);

            migrationBuilder.Sql("""
                INSERT INTO "TaxDeductions" ("Id", "PayPeriod", "TaxType", "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year")
                SELECT "Id", 4, 2, "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year" FROM "ProvincialTaxMonthly";
                """);

            migrationBuilder.DropTable(
                name: "CppBiWeekly");

            migrationBuilder.DropTable(
                name: "CppMonthly");

            migrationBuilder.DropTable(
                name: "CppSemiMonthly");

            migrationBuilder.DropTable(
                name: "CppWeekly");

            migrationBuilder.DropTable(
                name: "FederalTaxBiWeekly");

            migrationBuilder.DropTable(
                name: "FederalTaxMonthly");

            migrationBuilder.DropTable(
                name: "FederalTaxSemiMonthly");

            migrationBuilder.DropTable(
                name: "ProvincialTaxBiWeekly");

            migrationBuilder.DropTable(
                name: "ProvincialTaxMonthly");

            migrationBuilder.DropTable(
                name: "ProvincialTaxSemiMonthly");

            migrationBuilder.DropTable(
                name: "ProvincialTaxWeekly");

            migrationBuilder.DropTable(
                name: "TaxWeekly");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CppBiWeekly",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Cpp = table.Column<decimal>(type: "numeric", nullable: false),
                    From = table.Column<decimal>(type: "numeric", nullable: false),
                    To = table.Column<decimal>(type: "numeric", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CppBiWeekly", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CppMonthly",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Cpp = table.Column<decimal>(type: "numeric", nullable: false),
                    From = table.Column<decimal>(type: "numeric", nullable: false),
                    To = table.Column<decimal>(type: "numeric", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CppMonthly", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CppSemiMonthly",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Cpp = table.Column<decimal>(type: "numeric", nullable: false),
                    From = table.Column<decimal>(type: "numeric", nullable: false),
                    To = table.Column<decimal>(type: "numeric", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CppSemiMonthly", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CppWeekly",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Cpp = table.Column<decimal>(type: "numeric", nullable: false),
                    From = table.Column<decimal>(type: "numeric", nullable: false),
                    To = table.Column<decimal>(type: "numeric", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CppWeekly", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FederalTaxBiWeekly",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Cc0 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc1 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc10 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc2 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc3 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc4 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc5 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc6 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc7 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc8 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc9 = table.Column<decimal>(type: "numeric", nullable: true),
                    From = table.Column<decimal>(type: "numeric", nullable: false),
                    To = table.Column<decimal>(type: "numeric", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FederalTaxBiWeekly", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FederalTaxMonthly",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Cc0 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc1 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc10 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc2 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc3 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc4 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc5 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc6 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc7 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc8 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc9 = table.Column<decimal>(type: "numeric", nullable: true),
                    From = table.Column<decimal>(type: "numeric", nullable: false),
                    To = table.Column<decimal>(type: "numeric", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FederalTaxMonthly", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FederalTaxSemiMonthly",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Cc0 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc1 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc10 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc2 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc3 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc4 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc5 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc6 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc7 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc8 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc9 = table.Column<decimal>(type: "numeric", nullable: true),
                    From = table.Column<decimal>(type: "numeric", nullable: false),
                    To = table.Column<decimal>(type: "numeric", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FederalTaxSemiMonthly", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProvincialTaxBiWeekly",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Cc0 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc1 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc10 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc2 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc3 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc4 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc5 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc6 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc7 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc8 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc9 = table.Column<decimal>(type: "numeric", nullable: true),
                    From = table.Column<decimal>(type: "numeric", nullable: false),
                    To = table.Column<decimal>(type: "numeric", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProvincialTaxBiWeekly", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProvincialTaxMonthly",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Cc0 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc1 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc10 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc2 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc3 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc4 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc5 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc6 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc7 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc8 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc9 = table.Column<decimal>(type: "numeric", nullable: true),
                    From = table.Column<decimal>(type: "numeric", nullable: false),
                    To = table.Column<decimal>(type: "numeric", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProvincialTaxMonthly", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProvincialTaxSemiMonthly",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Cc0 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc1 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc10 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc2 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc3 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc4 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc5 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc6 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc7 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc8 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc9 = table.Column<decimal>(type: "numeric", nullable: true),
                    From = table.Column<decimal>(type: "numeric", nullable: false),
                    To = table.Column<decimal>(type: "numeric", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProvincialTaxSemiMonthly", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProvincialTaxWeekly",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Cc0 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc1 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc10 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc2 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc3 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc4 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc5 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc6 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc7 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc8 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc9 = table.Column<decimal>(type: "numeric", nullable: true),
                    From = table.Column<decimal>(type: "numeric", nullable: false),
                    To = table.Column<decimal>(type: "numeric", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProvincialTaxWeekly", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxWeekly",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Cc0 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc1 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc10 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc2 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc3 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc4 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc5 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc6 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc7 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc8 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cc9 = table.Column<decimal>(type: "numeric", nullable: true),
                    From = table.Column<decimal>(type: "numeric", nullable: false),
                    To = table.Column<decimal>(type: "numeric", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxWeekly", x => x.Id);
                });

            migrationBuilder.Sql("""
                INSERT INTO "CppWeekly" ("Id", "From", "To", "Cpp", "Year")
                SELECT "Id", "From", "To", "Cpp", "Year" FROM "CppDeductions" WHERE "PayPeriod" = 1;
                """);

            migrationBuilder.Sql("""
                INSERT INTO "CppBiWeekly" ("Id", "From", "To", "Cpp", "Year")
                SELECT "Id", "From", "To", "Cpp", "Year" FROM "CppDeductions" WHERE "PayPeriod" = 2;
                """);

            migrationBuilder.Sql("""
                INSERT INTO "CppSemiMonthly" ("Id", "From", "To", "Cpp", "Year")
                SELECT "Id", "From", "To", "Cpp", "Year" FROM "CppDeductions" WHERE "PayPeriod" = 3;
                """);

            migrationBuilder.Sql("""
                INSERT INTO "CppMonthly" ("Id", "From", "To", "Cpp", "Year")
                SELECT "Id", "From", "To", "Cpp", "Year" FROM "CppDeductions" WHERE "PayPeriod" = 4;
                """);

            migrationBuilder.Sql("""
                INSERT INTO "TaxWeekly" ("Id", "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year")
                SELECT "Id", "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year" FROM "TaxDeductions" WHERE "PayPeriod" = 1 AND "TaxType" = 1;
                """);

            migrationBuilder.Sql("""
                INSERT INTO "FederalTaxBiWeekly" ("Id", "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year")
                SELECT "Id", "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year" FROM "TaxDeductions" WHERE "PayPeriod" = 2 AND "TaxType" = 1;
                """);

            migrationBuilder.Sql("""
                INSERT INTO "FederalTaxSemiMonthly" ("Id", "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year")
                SELECT "Id", "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year" FROM "TaxDeductions" WHERE "PayPeriod" = 3 AND "TaxType" = 1;
                """);

            migrationBuilder.Sql("""
                INSERT INTO "FederalTaxMonthly" ("Id", "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year")
                SELECT "Id", "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year" FROM "TaxDeductions" WHERE "PayPeriod" = 4 AND "TaxType" = 1;
                """);

            migrationBuilder.Sql("""
                INSERT INTO "ProvincialTaxWeekly" ("Id", "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year")
                SELECT "Id", "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year" FROM "TaxDeductions" WHERE "PayPeriod" = 1 AND "TaxType" = 2;
                """);

            migrationBuilder.Sql("""
                INSERT INTO "ProvincialTaxBiWeekly" ("Id", "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year")
                SELECT "Id", "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year" FROM "TaxDeductions" WHERE "PayPeriod" = 2 AND "TaxType" = 2;
                """);

            migrationBuilder.Sql("""
                INSERT INTO "ProvincialTaxSemiMonthly" ("Id", "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year")
                SELECT "Id", "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year" FROM "TaxDeductions" WHERE "PayPeriod" = 3 AND "TaxType" = 2;
                """);

            migrationBuilder.Sql("""
                INSERT INTO "ProvincialTaxMonthly" ("Id", "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year")
                SELECT "Id", "From", "To", "Cc0", "Cc1", "Cc2", "Cc3", "Cc4", "Cc5", "Cc6", "Cc7", "Cc8", "Cc9", "Cc10", "Year" FROM "TaxDeductions" WHERE "PayPeriod" = 4 AND "TaxType" = 2;
                """);

            migrationBuilder.DropTable(
                name: "CppDeductions");

            migrationBuilder.DropTable(
                name: "TaxDeductions");
        }
    }
}
