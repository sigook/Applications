using Covenant.Common.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations;

public class AvailabilityTimeConfiguration : IEntityTypeConfiguration<AvailabilityTime>
{
    public void Configure(EntityTypeBuilder<AvailabilityTime> builder)
    {
        builder.ToTable("AvailabilityTimes");
        builder.HasKey(x => x.Id);
    }
}
