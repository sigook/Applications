using Covenant.Common.Entities.Company;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Companies;

public class CompanyProfileJobPositionRateConfiguration : IEntityTypeConfiguration<CompanyProfileJobPositionRate>
{
    public void Configure(EntityTypeBuilder<CompanyProfileJobPositionRate> builder)
    {
        builder.ToTable("CompanyProfileJobPositionRates");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.CompanyProfile)
            .WithMany(x => x.JobPositionRates)
            .HasForeignKey(x => x.CompanyProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Shift)
            .WithMany()
            .HasForeignKey(x => x.ShiftId)
            .IsRequired(false);
    }
}
