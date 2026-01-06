using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Request
{
    public class WorkerRequestConfiguration : IEntityTypeConfiguration<WorkerRequest>
    {
        public void Configure(EntityTypeBuilder<WorkerRequest> builder)
        {
            builder.ToTable("WorkerRequest");
            builder.HasKey(k => k.Id);
            builder.HasIndex(r => new { r.RequestId, r.WorkerId }).IsUnique();
        }
    }
}