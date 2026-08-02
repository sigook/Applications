using Covenant.Common.Entities.Company;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Company;

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
