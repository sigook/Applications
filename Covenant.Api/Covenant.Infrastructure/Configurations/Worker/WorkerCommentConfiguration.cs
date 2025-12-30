using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Worker
{
    public class WorkerCommentConfiguration : IEntityTypeConfiguration<WorkerComment>
    {
        public void Configure(EntityTypeBuilder<WorkerComment> builder)
        {
            builder.ToTable("WorkerComment");
            builder.HasKey(k => k.Id);
            builder.Property(c => c.NumberId).ValueGeneratedOnAdd();
        }
    }
} 