using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.IdentityServer.Migrations
{
    /// <inheritdoc />
    public partial class RemoveInvalidUsers : Migration
    {
        private const string InvalidUsers = @"
            '815cd42c-f695-40ac-9163-b98375408544',
            'd5438878-57fc-43cf-9b36-9de4fb74bdee',
            '7d9fa4b2-ab35-4c01-98b9-19584b8ff438',
            'ef22ab64-a525-423e-9011-dd83f85c5182'";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                DELETE FROM ""PersistedGrants"" WHERE ""SubjectId"" IN ({InvalidUsers});
                DELETE FROM ""InactiveUsers"" WHERE ""UserId"" IN ({InvalidUsers});
                DELETE FROM ""UserToken"" WHERE ""UserId"" IN ({InvalidUsers});
                DELETE FROM ""UserLogin"" WHERE ""UserId"" IN ({InvalidUsers});
                DELETE FROM ""UserClaim"" WHERE ""UserId"" IN ({InvalidUsers});
                DELETE FROM ""UserRole"" WHERE ""UserId"" IN ({InvalidUsers});
                DELETE FROM ""User"" WHERE ""Id"" IN ({InvalidUsers});");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
