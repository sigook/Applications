using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CleanupCandidatesAlreadyWorkers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM ""RequestApplicant"" ra
                USING ""Candidates"" c, ""User"" u, ""WorkerProfile"" wp
                WHERE ra.""CandidateId"" = c.""Id""
                  AND wp.""WorkerId"" = u.""Id""
                  AND c.""Email"" IS NOT NULL
                  AND TRIM(c.""Email"") <> ''
                  AND LOWER(TRIM(c.""Email"")) = LOWER(TRIM(u.""Email""));");

            migrationBuilder.Sql(@"
                DELETE FROM ""Candidates"" c
                USING ""User"" u, ""WorkerProfile"" wp
                WHERE wp.""WorkerId"" = u.""Id""
                  AND c.""Email"" IS NOT NULL
                  AND TRIM(c.""Email"") <> ''
                  AND LOWER(TRIM(c.""Email"")) = LOWER(TRIM(u.""Email""));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
