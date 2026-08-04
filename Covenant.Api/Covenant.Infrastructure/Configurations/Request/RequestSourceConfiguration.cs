using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Request
{
    public class RequestSourceConfiguration : IEntityTypeConfiguration<RequestSource>
    {
        public void Configure(EntityTypeBuilder<RequestSource> builder)
        {
            builder.ToTable("RequestSources");
            builder.HasKey(k => new { k.RequestId, k.SourceId });

            builder.Property(p => p.ExternalUrl).IsRequired(false);
            builder.Property(p => p.PublishedAt).IsRequired(false);
            builder.Property(p => p.CreatedAt).ValueGeneratedOnAdd();

            builder.HasOne(r => r.Source)
                .WithMany()
                .HasForeignKey(f => f.SourceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
