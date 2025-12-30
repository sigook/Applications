using Covenant.Common.Entities.Company;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Company
{
    public class CompanyProfileConfiguration : IEntityTypeConfiguration<CompanyProfile>
    {
        public void Configure(EntityTypeBuilder<CompanyProfile> builder)
        {
            builder.Property(c => c.NumberId).ValueGeneratedOnAdd();
            builder.HasIndex(p => new { p.CompanyId, p.AgencyId }).IsUnique();
        }
    }
}