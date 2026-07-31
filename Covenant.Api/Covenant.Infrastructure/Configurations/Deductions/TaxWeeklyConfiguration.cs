using Covenant.Common.Entities.Deductions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Deductions;

public class TaxWeeklyConfiguration : IEntityTypeConfiguration<TaxWeekly>
{
    public void Configure(EntityTypeBuilder<TaxWeekly> builder)
    {
        builder.ToTable("TaxWeekly");
        builder.HasKey(x => x.Id);
    }
}
