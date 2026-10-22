using Covenant.Common.Entities.Requests;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Requests;

public class WorkerRequestNoteConfiguration : IEntityTypeConfiguration<WorkerRequestNote>
{
    public void Configure(EntityTypeBuilder<WorkerRequestNote> builder)
    {
        builder.ToTable("WorkerRequestNotes");
        builder.HasKey(k => new { k.WorkerRequestId, k.NoteId });
    }
}
