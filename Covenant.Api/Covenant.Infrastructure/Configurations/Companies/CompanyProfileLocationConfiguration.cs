using Covenant.Common.Entities.Company;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Companies;

public class CompanyProfileLocationConfiguration : IEntityTypeConfiguration<CompanyProfileLocation>
{
    public void Configure(EntityTypeBuilder<CompanyProfileLocation> builder)
    {
        builder.ToTable("CompanyProfileLocations");
        builder.HasKey(l => new { l.CompanyProfileId, l.LocationId });
    }
}
