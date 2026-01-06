using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    public partial class RemoveUnusedTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppCommissionChargeError");

            migrationBuilder.DropTable(
                name: "InvoiceChargeError");

            migrationBuilder.DropTable(
                name: "PayrollReportHolidayPay");

            migrationBuilder.DropTable(
                name: "PayrollReportItem");

            migrationBuilder.DropTable(
                name: "PayrollReportOtherDeduction");

            migrationBuilder.DropTable(
                name: "PayrollReportTotal");

            migrationBuilder.DropTable(
                name: "PayrollReportWageDetail");

            migrationBuilder.DropTable(
                name: "SummaryReportAdditionalItem");

            migrationBuilder.DropTable(
                name: "SummaryReportDiscount");

            migrationBuilder.DropTable(
                name: "SummaryReportHoliday");

            migrationBuilder.DropTable(
                name: "SummaryReportTotal");

            migrationBuilder.DropTable(
                name: "AppCommissionCharge");

            migrationBuilder.DropTable(
                name: "InvoiceCharge");

            migrationBuilder.DropTable(
                name: "PayrollReport");

            migrationBuilder.DropTable(
                name: "AppCommission");

            migrationBuilder.DropTable(
                name: "SummaryReport");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvoiceCharge",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Attempts = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastAttempt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StripeChargeId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceCharge", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceCharge_Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayrollReport",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkerProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Cpp = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateWorkBegins = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateWorkEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Ei = table.Column<decimal>(type: "numeric", nullable: false),
                    FederalTax = table.Column<decimal>(type: "numeric", nullable: false),
                    GrossPayment = table.Column<decimal>(type: "numeric", nullable: false),
                    NumberId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OtherDeductions = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProvincialTax = table.Column<decimal>(type: "numeric", nullable: false),
                    PublicHolidayPay = table.Column<decimal>(type: "numeric", nullable: false),
                    RegularWage = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalDeductions = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalEarnings = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalPaid = table.Column<decimal>(type: "numeric", nullable: false),
                    TypeOfWork = table.Column<string>(type: "text", nullable: true),
                    Vacations = table.Column<decimal>(type: "numeric", nullable: false),
                    WeekEnding = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollReport_WorkerProfile_WorkerProfileId",
                        column: x => x.WorkerProfileId,
                        principalTable: "WorkerProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SummaryReport",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    BonusRate = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    HolidayRate = table.Column<decimal>(type: "numeric", nullable: false),
                    NightShiftRate = table.Column<decimal>(type: "numeric", nullable: false),
                    NumberId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OverTimeRate = table.Column<decimal>(type: "numeric", nullable: false),
                    SubTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalNet = table.Column<decimal>(type: "numeric", nullable: false),
                    VacationsRate = table.Column<decimal>(type: "numeric", nullable: false),
                    WeekEnding = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummaryReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SummaryReport_CompanyProfile_CompanyProfileId",
                        column: x => x.CompanyProfileId,
                        principalTable: "CompanyProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceChargeError",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceChargeId = table.Column<Guid>(type: "uuid", nullable: false),
                    FailureMessage = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceChargeError", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceChargeError_InvoiceCharge_InvoiceChargeId",
                        column: x => x.InvoiceChargeId,
                        principalTable: "InvoiceCharge",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayrollReportHolidayPay",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PayrollReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Holiday = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollReportHolidayPay", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollReportHolidayPay_PayrollReport_PayrollReportId",
                        column: x => x.PayrollReportId,
                        principalTable: "PayrollReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayrollReportItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PayrollReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<double>(type: "double precision", nullable: false),
                    Total = table.Column<decimal>(type: "numeric", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollReportItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollReportItem_PayrollReport_PayrollReportId",
                        column: x => x.PayrollReportId,
                        principalTable: "PayrollReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayrollReportOtherDeduction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PayrollReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<double>(type: "double precision", nullable: false),
                    Total = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollReportOtherDeduction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollReportOtherDeduction_PayrollReport_PayrollReportId",
                        column: x => x.PayrollReportId,
                        principalTable: "PayrollReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayrollReportTotal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PayrollReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeSheetTotalId = table.Column<Guid>(type: "uuid", nullable: false),
                    Holiday = table.Column<decimal>(type: "numeric", nullable: false),
                    Missing = table.Column<decimal>(type: "numeric", nullable: false),
                    MissingOvertime = table.Column<decimal>(type: "numeric", nullable: false),
                    NightShift = table.Column<decimal>(type: "numeric", nullable: false),
                    Overtime = table.Column<decimal>(type: "numeric", nullable: false),
                    Regular = table.Column<decimal>(type: "numeric", nullable: false),
                    WorkerRate = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollReportTotal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollReportTotal_PayrollReport_PayrollReportId",
                        column: x => x.PayrollReportId,
                        principalTable: "PayrollReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PayrollReportTotal_TimeSheetTotal_TimeSheetTotalId",
                        column: x => x.TimeSheetTotalId,
                        principalTable: "TimeSheetTotal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayrollReportWageDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PayrollReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeSheetTotalId = table.Column<Guid>(type: "uuid", nullable: false),
                    Holiday = table.Column<decimal>(type: "numeric", nullable: false),
                    Missing = table.Column<decimal>(type: "numeric", nullable: false),
                    MissingOvertime = table.Column<decimal>(type: "numeric", nullable: false),
                    NightShift = table.Column<decimal>(type: "numeric", nullable: false),
                    OtherRegular = table.Column<decimal>(type: "numeric", nullable: false),
                    Overtime = table.Column<decimal>(type: "numeric", nullable: false),
                    Regular = table.Column<decimal>(type: "numeric", nullable: false),
                    WorkerRate = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollReportWageDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollReportWageDetail_PayrollReport_PayrollReportId",
                        column: x => x.PayrollReportId,
                        principalTable: "PayrollReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PayrollReportWageDetail_TimeSheetTotalPayroll_TimeSheetTota~",
                        column: x => x.TimeSheetTotalId,
                        principalTable: "TimeSheetTotalPayroll",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppCommission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SummaryReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    CommissionRate = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Hst = table.Column<decimal>(type: "numeric", nullable: false),
                    NumberId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Subtotal = table.Column<decimal>(type: "numeric", nullable: false),
                    Total = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCommission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCommission_SummaryReport_SummaryReportId",
                        column: x => x.SummaryReportId,
                        principalTable: "SummaryReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SummaryReportAdditionalItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SummaryReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<double>(type: "double precision", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummaryReportAdditionalItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SummaryReportAdditionalItem_SummaryReport_SummaryReportId",
                        column: x => x.SummaryReportId,
                        principalTable: "SummaryReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SummaryReportDiscount",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SummaryReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummaryReportDiscount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SummaryReportDiscount_SummaryReport_SummaryReportId",
                        column: x => x.SummaryReportId,
                        principalTable: "SummaryReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SummaryReportHoliday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SummaryReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkerProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Holiday = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Hours = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummaryReportHoliday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SummaryReportHoliday_SummaryReport_SummaryReportId",
                        column: x => x.SummaryReportId,
                        principalTable: "SummaryReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SummaryReportHoliday_WorkerProfile_WorkerProfileId",
                        column: x => x.WorkerProfileId,
                        principalTable: "WorkerProfile",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SummaryReportTotal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SummaryReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeSheetTotalId = table.Column<Guid>(type: "uuid", nullable: false),
                    AgencyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    Holiday = table.Column<decimal>(type: "numeric", nullable: false),
                    Missing = table.Column<decimal>(type: "numeric", nullable: false),
                    MissingOvertime = table.Column<decimal>(type: "numeric", nullable: false),
                    NightShift = table.Column<decimal>(type: "numeric", nullable: false),
                    OtherRegular = table.Column<decimal>(type: "numeric", nullable: false),
                    Overtime = table.Column<decimal>(type: "numeric", nullable: false),
                    Regular = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalGross = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalNet = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummaryReportTotal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SummaryReportTotal_SummaryReport_SummaryReportId",
                        column: x => x.SummaryReportId,
                        principalTable: "SummaryReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SummaryReportTotal_TimeSheetTotal_TimeSheetTotalId",
                        column: x => x.TimeSheetTotalId,
                        principalTable: "TimeSheetTotal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppCommissionCharge",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AppCommissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Attempts = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastAttempt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StripeChargeId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCommissionCharge", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCommissionCharge_AppCommission_AppCommissionId",
                        column: x => x.AppCommissionId,
                        principalTable: "AppCommission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppCommissionChargeError",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AppCommissionChargeId = table.Column<Guid>(type: "uuid", nullable: false),
                    FailureMessage = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCommissionChargeError", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCommissionChargeError_AppCommissionCharge_AppCommissionC~",
                        column: x => x.AppCommissionChargeId,
                        principalTable: "AppCommissionCharge",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppCommission_SummaryReportId",
                table: "AppCommission",
                column: "SummaryReportId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCommissionCharge_AppCommissionId",
                table: "AppCommissionCharge",
                column: "AppCommissionId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCommissionChargeError_AppCommissionChargeId",
                table: "AppCommissionChargeError",
                column: "AppCommissionChargeId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCharge_InvoiceId",
                table: "InvoiceCharge",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceChargeError_InvoiceChargeId",
                table: "InvoiceChargeError",
                column: "InvoiceChargeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollReport_WorkerProfileId",
                table: "PayrollReport",
                column: "WorkerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollReportHolidayPay_PayrollReportId",
                table: "PayrollReportHolidayPay",
                column: "PayrollReportId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollReportItem_PayrollReportId",
                table: "PayrollReportItem",
                column: "PayrollReportId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollReportOtherDeduction_PayrollReportId",
                table: "PayrollReportOtherDeduction",
                column: "PayrollReportId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollReportTotal_PayrollReportId",
                table: "PayrollReportTotal",
                column: "PayrollReportId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollReportTotal_TimeSheetTotalId",
                table: "PayrollReportTotal",
                column: "TimeSheetTotalId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollReportWageDetail_PayrollReportId",
                table: "PayrollReportWageDetail",
                column: "PayrollReportId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollReportWageDetail_TimeSheetTotalId",
                table: "PayrollReportWageDetail",
                column: "TimeSheetTotalId");

            migrationBuilder.CreateIndex(
                name: "IX_SummaryReport_CompanyProfileId",
                table: "SummaryReport",
                column: "CompanyProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_SummaryReportAdditionalItem_SummaryReportId",
                table: "SummaryReportAdditionalItem",
                column: "SummaryReportId");

            migrationBuilder.CreateIndex(
                name: "IX_SummaryReportDiscount_SummaryReportId",
                table: "SummaryReportDiscount",
                column: "SummaryReportId");

            migrationBuilder.CreateIndex(
                name: "IX_SummaryReportHoliday_SummaryReportId",
                table: "SummaryReportHoliday",
                column: "SummaryReportId");

            migrationBuilder.CreateIndex(
                name: "IX_SummaryReportHoliday_WorkerProfileId",
                table: "SummaryReportHoliday",
                column: "WorkerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_SummaryReportTotal_SummaryReportId",
                table: "SummaryReportTotal",
                column: "SummaryReportId");

            migrationBuilder.CreateIndex(
                name: "IX_SummaryReportTotal_TimeSheetTotalId",
                table: "SummaryReportTotal",
                column: "TimeSheetTotalId");
        }
    }
}
