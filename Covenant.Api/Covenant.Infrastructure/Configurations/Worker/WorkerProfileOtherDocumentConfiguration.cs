using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Worker
{
    public class WorkerProfileOtherDocumentConfiguration : IEntityTypeConfiguration<WorkerProfileOtherDocument>
    {
        public void Configure(EntityTypeBuilder<WorkerProfileOtherDocument> builder)
        {
            builder.ToTable("WorkerProfileOtherDocument");
            builder.HasKey(k => k.Id);
        }
    }
} 