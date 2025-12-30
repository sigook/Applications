using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Request
{
    public class WorkerRequestNoteConfiguration : IEntityTypeConfiguration<WorkerRequestNote>
    {
        public void Configure(EntityTypeBuilder<WorkerRequestNote> builder)
        {
            builder.ToTable("WorkerRequestNote");
            builder.HasKey(k => new { k.WorkerRequestId, k.NoteId });
        }
    }
}