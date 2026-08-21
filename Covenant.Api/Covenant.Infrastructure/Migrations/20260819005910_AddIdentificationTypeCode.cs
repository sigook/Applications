using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentificationTypeCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "IdentificationTypes",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "None");

            migrationBuilder.Sql("UPDATE \"IdentificationTypes\" SET \"Code\" = 'PassportCanadian' WHERE \"Value\" = 'Passport - Canadian';");
            migrationBuilder.Sql("UPDATE \"IdentificationTypes\" SET \"Code\" = 'PassportForeign' WHERE \"Value\" = 'Passport - Foreign';");
            migrationBuilder.Sql("UPDATE \"IdentificationTypes\" SET \"Code\" = 'PermanentResidentCard' WHERE \"Value\" = 'Permanent Resident Card';");
            migrationBuilder.Sql("UPDATE \"IdentificationTypes\" SET \"Code\" = 'ImmigrantVisa' WHERE \"Value\" = 'Immigrant Visa';");
            migrationBuilder.Sql("UPDATE \"IdentificationTypes\" SET \"Code\" = 'BirthCertificate' WHERE \"Value\" = 'Birth Certificate';");
            migrationBuilder.Sql("UPDATE \"IdentificationTypes\" SET \"Code\" = 'DriversLicense' WHERE \"Value\" = 'Driver''s License';");
            migrationBuilder.Sql("UPDATE \"IdentificationTypes\" SET \"Code\" = 'StudentCard' WHERE \"Value\" = 'Student Card';");
            migrationBuilder.Sql("UPDATE \"IdentificationTypes\" SET \"Code\" = 'RefugeeProtection' WHERE \"Value\" = 'Refugee Protection';");
            migrationBuilder.Sql("UPDATE \"IdentificationTypes\" SET \"Code\" = 'ProvinceStatePhotoCard' WHERE \"Value\" = 'Province/State Photo Card';");
            migrationBuilder.Sql("UPDATE \"IdentificationTypes\" SET \"Code\" = 'OntarioHealthCard' WHERE \"Value\" = 'Ontario Health Card';");
            migrationBuilder.Sql("UPDATE \"IdentificationTypes\" SET \"Code\" = 'UsPassport' WHERE \"Value\" = 'U.S. Passport/U.S. Passport Card';");
            migrationBuilder.Sql("UPDATE \"IdentificationTypes\" SET \"Code\" = 'SinSsn' WHERE \"Value\" = 'SIN/SSN';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "IdentificationTypes");
        }
    }
}
