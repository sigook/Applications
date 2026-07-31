using Covenant.Common.Entities.Deductions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Deductions;

public class CppBiWeeklyConfiguration : IEntityTypeConfiguration<CppBiWeekly>
{
    public void Configure(EntityTypeBuilder<CppBiWeekly> builder)
    {
        builder.ToTable("CppBiWeekly");
        builder.HasKey(x => x.Id);
    }
}
