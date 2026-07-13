using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.IdentityServer.Migrations
{
    /// <inheritdoc />
    public partial class RoleRedesign : Migration
    {
        private const string RecruitingRoleId = "3bc47ccb-1756-4e19-be98-157c24eb0aac";
        private const string AdminRoleId = "c5363963-c994-4f04-b354-632e0380ed62";
        private const string SuperAdminRoleId = "5f2b9c1e-9d3a-4f77-9a1c-2e6b8d4f0a11";
        private const string SalesRoleId = "7c8e4d2b-6a19-4c53-8f2d-1b9e7a3c5d64";
        private const string PayrollRoleId = "a397e9e7-6a21-4bf2-a978-41780f2896aa";
        private const string AgencyRoleId = "cf4349a2-39cf-499f-bef7-4783b9652df4";

        private const string SuperAdminUsers = "'42520973-ab38-4715-806f-05222894a060'";

        private const string AdminUsers = """
            '9b8de846-892c-405e-9843-8f8c0bf67a3c',
            'b2505d4f-c7c4-4056-bcc2-40d965cca9d7',
            '8a2806af-836a-4289-b7d6-3bab3f64c4df',
            'fe4e7a59-4aa9-4d32-8dc3-a454fdbc23a0',
            '0af14bad-15b2-429a-b42e-8bc2fa433059',
            '8fd00ffe-6094-4329-a072-b7ccbb9ffdc6',
            'b20cac7c-6ed5-47da-9e04-2738f786061d',
            '71717e26-3db2-47d6-b6d4-ea455814e940',
            'ba044a52-9181-4426-96ea-30aa9525455d'
            """;

        private const string RecruitingUsers = """
            '6001cfce-c74d-4671-9764-d2710b4911cf',
            '6a70c60f-37b9-4544-b2c7-f7a72e11a0d4',
            '3a6dd788-1ec3-455c-b347-64396c1320f7',
            '65ac0700-a5a6-4f0f-bc41-f7cb2bcd3ede',
            '8575cb8c-56d7-4f21-9521-0f587aaeb438',
            'c786631c-a00e-4f4d-bbd3-d6f743302214',
            'a27a0f42-f4de-462f-929f-9dc8b84f393c'
            """;

        private const string PendingRemovalUsers = """
            '815cd42c-f695-40ac-9163-b98375408544',
            'd5438878-57fc-43cf-9b36-9de4fb74bdee',
            '7d9fa4b2-ab35-4c01-98b9-19584b8ff438',
            'ef22ab64-a525-423e-9011-dd83f85c5182'
            """;

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"""
                UPDATE "Rol" SET "Name" = 'recruiting', "NormalizedName" = 'RECRUITING'
                WHERE "Id" = '{RecruitingRoleId}';

                UPDATE "Rol" SET "Name" = 'admin', "NormalizedName" = 'ADMIN'
                WHERE "Id" = '{AdminRoleId}';

                INSERT INTO "Rol" ("Id", "Name", "NormalizedName", "ConcurrencyStamp")
                SELECT '{SuperAdminRoleId}', 'superadmin', 'SUPERADMIN', '{SuperAdminRoleId}'
                WHERE NOT EXISTS (SELECT 1 FROM "Rol" WHERE "NormalizedName" = 'SUPERADMIN');

                INSERT INTO "Rol" ("Id", "Name", "NormalizedName", "ConcurrencyStamp")
                SELECT '{SalesRoleId}', 'sales', 'SALES', '{SalesRoleId}'
                WHERE NOT EXISTS (SELECT 1 FROM "Rol" WHERE "NormalizedName" = 'SALES');
                """);

            migrationBuilder.Sql($"""
                DELETE FROM "UserRole"
                WHERE "UserId" IN ({SuperAdminUsers}, {AdminUsers}, {RecruitingUsers});

                INSERT INTO "UserRole" ("UserId", "RoleId")
                SELECT "Id", '{SuperAdminRoleId}' FROM "User" WHERE "Id" IN ({SuperAdminUsers});

                INSERT INTO "UserRole" ("UserId", "RoleId")
                SELECT "Id", '{AdminRoleId}' FROM "User" WHERE "Id" IN ({AdminUsers});

                INSERT INTO "UserRole" ("UserId", "RoleId")
                SELECT "Id", '{RecruitingRoleId}' FROM "User" WHERE "Id" IN ({RecruitingUsers});
                """);

            migrationBuilder.Sql($"""
                DELETE FROM "RoleClaim" WHERE "RoleId" IN ('{PayrollRoleId}', '{AgencyRoleId}');
                DELETE FROM "UserRole" WHERE "RoleId" IN ('{PayrollRoleId}', '{AgencyRoleId}');
                DELETE FROM "Rol" WHERE "Id" IN ('{PayrollRoleId}', '{AgencyRoleId}');
                """);

            migrationBuilder.Sql($"""
                DELETE FROM "PersistedGrants"
                WHERE "SubjectId" IN ({SuperAdminUsers}, {AdminUsers}, {RecruitingUsers}, {PendingRemovalUsers});
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
