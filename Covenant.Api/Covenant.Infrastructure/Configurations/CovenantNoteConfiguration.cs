using Covenant.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations;

public class CovenantNoteConfiguration : IEntityTypeConfiguration<CovenantNote>
{
    public void Configure(EntityTypeBuilder<CovenantNote> builder)
    {
        builder.ToTable("CovenantNotes");
        builder.HasKey(x => x.Id);
    }
}
