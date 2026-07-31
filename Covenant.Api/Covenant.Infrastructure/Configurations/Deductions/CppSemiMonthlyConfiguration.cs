using Covenant.Common.Entities.Deductions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Deductions;

public class CppSemiMonthlyConfiguration : IEntityTypeConfiguration<CppSemiMonthly>
{
    public void Configure(EntityTypeBuilder<CppSemiMonthly> builder)
    {
        builder.ToTable("CppSemiMonthly");
        builder.HasKey(x => x.Id);
    }
}
