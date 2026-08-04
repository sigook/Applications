using Covenant.Common.Entities.Company;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Company;

public class CompanyProfileIndustryConfiguration : IEntityTypeConfiguration<CompanyProfileIndustry>
{
    public void Configure(EntityTypeBuilder<CompanyProfileIndustry> builder)
    {
        builder.ToTable("CompanyProfileIndustries");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Industry)
            .WithMany(x => x.CompanyProfileIndustries)
            .HasForeignKey(x => x.IndustryId)
            .IsRequired(false);
    }
}
