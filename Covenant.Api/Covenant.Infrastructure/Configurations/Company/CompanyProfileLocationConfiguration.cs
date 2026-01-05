using Covenant.Common.Entities.Company;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Company
{
    public class CompanyProfileLocationConfiguration : IEntityTypeConfiguration<CompanyProfileLocation>
    {
        public void Configure(EntityTypeBuilder<CompanyProfileLocation> builder)
        {
            builder.ToTable("CompanyProfileLocation");
            builder.HasKey(l => new { l.CompanyProfileId, l.LocationId });
        }
    }
} 