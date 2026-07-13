using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.IdentityServer.Migrations
{
    /// <inheritdoc />
    public partial class RemoveInvalidUsers : Migration
    {
        private const string AnabelleAgencyId = "3db66ffd-f3bf-4628-a23f-f07aaf73cb75";

        private const string InvalidUsers = """
            '815cd42c-f695-40ac-9163-b98375408544',
            'd5438878-57fc-43cf-9b36-9de4fb74bdee',
            '7d9fa4b2-ab35-4c01-98b9-19584b8ff438',
            'ef22ab64-a525-423e-9011-dd83f85c5182'
            """;

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"""
                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM "Request" WHERE "AgencyId" = '{AnabelleAgencyId}')
                        OR EXISTS (SELECT 1 FROM "CompanyProfile" WHERE "AgencyId" = '{AnabelleAgencyId}')
                        OR EXISTS (SELECT 1 FROM "WorkerProfile" WHERE "AgencyId" = '{AnabelleAgencyId}')
                        OR EXISTS (SELECT 1 FROM "Candidate" WHERE "AgencyId" = '{AnabelleAgencyId}')
                        OR EXISTS (SELECT 1 FROM "Runner" WHERE "AgencyId" = '{AnabelleAgencyId}')
                        OR EXISTS (SELECT 1 FROM "AgencyLocation" WHERE "AgencyId" = '{AnabelleAgencyId}')
                        OR EXISTS (SELECT 1 FROM "AgencyWsibGroup" WHERE "AgencyId" = '{AnabelleAgencyId}')
                    THEN
                        RAISE EXCEPTION 'Agency % is not empty, aborting user cleanup', '{AnabelleAgencyId}';
                    END IF;

                    IF EXISTS (SELECT 1 FROM "CompanyUser" WHERE "UserId" IN ({InvalidUsers}))
                        OR EXISTS (SELECT 1 FROM "WorkerDispatch" WHERE "UserId" IN ({InvalidUsers}))
                        OR EXISTS (SELECT 1 FROM "AgencyPersonnel" WHERE "UserId" IN ({InvalidUsers}) AND "AgencyId" <> '{AnabelleAgencyId}')
                    THEN
                        RAISE EXCEPTION 'Users pending removal still have Covenant references, aborting user cleanup';
                    END IF;
                END $$;
                """);

            migrationBuilder.Sql($"""
                DELETE FROM "UserNotificationType" WHERE "UserId" IN ({InvalidUsers});
                DELETE FROM "AgencyPersonnel" WHERE "UserId" IN ({InvalidUsers});
                DELETE FROM "Agency" WHERE "UserId" IN ({InvalidUsers});
                """);

            migrationBuilder.Sql($"""
                DELETE FROM "PersistedGrants" WHERE "SubjectId" IN ({InvalidUsers});
                DELETE FROM "InactiveUsers" WHERE "UserId" IN ({InvalidUsers});
                DELETE FROM "UserToken" WHERE "UserId" IN ({InvalidUsers});
                DELETE FROM "UserLogin" WHERE "UserId" IN ({InvalidUsers});
                DELETE FROM "UserClaim" WHERE "UserId" IN ({InvalidUsers});
                DELETE FROM "UserRole" WHERE "UserId" IN ({InvalidUsers});
                DELETE FROM "User" WHERE "Id" IN ({InvalidUsers});
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
