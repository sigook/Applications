using Covenant.Common.Entities.Deductions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Deductions;

public class CppMonthlyConfiguration : IEntityTypeConfiguration<CppMonthly>
{
    public void Configure(EntityTypeBuilder<CppMonthly> builder)
    {
        builder.ToTable("CppMonthly");
        builder.HasKey(x => x.Id);
    }
}
