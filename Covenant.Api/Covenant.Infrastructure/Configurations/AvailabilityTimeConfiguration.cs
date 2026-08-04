using Covenant.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations;

public class AvailabilityTimeConfiguration : IEntityTypeConfiguration<AvailabilityTime>
{
    public void Configure(EntityTypeBuilder<AvailabilityTime> builder)
    {
        builder.ToTable("AvailabilityTimes");
        builder.HasKey(x => x.Id);
    }
}
