using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Worker;

public class WorkerProfileNoteConfiguration : IEntityTypeConfiguration<WorkerProfileNote>
{
    public void Configure(EntityTypeBuilder<WorkerProfileNote> builder)
    {
        builder.ToTable("WorkerProfileNotes");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.WorkerProfile)
            .WithMany(x => x.Notes)
            .HasForeignKey(x => x.WorkerProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
