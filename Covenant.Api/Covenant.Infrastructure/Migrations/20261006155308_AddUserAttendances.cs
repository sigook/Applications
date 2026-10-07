using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAttendances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserAttendances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ClockIn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ClockOut = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LunchMinutes = table.Column<int>(type: "integer", nullable: false),
                    EditedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EditedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EditReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAttendances_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAttendances_UserId_Date",
                table: "UserAttendances",
                columns: new[] { "UserId", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAttendances");
        }
    }
}
