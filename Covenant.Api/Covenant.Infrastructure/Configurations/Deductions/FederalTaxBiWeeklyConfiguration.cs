using Covenant.Common.Entities.Deductions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Deductions;

public class FederalTaxBiWeeklyConfiguration : IEntityTypeConfiguration<FederalTaxBiWeekly>
{
    public void Configure(EntityTypeBuilder<FederalTaxBiWeekly> builder)
    {
        builder.ToTable("FederalTaxBiWeekly");
        builder.HasKey(x => x.Id);
    }
}
