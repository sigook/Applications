using Covenant.Common.Entities.Deductions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Deductions;

public class ProvincialTaxBiWeeklyConfiguration : IEntityTypeConfiguration<ProvincialTaxBiWeekly>
{
    public void Configure(EntityTypeBuilder<ProvincialTaxBiWeekly> builder)
    {
        builder.ToTable("ProvincialTaxBiWeekly");
        builder.HasKey(x => x.Id);
    }
}
