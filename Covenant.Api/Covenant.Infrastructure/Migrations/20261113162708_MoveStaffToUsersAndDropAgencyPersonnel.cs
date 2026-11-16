using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoveStaffToUsersAndDropAgencyPersonnel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfiles_AgencyPersonnel_SalesRepresentativeId",
                table: "CompanyProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestComissions_AgencyPersonnel_AgencyPersonnelId",
                table: "RequestComissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestRecruiters_AgencyPersonnel_RecruiterId",
                table: "RequestRecruiters");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "Users" u
                SET "Name" = COALESCE(
                    (SELECT p."Name" FROM "AgencyPersonnel" p
                     WHERE p."UserId" = u."Id" AND NULLIF(TRIM(p."Name"), '') IS NOT NULL
                     ORDER BY p."IsPrimary" DESC, p."CreatedAt"
                     LIMIT 1),
                    split_part(u."Email", '@', 1))
                WHERE EXISTS (SELECT 1 FROM "AgencyPersonnel" p WHERE p."UserId" = u."Id");
                """);

            migrationBuilder.RenameColumn(
                name: "AgencyPersonnelId",
                table: "RequestComissions",
                newName: "SalesRepresentativeId");

            migrationBuilder.Sql("""
                DO $$
                DECLARE index_name text;
                BEGIN
                    FOR index_name IN
                        SELECT i.relname
                        FROM pg_index x
                        JOIN pg_class i ON i.oid = x.indexrelid
                        JOIN pg_class t ON t.oid = x.indrelid
                        JOIN pg_attribute a ON a.attrelid = t.oid AND a.attnum = ANY (x.indkey)
                        WHERE t.relname = 'RequestComissions'
                          AND a.attname = 'SalesRepresentativeId'
                          AND x.indnatts = 1
                          AND NOT x.indisprimary
                    LOOP
                        EXECUTE format('DROP INDEX %I', index_name);
                    END LOOP;
                END $$;

                CREATE INDEX "IX_RequestComissions_SalesRepresentativeId" ON "RequestComissions" ("SalesRepresentativeId");

                UPDATE "RequestComissions" rc
                SET "SalesRepresentativeId" = p."UserId"
                FROM "AgencyPersonnel" p
                WHERE p."Id" = rc."SalesRepresentativeId";

                UPDATE "CompanyProfiles" cp
                SET "SalesRepresentativeId" = p."UserId"
                FROM "AgencyPersonnel" p
                WHERE p."Id" = cp."SalesRepresentativeId";

                CREATE TEMP TABLE "RecruiterUserMap" ON COMMIT DROP AS
                SELECT rr."Id",
                       p."UserId",
                       FIRST_VALUE(rr."Id") OVER (
                           PARTITION BY rr."RequestId", p."UserId", rr."WorkDate"
                           ORDER BY rr."CreatedAt", rr."Id") AS "KeepId"
                FROM "RequestRecruiters" rr
                JOIN "AgencyPersonnel" p ON p."Id" = rr."RecruiterId";

                UPDATE "Runners" r
                SET "RequestRecruiterId" = m."KeepId"
                FROM "RecruiterUserMap" m
                WHERE r."RequestRecruiterId" = m."Id" AND m."Id" <> m."KeepId";

                DELETE FROM "RequestRecruiters" rr
                USING "RecruiterUserMap" m
                WHERE rr."Id" = m."Id" AND m."Id" <> m."KeepId";

                UPDATE "RequestRecruiters" rr
                SET "RecruiterId" = m."UserId"
                FROM "RecruiterUserMap" m
                WHERE rr."Id" = m."Id";
                """);

            migrationBuilder.DropTable(
                name: "AgencyPersonnel");

            migrationBuilder.Sql("""
                DELETE FROM "UserNotificationTypes" WHERE "NotificationTypeId" IN (9, 41, 47, 71, 73);
                DELETE FROM "NotificationTypes" WHERE "Id" IN (9, 41, 47, 71, 73);
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfiles_Users_SalesRepresentativeId",
                table: "CompanyProfiles",
                column: "SalesRepresentativeId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestComissions_Users_SalesRepresentativeId",
                table: "RequestComissions",
                column: "SalesRepresentativeId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestRecruiters_Users_RecruiterId",
                table: "RequestRecruiters",
                column: "RecruiterId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfiles_Users_SalesRepresentativeId",
                table: "CompanyProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestComissions_Users_SalesRepresentativeId",
                table: "RequestComissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestRecruiters_Users_RecruiterId",
                table: "RequestRecruiters");

            migrationBuilder.Sql("""
                INSERT INTO "NotificationTypes" ("Id", "Title", "Description", "Target") VALUES
                (9, 'New Request', 'When new request has been created', 0),
                (41, 'Worker Apply', 'When a worker apply to requests', 0),
                (47, 'Worker Decline', 'When a worker isn''t going to work', 0),
                (71, 'Request has been canceled', 'When request has been canceled', 0),
                (73, 'New Worker Requested', 'When a new worker is requested', 0)
                ON CONFLICT ("Id") DO NOTHING;
                """);

            migrationBuilder.CreateTable(
                name: "AgencyPersonnel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AgencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgencyPersonnel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgencyPersonnel_Agencies_AgencyId",
                        column: x => x.AgencyId,
                        principalTable: "Agencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgencyPersonnel_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgencyPersonnel_AgencyId_UserId",
                table: "AgencyPersonnel",
                columns: new[] { "AgencyId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AgencyPersonnel_UserId",
                table: "AgencyPersonnel",
                column: "UserId");

            migrationBuilder.Sql("""
                UPDATE "AgencyPersonnel" p
                SET "Name" = u."Name"
                FROM "Users" u
                WHERE u."Id" = p."UserId";

                UPDATE "CompanyProfiles" cp
                SET "SalesRepresentativeId" = (
                    SELECT p."Id" FROM "AgencyPersonnel" p
                    WHERE p."UserId" = cp."SalesRepresentativeId"
                    ORDER BY (p."AgencyId" = cp."AgencyId") DESC, p."IsPrimary" DESC
                    LIMIT 1)
                WHERE cp."SalesRepresentativeId" IS NOT NULL;

                UPDATE "RequestComissions" rc
                SET "SalesRepresentativeId" = (
                    SELECT p."Id" FROM "AgencyPersonnel" p
                    JOIN "Requests" r ON r."Id" = rc."RequestId"
                    JOIN "CompanyProfiles" cp ON cp."Id" = r."CompanyProfileId"
                    WHERE p."UserId" = rc."SalesRepresentativeId"
                    ORDER BY (p."AgencyId" = cp."AgencyId") DESC, p."IsPrimary" DESC
                    LIMIT 1);

                UPDATE "RequestRecruiters" rr
                SET "RecruiterId" = (
                    SELECT p."Id" FROM "AgencyPersonnel" p
                    JOIN "Requests" r ON r."Id" = rr."RequestId"
                    JOIN "CompanyProfiles" cp ON cp."Id" = r."CompanyProfileId"
                    WHERE p."UserId" = rr."RecruiterId"
                    ORDER BY (p."AgencyId" = cp."AgencyId") DESC, p."IsPrimary" DESC
                    LIMIT 1);
                """);

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "SalesRepresentativeId",
                table: "RequestComissions",
                newName: "AgencyPersonnelId");

            migrationBuilder.Sql("""
                DROP INDEX IF EXISTS "IX_RequestComissions_SalesRepresentativeId";
                CREATE INDEX "IX_RequestComissions_AgencyPersonnelId" ON "RequestComissions" ("AgencyPersonnelId");
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfiles_AgencyPersonnel_SalesRepresentativeId",
                table: "CompanyProfiles",
                column: "SalesRepresentativeId",
                principalTable: "AgencyPersonnel",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestComissions_AgencyPersonnel_AgencyPersonnelId",
                table: "RequestComissions",
                column: "AgencyPersonnelId",
                principalTable: "AgencyPersonnel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestRecruiters_AgencyPersonnel_RecruiterId",
                table: "RequestRecruiters",
                column: "RecruiterId",
                principalTable: "AgencyPersonnel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
