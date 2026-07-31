using Covenant.Common.Entities.Company;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Company
{
    public class CompanyUserConfiguration : IEntityTypeConfiguration<CompanyUser>
    {
        public void Configure(EntityTypeBuilder<CompanyUser> builder)
        {
            builder.ToTable("CompanyUsers");
            builder.HasIndex(a => new { a.CompanyProfileId, a.UserId }).IsUnique();

            builder.HasOne(c => c.CompanyProfile)
                .WithMany()
                .HasForeignKey(c => c.CompanyProfileId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
