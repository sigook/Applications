using Covenant.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations
{
    public class SourceConfiguration : IEntityTypeConfiguration<Source>
    {
        public void Configure(EntityTypeBuilder<Source> builder)
        {
            builder.ToTable("Sources");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            builder.Property(x => x.Value).IsRequired();
            builder.Property(x => x.IsAvailableForRequests).HasDefaultValue(false);
            builder.HasIndex(x => x.Value).IsUnique();
        }
    }
}
