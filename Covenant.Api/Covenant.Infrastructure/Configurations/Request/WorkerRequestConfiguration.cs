using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Request
{
    public class WorkerRequestConfiguration : IEntityTypeConfiguration<WorkerRequest>
    {
        public void Configure(EntityTypeBuilder<WorkerRequest> builder)
        {
            builder.ToTable("WorkerRequests");
            builder.HasKey(k => k.Id);
            builder.HasIndex(r => new { r.RequestId, r.WorkerProfileId }).IsUnique();

            builder.HasOne(e => e.WorkerProfile)
                .WithMany(e => e.WorkerRequests)
                .HasForeignKey(e => e.WorkerProfileId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}