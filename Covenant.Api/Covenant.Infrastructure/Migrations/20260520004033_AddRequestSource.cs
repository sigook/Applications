using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailableForRequests",
                table: "Sources",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "RequestSource",
                columns: table => new
                {
                    RequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExternalUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestSource", x => new { x.RequestId, x.SourceId });
                    table.ForeignKey(
                        name: "FK_RequestSource_Request_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestSource_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestSource_SourceId",
                table: "RequestSource",
                column: "SourceId");

            // Seed: flag ON for the only platforms used to publish requests today.
            migrationBuilder.Sql(
                "UPDATE \"Sources\" SET \"IsAvailableForRequests\" = TRUE " +
                "WHERE \"Value\" IN ('Indeed', 'Zip Recruiter');");

            // Aggregated bucket for LinkedIn / Instagram / Facebook / etc.
            migrationBuilder.Sql(
                "INSERT INTO \"Sources\" (\"Value\", \"IsAvailableForRequests\") " +
                "VALUES ('Social Media', TRUE);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestSource");

            migrationBuilder.Sql("DELETE FROM \"Sources\" WHERE \"Value\" = 'Social Media';");

            migrationBuilder.DropColumn(
                name: "IsAvailableForRequests",
                table: "Sources");
        }
    }
}
