using Covenant.Common.Entities.Deductions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Deductions;

public class CppWeeklyConfiguration : IEntityTypeConfiguration<CppWeekly>
{
    public void Configure(EntityTypeBuilder<CppWeekly> builder)
    {
        builder.ToTable("CppWeekly");
        builder.HasKey(x => x.Id);
    }
}
