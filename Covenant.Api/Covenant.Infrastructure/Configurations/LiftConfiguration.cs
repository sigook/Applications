using Covenant.Common.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations;

public class LiftConfiguration : IEntityTypeConfiguration<Lift>
{
    public void Configure(EntityTypeBuilder<Lift> builder)
    {
        builder.ToTable("Lifts");
        builder.HasKey(x => x.Id);
    }
}
