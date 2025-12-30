using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    public partial class RemovePayStubUSA : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PayStubUSACompensation");

            migrationBuilder.DropTable(
                name: "PayStubUSADeduction");

            migrationBuilder.DropTable(
                name: "PayStubUSATax");

            migrationBuilder.DropTable(
                name: "PayStubUSATimeSheetTotalPayroll");

            migrationBuilder.DropTable(
                name: "PayStubUSA");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PayStubUSA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkerProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DateWorkBegins = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DateWorkEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    GrossPayment = table.Column<decimal>(type: "numeric", nullable: false),
                    NumberId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PayStubNumber = table.Column<string>(type: "text", nullable: true),
                    PayStubNumberId = table.Column<long>(type: "bigint", nullable: false),
                    PaymentDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TotalDeductions = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalEarnings = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalPaid = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalTaxes = table.Column<decimal>(type: "numeric", nullable: false),
                    TypeOfWork = table.Column<string>(type: "text", nullable: true),
                    WeekEnding = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayStubUSA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayStubUSA_WorkerProfile_WorkerProfileId",
                        column: x => x.WorkerProfileId,
                        principalTable: "WorkerProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayStubUSACompensation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PayStubUSAId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<double>(type: "double precision", nullable: false),
                    Total = table.Column<decimal>(type: "numeric", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayStubUSACompensation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayStubUSACompensation_PayStubUSA_PayStubUSAId",
                        column: x => x.PayStubUSAId,
                        principalTable: "PayStubUSA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayStubUSADeduction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PayStubUSAId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<double>(type: "double precision", nullable: false),
                    Total = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayStubUSADeduction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayStubUSADeduction_PayStubUSA_PayStubUSAId",
                        column: x => x.PayStubUSAId,
                        principalTable: "PayStubUSA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayStubUSATax",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PayStubUSAId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    Total = table.Column<decimal>(type: "numeric", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayStubUSATax", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayStubUSATax_PayStubUSA_PayStubUSAId",
                        column: x => x.PayStubUSAId,
                        principalTable: "PayStubUSA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayStubUSATimeSheetTotalPayroll",
                columns: table => new
                {
                    PayStubUSAId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeSheetTotalPayrollId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayStubUSATimeSheetTotalPayroll", x => new { x.PayStubUSAId, x.TimeSheetTotalPayrollId });
                    table.ForeignKey(
                        name: "FK_PayStubUSATimeSheetTotalPayroll_PayStubUSA_PayStubUSAId",
                        column: x => x.PayStubUSAId,
                        principalTable: "PayStubUSA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PayStubUSATimeSheetTotalPayroll_TimeSheetTotalPayroll_TimeS~",
                        column: x => x.TimeSheetTotalPayrollId,
                        principalTable: "TimeSheetTotalPayroll",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PayStubUSA_PayStubNumber",
                table: "PayStubUSA",
                column: "PayStubNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PayStubUSA_PayStubNumberId",
                table: "PayStubUSA",
                column: "PayStubNumberId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PayStubUSA_WorkerProfileId",
                table: "PayStubUSA",
                column: "WorkerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PayStubUSACompensation_PayStubUSAId",
                table: "PayStubUSACompensation",
                column: "PayStubUSAId");

            migrationBuilder.CreateIndex(
                name: "IX_PayStubUSADeduction_PayStubUSAId",
                table: "PayStubUSADeduction",
                column: "PayStubUSAId");

            migrationBuilder.CreateIndex(
                name: "IX_PayStubUSATax_PayStubUSAId",
                table: "PayStubUSATax",
                column: "PayStubUSAId");

            migrationBuilder.CreateIndex(
                name: "IX_PayStubUSATimeSheetTotalPayroll_TimeSheetTotalPayrollId",
                table: "PayStubUSATimeSheetTotalPayroll",
                column: "TimeSheetTotalPayrollId");
        }
    }
}
